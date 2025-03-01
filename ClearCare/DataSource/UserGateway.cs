using System.IO;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using ClearCare.Models.Entities;

namespace ClearCare.DataSource
{
    public class UserGateway
    {
        private FirestoreDb db;

        public UserGateway()
        {
            // Initialize Firebase
            db = FirebaseService.Initialize();
        }

        // Query Firebase Firestore for a user where Email matches for Login Page.
        public async Task<User> FindUserByEmail(string email)
        {
            Query query = db.Collection("User").WhereEqualTo("Email", email);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            if (snapshot.Documents.Count == 0)
            {
                Console.WriteLine($"User with email {email} not found in Firestore.");
                return null;
            }

            var doc = snapshot.Documents[0];

            if (!doc.Exists)
            {
                Console.WriteLine($"Document for email {email} exists in query, but Firestore returned empty data.");
                return null;
            }

            // Assign UserID from document ID
            string userID = doc.Id;
            string emailAddress = doc.GetValue<string>("Email");
            string password = doc.GetValue<string>("Password");
            string name = doc.GetValue<string>("Name");
            long mobileNumber = doc.GetValue<long>("MobileNumber");
            string address = doc.GetValue<string>("Address");
            string role = doc.GetValue<string>("Role");

            // Default to generic User if no matching role found
            return new User(userID, emailAddress, password, name, mobileNumber, address, role);
        }

        // Function to find user by ID
        public async Task<User> FindUserByID(string userID)
        {
            DocumentReference docRef = db.Collection("User").Document(userID);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Console.WriteLine($"User {userID} not found in Firestore.");
                return null;
            }

            // Assign UserID from document ID
            string role = snapshot.GetValue<string>("Role");
            string emailAddress = snapshot.GetValue<string>("Email");
            string password = snapshot.GetValue<string>("Password");
            string name = snapshot.GetValue<string>("Name");
            int mobileNumber = (int)snapshot.GetValue<long>("MobileNumber");
            string address = snapshot.GetValue<string>("Address");

            // Use the UserFactory to create the appropriate user based on the role
            return UserFactory.CreateUser(userID, emailAddress, password, name, mobileNumber, address, role, snapshot);
            
        }

        // Function to get all User in a list
        public async Task<List<User>> GetAllUsers()
        {
            List<User> userList = new List<User>();

            QuerySnapshot snapshot = await db.Collection("User").GetSnapshotAsync();

            if (snapshot.Documents.Count == 0)
            {
                Console.WriteLine("No users found in Firestore.");
            }

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    try
                    {
                        string userID = document.ContainsField("UserID") ? document.GetValue<string>("UserID") : document.Id;
                        string email = document.ContainsField("Email") ? document.GetValue<string>("Email") : "unknown@example.com";
                        string password = document.ContainsField("Password") ? document.GetValue<string>("Password") : "";
                        string name = document.ContainsField("Name") ? document.GetValue<string>("Name") : "Unknown";
                        long mobileNumber = document.ContainsField("MobileNumber") ? document.GetValue<long>("MobileNumber") : 0;
                        string address = document.ContainsField("Address") ? document.GetValue<string>("Address") : "Unknown";
                        string role = document.ContainsField("Role") ? document.GetValue<string>("Role") : "User";

                        // Use the UserFactory to create the appropriate user
                        User user = UserFactory.CreateUser(userID, email, password, name, mobileNumber, address, role, document);
                        userList.Add(user);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting user {document.Id}: {ex.Message}");
                    }
                }
            }

            return userList;
        }

        // Method to retrieve the user's name based on their ID
        public async Task<string> FindUserNameByID(string userID)
        {
            // Check for null or empty userID
            if (string.IsNullOrEmpty(userID))
            {
                Console.WriteLine("Error: UserID is null or empty.");
                return "Unknown User";
            }

            // Fetch user document from Firestore
            DocumentReference docRef = db.Collection("User").Document(userID);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            // Check if document exists
            if (!snapshot.Exists)
            {
                Console.WriteLine($"User with ID {userID} not found in Firestore.");
                return "Unknown User";
            }

            // Check if "Name" field exists
            if (snapshot.ContainsField("Name"))
            {
                string userName = snapshot.GetValue<string>("Name");
                return string.IsNullOrEmpty(userName) ? "Unknown User" : userName;
            }
            else
            {
                Console.WriteLine($"User {userID} exists but has no 'Name' field.");
                return "Unknown User";
            }
        }

        public async Task<bool> UpdateUserProfile(string userId, Dictionary<string, object> updatedFields)
        {
            try
            {
                DocumentReference userDocRef = db.Collection("User").Document(userId);
                DocumentSnapshot snapshot = await userDocRef.GetSnapshotAsync();
                
                if (!snapshot.Exists)
                    throw new Exception("User not found. userID: " + userId);
                
                Dictionary<string, object> updates = new Dictionary<string, object>();
                
                // Ensure only selected base user fields are updated
                if (updatedFields.ContainsKey("Email"))
                    updates["Email"] = updatedFields["Email"].ToString();
                if (updatedFields.ContainsKey("Name"))
                    updates["Name"] = updatedFields["Name"].ToString();
                if (updatedFields.ContainsKey("MobileNumber"))
                    updates["MobileNumber"] = Convert.ToInt64(updatedFields["MobileNumber"]);
                if (updatedFields.ContainsKey("Address"))
                    updates["Address"] = updatedFields["Address"].ToString();
                if (updatedFields.ContainsKey("Password"))
                    updates["Password"] = updatedFields["Password"].ToString();
                
                string role = snapshot.GetValue<string>("Role");
                
                switch (role)
                {
                    case "Nurse":
                        if (updatedFields.ContainsKey("Department"))
                            updates["Department"] = updatedFields["Department"].ToString();
                        break;
                    
                    case "Doctor":
                        if (updatedFields.ContainsKey("Specialization"))
                            updates["Specialization"] = updatedFields["Specialization"].ToString();
                        break;
                    
                    case "Patient":
                        if (updatedFields.ContainsKey("AssignedCaregiverName"))
                            updates["AssignedCaregiverName"] = updatedFields["AssignedCaregiverName"].ToString();
                        if (updatedFields.ContainsKey("AssignedCaregiverID"))
                            updates["AssignedCaregiverID"] = updatedFields["AssignedCaregiverID"].ToString();
                        if (updatedFields.ContainsKey("DateOfBirth"))
                            updates["DateOfBirth"] = Timestamp.FromDateTime(DateTime.Parse(updatedFields["DateOfBirth"].ToString()).ToUniversalTime());
                        break;
                    
                    case "Caregiver":
                        if (updatedFields.ContainsKey("AssignedPatientName"))
                            updates["AssignedPatientName"] = updatedFields["AssignedPatientName"].ToString();
                        if (updatedFields.ContainsKey("AssignedPatientID"))
                            updates["AssignedPatientID"] = updatedFields["AssignedPatientID"].ToString();
                        break;
                    
                    default:
                        throw new Exception("Invalid role detected.");
                }
                
                if (updates.Count > 0)
                    await userDocRef.UpdateAsync(updates);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating user profile: " + ex.Message);
                return false;
            }
        }
    }
}
