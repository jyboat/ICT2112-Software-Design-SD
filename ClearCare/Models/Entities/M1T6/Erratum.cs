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

        // Stores the file as a byte array, easier retrieval
        [FirestoreProperty]
        private byte[] ErratumAttachment { get; set; } 

        // store the name of the file
        [FirestoreProperty]
        private string ErratumAttachmentName { get; set; }

        // Getter and Setters
        private string getErratumID() => ErratumID;
        private string getMedicalRecordID() => MedicalRecordID;
        private Timestamp getDate() => Date;
        private string getErratumDetails() => ErratumDetails;
        private string getDoctorID() => DoctorID;
        private byte[] getErratumAttachment() => ErratumAttachment;
        private string getErratumAttachmentName() => ErratumAttachmentName;

        private void setErratumID(string erratumID) => ErratumID = erratumID;
        private void setMedicalRecordID(string medicalRecordID) => MedicalRecordID = medicalRecordID;
        private void setDate(Timestamp date) => Date = date;
        private void setErratumDetails(string erratumDetails) => ErratumDetails = erratumDetails;
        private void setDoctorID(string doctorID) => DoctorID = doctorID;
        private void setErratumAttachment(byte[] attachment) => ErratumAttachment = attachment;
        private void setErratumAttachmentName(string attachmentName) => ErratumAttachmentName = attachmentName;

        public Erratum()
        {
            ErratumAttachment = Array.Empty<byte>();
            ErratumAttachmentName = string.Empty;
        }

        // Object Creation
        public Erratum(string erratumID, string medicalRecordID, Timestamp date, string erratumDetails, string doctorID, byte[] attachment, string attachmentName)
        {
            ErratumID = erratumID;
            MedicalRecordID = medicalRecordID;
            Date = date;
            ErratumDetails = erratumDetails;
            DoctorID = doctorID;
            ErratumAttachment = attachment;
            ErratumAttachmentName = attachmentName;
        }

        // Method to check if an attachment exists
        public bool hasErratumAttachment() => getErratumAttachment() != null && getErratumAttachment().Length > 0;

        // Method to retrieve attachment safely
        public (byte[]?, string?) retrieveErratumAttachment()
        {
            return hasErratumAttachment() ? (getErratumAttachment(), getErratumAttachmentName()) : (null, null);
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
                { "DoctorID", DoctorID },
                { "ErratumAttachmentName", ErratumAttachmentName },
                { "HasErratumAttachment", hasErratumAttachment() }
            };
            return details;
        }
    }
}