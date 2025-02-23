using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities
{
    [FirestoreData]
    public class Erratum
    {
        [FirestoreProperty]
        private string ErratumID { get; set; }

        [FirestoreProperty]
        private string MedicalRecordID { get; set; }

        [FirestoreProperty]
        private Timestamp Date { get; set; }

        [FirestoreProperty]
        private string ErratumDetails { get; set; }

        // Getter and Setters
        private string getErratumID() => ErratumID;
        private string getMedicalRecordID() => MedicalRecordID;
        private Timestamp getDate() => Date;
        private string getErratumDetails() => ErratumDetails;

        private void setErratumID(string erratumID) => ErratumID = erratumID;
        private void setMedicalRecordID(string medicalRecordID) => MedicalRecordID = medicalRecordID;
        private void setDate(Timestamp date) => Date = date;
        private void setErratumDetails(string erratumDetails) => ErratumDetails = erratumDetails;

        public Erratum() {}

        // Object Creation
        public Erratum(string erratumID, string medicalRecordID, Timestamp date, string erratumDetails)
        {
            ErratumID = erratumID;
            MedicalRecordID = medicalRecordID;
            Date = date;
            ErratumDetails = erratumDetails;
        }
    }
}