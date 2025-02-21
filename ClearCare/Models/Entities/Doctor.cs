using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities
{
    public class Doctor : User
    {
        [FirestoreProperty]
        protected string Specialization { get; set; }


        protected string GetSpecialization() => Specialization;
        protected void SetSpecialization(string specialization) => Specialization = specialization;

        public Doctor(string userID, string email, string password, string name, int mobileNumber, string address, string role, string specialization)
        {
            UserID = userID;
            Email = email;
            Password = password;
            Name = name;
            MobileNumber = mobileNumber;
            Address = address;
            Role = role;
            Specialization = specialization;
        }

        public Doctor() {}

        // 🔹 Override GetPublicDetails() to include Doctor-specific fields
        public override Dictionary<string, object> GetPublicDetails()
        {
            var details = base.GetPublicDetails();
            details.Add("Specialization", Specialization);
            return details;
        }

    }
}