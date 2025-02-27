using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities
{
    [FirestoreData]
    public class User
    {
        // Class properties
        [FirestoreProperty]
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
        protected string GetUserID() => UserID;
        protected string GetEmail() => Email;
        protected string GetPassword() => Password;
        protected string GetName() => Name;
        protected long GetMobileNumber() => MobileNumber;
        protected string GetAddress() => Address;
        protected string GetRole() => Role;

        protected void SetUserID(string userID) => UserID = userID;
        protected void SetEmail(string email) => Email = email;
        protected void SetPassword(string password) => Password = password;
        protected void SetName(string name) => Name = name;
        protected void SetMobileNumber(long mobileNumber) => MobileNumber = mobileNumber;
        protected void SetAddress(string address) => Address = address;
        protected void SetRole(string role) => Role = role;

        public User() {}

        // Object Creation
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
        public string GetHashedPassword()
        {
            return GetPassword(); 
        }


        // Returns userID and Role to store in Session upon User login
        public (string userID, string role) GetSessionData()
        {
            return (GetUserID(), GetRole()); 
        }

        // Returns all user data to be used in Profile
        public virtual Dictionary<string, object> GetProfileData()
        {
            return new Dictionary<string, object>
            {
                { "UserID", GetUserID() },
                { "Email", GetEmail() },
                { "Name", GetName() },
                { "MobileNumber", GetMobileNumber() },
                { "Address", GetAddress() },
                { "Role", GetRole() }
            };
        }

    }
}