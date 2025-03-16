using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities
{
    [FirestoreData]
    public class Nurse : User
    {
        // Class properties
        [FirestoreProperty]
        protected string Department { get; set; }

        // Getter & Setter
        protected string getDepartment() => Department;
        protected void setDepartment(string department) => Department = department;

        public Nurse() {}

        // Object Creation
        public Nurse(string userID, string email, string password, string name, long mobileNumber, string address, string role, string department)
            : base(userID, email, password, name, mobileNumber, address, role)  // Call base constructor
        {
            Department = department;  // Nurse-specific field
        }

        // Override GetProfileDetails() to include Nurse-specific fields
        public override Dictionary<string, object> getProfileData()
        {
            var details = base.getProfileData();
            details.Add("Department", getDepartment());
            return details;
        }

    }
}