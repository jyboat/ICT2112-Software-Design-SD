using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities
{
    [FirestoreData]
    public abstract class User
    {
        // Class properties
        // UserID is assigned from Firestore document ID
        protected string UserID { get; set; } 
        [FirestoreProperty]
        protected string Email { get; set; }
        [FirestoreProperty]
        protected string Password { get; set; }
        [FirestoreProperty]
        protected string Name { get; set; }
        [FirestoreProperty]
        protected long MobileNumber { get; set; }
        [FirestoreProperty]
        protected string Address { get; set; }
        [FirestoreProperty]
        protected string Role { get; set; }

        // Getter and Setter
        protected string getUserID() => UserID;
        protected string getEmail() => Email;
        protected string getPassword() => Password;
        protected string getName() => Name;
        protected long getMobileNumber() => MobileNumber;
        protected string getAddress() => Address;
        protected string getRole() => Role;

        protected void setUserID(string userID) => UserID = userID;
        protected void setEmail(string email) => Email = email;
        protected void setPassword(string password) => Password = password;
        protected void setName(string name) => Name = name;
        protected void setMobileNumber(long mobileNumber) => MobileNumber = mobileNumber;
        protected void setAddress(string address) => Address = address;
        protected void setRole(string role) => Role = role;

        public User() {}

        // Constructor for creating new user records
        public User(string userID, string email, string password, string name, long mobileNumber, string address, string role)
        {
            UserID = userID;
            Email = email;
            Password = password;
            Name = name;
            MobileNumber = mobileNumber;
            Address = address;
            Role = role;
        }

        // Returns hashed password to compare when User logs in
        public string getHashedPassword()
        {
            return getPassword(); 
        }


        // Returns userID and Role to store in Session upon User login
        public (string userID, string role) getSessionData()
        {
            return (getUserID(), getRole()); 
        }

        // Returns all user data to be used in Profile
        public virtual Dictionary<string, object> getProfileData()
        {
            return new Dictionary<string, object>
            {
                { "UserID", getUserID() },
                { "Email", getEmail() },
                { "Name", getName() },
                { "MobileNumber", getMobileNumber() },
                { "Address", getAddress() },
                { "Role", getRole() }
            };
        }

        // Setter for profile data
        public void SetProfileData(Dictionary<string, object> profileData)
        {
            if (profileData == null)
            {
                throw new ArgumentNullException(nameof(profileData), "Profile data cannot be null.");
            }

            // Use null-conditional and null-coalescing operators to handle potential null values
            if (profileData.ContainsKey("UserID")) SetUserID(profileData["UserID"]?.ToString() ?? string.Empty);
            if (profileData.ContainsKey("Email")) SetEmail(profileData["Email"]?.ToString() ?? string.Empty);
            if (profileData.ContainsKey("Password")) SetPassword(profileData["Password"]?.ToString() ?? string.Empty);
            if (profileData.ContainsKey("Name")) SetName(profileData["Name"]?.ToString() ?? string.Empty);
            if (profileData.ContainsKey("MobileNumber")) SetMobileNumber(profileData["MobileNumber"] as long? ?? 0);
            if (profileData.ContainsKey("Address")) SetAddress(profileData["Address"]?.ToString() ?? string.Empty);
            if (profileData.ContainsKey("Role")) SetRole(profileData["Role"]?.ToString() ?? string.Empty);
        }
    }
}