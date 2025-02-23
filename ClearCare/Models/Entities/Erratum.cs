using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities
{
    [FirestoreData]
    public class Erratum
    {
        [FirestoreProperty]
        private string ErratumID { get; set; } = string.Empty;

        [FirestoreProperty]
        private string MedicalRecordID { get; set; } = string.Empty;

        [FirestoreProperty]
        private Timestamp Date { get; set; }

        [FirestoreProperty]
        private string ErratumDetails { get; set; } = string.Empty;

        [FirestoreProperty]
        private string UserID { get; set; } = string.Empty;

        // Getter and Setters
        private string getErratumID() => ErratumID;
        private string getMedicalRecordID() => MedicalRecordID;
        private Timestamp getDate() => Date;
        private string getErratumDetails() => ErratumDetails;
        private string getUserID() => UserID;

        private void setErratumID(string erratumID) => ErratumID = erratumID;
        private void setMedicalRecordID(string medicalRecordID) => MedicalRecordID = medicalRecordID;
        private void setDate(Timestamp date) => Date = date;
        private void setErratumDetails(string erratumDetails) => ErratumDetails = erratumDetails;
        private void setUserID(string userID) => UserID = userID;

        public Erratum() {}

        // Object Creation
        public Erratum(string erratumID, string medicalRecordID, Timestamp date, string erratumDetails, string userID)
        {
            ErratumID = erratumID;
            MedicalRecordID = medicalRecordID;
            Date = date;
            ErratumDetails = erratumDetails;
            UserID = userID;
        }

        // Method to get Erratum details
        public Dictionary<string, object> GetErratumDetails()
        {
            var details = new Dictionary<string, object>
            {
                { "ErratumID", ErratumID },
                { "MedicalRecordID", MedicalRecordID },
                { "Date", Date },
                { "ErratumDetails", ErratumDetails },
                { "CreatedByUserID", UserID }
            };
            return details;
        }
    }
}