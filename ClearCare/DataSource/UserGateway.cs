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
            if (snapshot.Documents.Count > 0)
            {
                var doc = snapshot.Documents[0];
                Console.WriteLine($"User found: {doc.Id} - {doc.GetValue<string>("Email")}");

                // Fetched parameters for User base class
                string userID = doc.GetValue<string>("UserID");
                string role = doc.GetValue<string>("Role");
                string emailAddress = doc.GetValue<string>("Email");
                string password = doc.GetValue<string>("Password");
                string name = doc.GetValue<string>("Name");
                int mobileNumber = (int)doc.GetValue<long>("MobileNumber");
                string address = doc.GetValue<string>("Address");

                // Default to generic User if no matching role found
                return new User(userID, emailAddress, password, name, mobileNumber, address, role);
            }
            return null;
        }

        public async Task<User> FindUserByID(string userID)
        {
            // 🔹 Directly access Firestore document using UserID as key
            DocumentReference docRef = db.Collection("User").Document(userID);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Console.WriteLine($"User {userID} not found in Firestore.");
                return null;
            }

            // Fetch parameters for User base class
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

            // Default to generic User if no matching role found
            return new User(userID, emailAddress, password, name, mobileNumber, address, role);
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

                        // Create new User object
                        User user = new User(userID, email, password, name, (int)mobileNumber, address, role);
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

    }
}
