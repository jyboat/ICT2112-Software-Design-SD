using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty]
        protected string UserID { get; set; }
        [FirestoreProperty]
        protected string Email { get; set; }
        [FirestoreProperty]
        protected string Password { get; set; }
        [FirestoreProperty]
        protected string Name { get; set; }
        [FirestoreProperty]
        protected int MobileNumber { get; set; }
        [FirestoreProperty]
        protected string Address { get; set; }
        [FirestoreProperty]
        protected string Role { get; set; }

        // Getter and Setters
        protected string GetUserID() => UserID;
        protected string GetEmail() => Email;
        protected string GetPassword() => Password;
        protected string GetName() => Name;
        protected int GetMobileNumber() => MobileNumber;
        protected string GetAddress() => Address;
        protected string GetRole() => Role;

        protected void SetUserID(string userID) => UserID = userID;
        protected void SetEmail(string email) => Email = email;
        protected void SetPassword(string password) => Password = password;
        protected void SetName(string name) => Name = name;
        protected void SetMobileNumber(int mobileNumber) => MobileNumber = mobileNumber;
        protected void SetAddress(string address) => Address = address;
        protected void SetRole(string role) => Role = role;

        public User(string userID, string email, string password, string name, int mobileNumber, string address, string role)
        {
            UserID = userID;
            Email = email;
            Password = password;
            Name = name;
            MobileNumber = mobileNumber;
            Address = address;
            Role = role;
        }

        public User() {}


        // Make a new user object
        public static User CreateUserObject(string userID, string email, string password, string name, int mobileNumber, string address, string role)
        {
            return new User(userID, email, password, name, mobileNumber, address, role);
        }

        // Validate Password during login
        public bool ValidatePassword(string inputPassword)
        {
            return GetPassword() == inputPassword;
        }

        // Provide only necessary session data in a structured format
        public (string userID, string role) GetSessionData()
        {
            return (GetUserID(), GetRole()); 
        }

        // Provide structured public access to user details
        public virtual Dictionary<string, object> GetPublicDetails()
        {
            return new Dictionary<string, object>
            {
                { "UserID", UserID },
                { "Email", Email },
                { "Name", Name },
                { "Role", Role },
                { "MobileNumber", MobileNumber },
                { "Address", Address }
            };
        }

    }
}