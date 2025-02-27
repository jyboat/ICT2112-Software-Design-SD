using System;
using System.IO;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;

namespace ClearCare.DataSource
{
    public class UserGateway
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
            return snapshot.ContainsField("Name") ? snapshot.GetValue<string>("Name") : "Unknown User";
        }

        // Method to insert user into Firestore with encrypted and hashed data
        public async Task InsertUser(User user)
        {
            // Get the user's profile data as a dictionary
            Dictionary<string, object> profileData = user.GetProfileData();

            // Safely get the password
            profileData.TryGetValue("Password", out var passwordObj);
            string password = passwordObj?.ToString() ?? string.Empty;
            string hashedPassword = encryptionManagement.HashPassword(password);

            // Safely get other fields
            profileData.TryGetValue("Email", out var emailObj);
            string email = emailObj?.ToString() ?? string.Empty;

            profileData.TryGetValue("Name", out var nameObj);
            string name = nameObj?.ToString() ?? string.Empty;

            profileData.TryGetValue("Address", out var addressObj);
            string address = addressObj?.ToString() ?? string.Empty;

            // Construct the new user dictionary
            var newUser = new Dictionary<string, object>
            {
                { "UserID", profileData["UserID"] },
                { "Email", email },
                { "Password", hashedPassword },
                { "Name", name },
                { "MobileNumber", profileData["MobileNumber"] },
                { "Address", address },
                { "Role", profileData["Role"] }
            };

            // Add the new user to Firestore
            DocumentReference docRef = await db.Collection("User").AddAsync(newUser);
            Console.WriteLine($"User added with ID: {docRef.Id}");
        }

        // Method to get the next available user ID
        public async Task<string> GetNextUserId()
        {
            Query query = db.Collection("User").OrderByDescending("UserID").Limit(1);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            if (snapshot.Count == 0)
            {
                return "USR001";  // Start from here if no users are found
            }

            var lastUser = snapshot.Documents[0];
            string lastUserId = lastUser.GetValue<string>("UserID");

            int numericId = int.Parse(lastUserId.Substring(3)); // Assumes "USR" prefix and numeric suffix
            numericId++; // Increment to get the next user ID

            return $"USR{numericId:D3}"; // Pad with zeros to maintain the format
        }
    }
}
