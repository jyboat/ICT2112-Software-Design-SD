using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities
{
    [FirestoreData]
    public class Erratum
    {
        private string ErratumID { get; set; } = string.Empty;

        [FirestoreProperty]
        private string MedicalRecordID { get; set; } = string.Empty;

        [FirestoreProperty]
        private Timestamp Date { get; set; }

        [FirestoreProperty]
        private string ErratumDetails { get; set; } = string.Empty;

        [FirestoreProperty]
        private string DoctorID { get; set; } = string.Empty;

        // Getter and Setters
        private string getErratumID() => ErratumID;
        private string getMedicalRecordID() => MedicalRecordID;
        private Timestamp getDate() => Date;
        private string getErratumDetails() => ErratumDetails;
        private string getDoctorID() => DoctorID;

        private void setErratumID(string erratumID) => ErratumID = erratumID;
        private void setMedicalRecordID(string medicalRecordID) => MedicalRecordID = medicalRecordID;
        private void setDate(Timestamp date) => Date = date;
        private void setErratumDetails(string erratumDetails) => ErratumDetails = erratumDetails;
        private void setDoctorID(string doctorID) => DoctorID = doctorID;

        public Erratum() {}

        // Object Creation
        public Erratum(string erratumID, string medicalRecordID, Timestamp date, string erratumDetails, string doctorID)
        {
            ErratumID = erratumID;
            MedicalRecordID = medicalRecordID;
            Date = date;
            ErratumDetails = erratumDetails;
            DoctorID = doctorID;
        }

        // Method to get Erratum details
        public Dictionary<string, object> getErratumData()
        {
            var details = new Dictionary<string, object>
            {
                { "ErratumID", ErratumID },
                { "MedicalRecordID", MedicalRecordID },
                { "Date", Date },
                { "ErratumDetails", ErratumDetails },
                { "DoctorID", DoctorID }
            };
            return details;
        }
    }
}