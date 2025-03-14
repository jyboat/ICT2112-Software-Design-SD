using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities
{
    [FirestoreData]
    public class Doctor : User
    {
        // Class properties
        [FirestoreProperty]
        protected string Specialization { get; set; }

        // Getter & Setter
        protected string getSpecialization() => Specialization;
        protected void setSpecialization(string specialization) => Specialization = specialization;

        public Doctor() {}

        // Object Creation
        public Doctor(string userID, string email, string password, string name, long mobileNumber, string address, string role, string specialization)
            : base(userID, email, password, name, mobileNumber, address, role)  // Call base constructor
        {
            Specialization = specialization;  // Doctor-specific field
        }

        // Override GetProfileDetails() to include Doctor-specific fields
        public override Dictionary<string, object> getProfileData()
        {
            var details = base.getProfileData();
            details.Add("Specialization", getSpecialization());
            return details;
        }

    }
}