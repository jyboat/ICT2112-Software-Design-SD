using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities
{
    [FirestoreData]
    public class MedicalRecord
    {
        [FirestoreProperty]
        private string DoctorNote { get; set; }

        [FirestoreProperty]
        private Timestamp Date { get; set; }

        [FirestoreProperty]
        private string PatientID { get; set; }

        [FirestoreProperty]
        private string MedicalRecordID { get; set; }

        // Stores the file as a byte array, easier retrieval
        [FirestoreProperty]
        private byte[] Attachment { get; set; } 

        // store the name of the file
        [FirestoreProperty]
        private string AttachmentName { get; set; }

        [FirestoreProperty]
        private string UserID { get; set; }


        // Getter and Setters
        private string getDoctorNote() => DoctorNote;
        private Timestamp getDate() => Date;
        private string getPatientID() => PatientID;
        private string getUserID() => UserID;
        private string getMedicalRecordID() => MedicalRecordID;
        private byte[] getAttachment() => Attachment;
        private string getAttachmentName() => AttachmentName;

        private void setDoctorNote(string doctorNote) => DoctorNote = doctorNote;
        private void setDate(Timestamp date) => Date = date;
        private void setPatientID(string patientID) => PatientID = patientID;
        private void setUserID(string userID) => UserID = userID;
        private void setAttachment(byte[] attachment) => Attachment = attachment;
        private void setAttachmentName(string attachmentName) => AttachmentName = attachmentName;
        private void setMedicalRecordID(string medicalrecordID) => MedicalRecordID = medicalrecordID;

        public MedicalRecord() {}

        // Object Creation
        public MedicalRecord(string doctorNote, Timestamp date, string patientID, string medicalrecordID, byte[] attachment, string attachmentName, string userID)
        {
            DoctorNote = doctorNote;
            Date = date;
            PatientID = patientID;
            MedicalRecordID = medicalrecordID;
            Attachment = attachment;
            AttachmentName = attachmentName;
            UserID = userID;
        }

        // Method to check if an attachment exists
        public bool HasAttachment() => getAttachment() != null && getAttachment().Length > 0;

        // Method to retrieve attachment safely
        public (byte[], string) RetrieveAttachment()
        {
            if (!HasAttachment()) return (null, null);
            return (getAttachment(), getAttachmentName());
        }

        // Exposed method to return all necessary details
        public Dictionary<string, object> GetRecordDetails()
        {
            return new Dictionary<string, object>
            {
                { "MedicalRecordID", MedicalRecordID },
                { "PatientID", PatientID },
                { "CreatedByUserID", UserID },
                { "Date", Date },
                { "DoctorNote", DoctorNote },
                { "AttachmentName", AttachmentName },
                { "HasAttachment", Attachment != null && Attachment.Length > 0 }
            };
        }

    }
}