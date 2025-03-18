using System;
using System.IO;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Newtonsoft.Json;
using ClearCare.Models.Interface;

namespace ClearCare.DataSource
{
    public class UserGateway : IAdminDatabase //, IUserDatabase
    {
        private FirestoreDb db;
        private readonly EncryptionManagement encryptionManagement;

        public UserGateway()
        {
            // Initialize Firebase
            db = FirebaseService.Initialize();
            // Initialize Encryption Management
            encryptionManagement = new EncryptionManagement();
        }

        // Query Firebase Firestore for a user where Email matches for Login Page.
        public async Task<User> findUserByEmail(string email)
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

            var docDictionary = doc.ToDictionary();

            // Use UserFactory to create the correct user object
            return UserFactory.createUser(userID, emailAddress, password, name, mobileNumber, address, role, docDictionary);
        }

        // Function to find user by ID
        public async Task<User> findUserByID(string userID)
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

            // Determine which object to return based on Role
            if (role == "Doctor")
            {
                string specialization = snapshot.GetValue<string>("Specialization");
                return new Doctor(userID, emailAddress, password, name, mobileNumber, address, role, specialization);
            }

            if (role == "Nurse")
            {
                string department = snapshot.GetValue<string>("Department");
                return new Nurse(userID, emailAddress, password, name, mobileNumber, address, role, department);
            }

            if (role == "Patient")
            {
                string assignedCaregiverName = snapshot.GetValue<string>("AssignedCaregiverName");
                string assignedCaregiverID = snapshot.GetValue<string>("AssignedCaregiverID");
                Timestamp dateOfBirth = snapshot.GetValue<Timestamp>("DateOfBirth");
                return new Patient(userID, emailAddress, password, name, mobileNumber, address, role, assignedCaregiverName, assignedCaregiverID, dateOfBirth);
            }

            if (role == "Caregiver")
            {
                string assignedPatientName = snapshot.GetValue<string>("AssignedPatientName");
                string assignedPatientID = snapshot.GetValue<string>("AssignedPatientID");
                return new Caregiver(userID, emailAddress, password, name, mobileNumber, address, role, assignedPatientName, assignedPatientID);
            }

            var docDictionary = snapshot.ToDictionary();

            // Default to generic User if no matching role found
            // return new User(userID, emailAddress, password, name, mobileNumber, address, role);
            // Use the UserFactory to create the appropriate user based on the role
            return UserFactory.createUser(userID, emailAddress, password, name, mobileNumber, address, role, docDictionary);

        }

        // Function to get all User in a list
        public async Task<List<User>> getAllUsers()
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

                        var docDictionary = document.ToDictionary();

                        // Use the UserFactory to create the appropriate user
                        User user = UserFactory.createUser(userID, email, password, name, mobileNumber, address, role, docDictionary);
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
        public async Task<string> findUserNameByID(string userID)
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
            return snapshot.ContainsField("Name") ? snapshot.GetValue<string>("Name") : "Unknown User";
        }

        // Insert a new user with auto-incremented UserID
        // public async Task<string> InsertUser(string email, string password, string name, long mobileNumber, string address, string role)
        // {
        //     CollectionReference usersRef = db.Collection("User");

        //     // Fetch all user documents to find the highest existing ID
        //     QuerySnapshot allUsersSnapshot = await usersRef.GetSnapshotAsync();
        //     int highestID = 0;

        //     foreach (var doc in allUsersSnapshot.Documents)
        //     {
        //         string docID = doc.Id; // Example: "USR3"
        //         if (docID.StartsWith("USR") && int.TryParse(docID.Substring(3), out int id))
        //         {
        //             highestID = Math.Max(highestID, id);
        //         }
        //     }

        //     // Generate User ID
        //     string nextUserID = $"USR{highestID + 1}";

        //     // Prepare user data for insertion
        //     var userData = new Dictionary<string, object>
        //     {
        //         { "Email", email },
        //         { "Password", encryptionManagement.hashPassword(password) },
        //         { "Name", name },
        //         { "MobileNumber", mobileNumber },
        //         { "Address", address },
        //         { "Role", role }
        //     };

        //     // Explicitly use the new User ID as the document ID
        //     DocumentReference newUserRef = usersRef.Document(nextUserID);
        //     await newUserRef.SetAsync(userData);

        //     Console.WriteLine($"User inserted successfully with Firestore ID: {nextUserID}");

        //     return nextUserID;
        // }

        // Insert a new staff user with auto-incremented UserID
        public async Task<string> InsertUser(User newUser, String password)
        {
            CollectionReference usersRef = db.Collection("User");

            // Fetch all user documents to find the highest existing ID
            QuerySnapshot allUsersSnapshot = await usersRef.GetSnapshotAsync();
            int highestID = 0;

            foreach (var doc in allUsersSnapshot.Documents)
            {
                string docID = doc.Id; // Example: "USR3"
                if (docID.StartsWith("USR") && int.TryParse(docID.Substring(3), out int id))
                {
                    highestID = Math.Max(highestID, id);
                }
            }

            // Generate User ID
            string nextUserID = $"USR{highestID + 1}";

            // Prepare user data for insertion
            var userData = newUser.getProfileData();
            userData.Remove("UserID");
            userData.Add("Password", encryptionManagement.hashPassword(password));

            if ((String)userData["Role"] == "Patient")
            {
                DateTime dobDateTime = DateTime.ParseExact((string)userData["DateOfBirth"], "dd MMMM yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                // Step 2: Convert to UTC (assuming the input string is in UTC+8)
                DateTime dobUtc = dobDateTime.AddHours(-8); // Subtract 8 hours to convert UTC+8 to UTC

                // Step 3: Create a Firestore Timestamp
                Timestamp dobTimestamp = Timestamp.FromDateTime(DateTime.SpecifyKind(dobUtc, DateTimeKind.Utc));
                userData["DateOfBirth"] = dobTimestamp;
            }

            // Explicitly use the new User ID as the document ID
            DocumentReference newUserRef = usersRef.Document(nextUserID);
            await newUserRef.SetAsync(userData);

            Console.WriteLine($"User inserted successfully with Firestore ID: {nextUserID}");

            return nextUserID;
        }

        public async Task<bool> updateUser(string userId, Dictionary<string, object> updatedFields)
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
                        if (updatedFields.ContainsKey("DateOfBirth") && updatedFields["DateOfBirth"] is Timestamp dobTimestamp)
                        {
                            updates["DateOfBirth"] = dobTimestamp; // Ensure correct type before storing
                        }
                        break;

                    default:
                        break;
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

        public async Task<bool> resetPassword(String uid, String password)
        {
            try
            {
                if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("Error: UserID or password is null or empty");
                    return false;
                }

                DocumentReference userDocRef = db.Collection("User").Document(uid);
                DocumentSnapshot snapshot = await userDocRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    Console.WriteLine($"Error: User not found. userID: {uid}");
                    return false;
                }

                Dictionary<string, object> updates = new Dictionary<string, object>
                {
                    { "Password", encryptionManagement.hashPassword(password) },
                    { "RequirePasswordChange", true }
                };

                try
                {
                    await userDocRef.UpdateAsync(updates);
                    Console.WriteLine($"Successfully reset password for user {uid}");
                    return true;
                }
                catch (Exception updateEx)
                {
                    Console.WriteLine($"Failed to update user document: {updateEx.Message}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in resetPassword: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> deleteUser(string uid)
        {
            try
            {
                DocumentReference userDocRef = db.Collection("User").Document(uid);
                DocumentSnapshot snapshot = await userDocRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                    throw new Exception("User not found. userID: " + uid);

                await userDocRef.DeleteAsync();

                Console.WriteLine($"Successfully deleted user {uid}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting user: " + ex.Message);
                return false;
            }
        }
    }
}
