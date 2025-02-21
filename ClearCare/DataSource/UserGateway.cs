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

            Console.WriteLine($"User found: {snapshot.Id} - {snapshot.GetValue<string>("Email")}");

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

    }
}
