using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities
{
    [FirestoreData]
    public class MedicalRecord
    {
        // MedicalRecordID is assigned from Firestore document ID
        private string MedicalRecordID { get; set; }

        [FirestoreProperty]
        private string DoctorNote { get; set; }

        [FirestoreProperty]
        private Timestamp Date { get; set; }

        [FirestoreProperty]
        private string PatientID { get; set; }

        // Stores the file as a byte array, easier retrieval
        [FirestoreProperty]
        private byte[] Attachment { get; set; } 

        // store the name of the file
        [FirestoreProperty]
        private string AttachmentName { get; set; }

        [FirestoreProperty]
        private string DoctorID { get; set; }


        // Getter and Setters
        private string getDoctorNote() => DoctorNote;
        private Timestamp getDate() => Date;
        private string getPatientID() => PatientID;
        private string getDoctorID() => DoctorID;
        private string getMedicalRecordID() => MedicalRecordID;
        private byte[] getAttachment() => Attachment;
        private string getAttachmentName() => AttachmentName;

        private void setDoctorNote(string doctorNote) => DoctorNote = doctorNote;
        private void setDate(Timestamp date) => Date = date;
        private void setPatientID(string patientID) => PatientID = patientID;
        private void setDoctorID(string doctorID) => DoctorID = doctorID;
        private void setAttachment(byte[] attachment) => Attachment = attachment;
        private void setAttachmentName(string attachmentName) => AttachmentName = attachmentName;
        private void setMedicalRecordID(string medicalrecordID) => MedicalRecordID = medicalrecordID;

        public MedicalRecord() {}

        // Constructor for creating new medical records
        public MedicalRecord(string medicalrecordID, string doctorNote, Timestamp date, string patientID, byte[] attachment, string attachmentName, string doctorID)
        {
            MedicalRecordID = medicalrecordID;
            DoctorNote = doctorNote;
            Date = date;
            PatientID = patientID;
            Attachment = attachment;
            AttachmentName = attachmentName;
            DoctorID = doctorID;
        }

        // Method to check if an attachment exists
        public bool hasAttachment() => getAttachment() != null && getAttachment().Length > 0;

        // Method to retrieve attachment safely
        public (byte[], string) retrieveAttachment()
        {
            return hasAttachment() ? (getAttachment(), getAttachmentName()) : (null, null);
        }

        // Exposed method to return all necessary details
        public Dictionary<string, object> getRecordDetails()
        { 
            return new Dictionary<string, object>
            {
                { "MedicalRecordID", MedicalRecordID },
                { "PatientID", PatientID },
                { "DoctorID", DoctorID },
                { "Date", Date },
                { "DoctorNote", DoctorNote },
                { "AttachmentName", AttachmentName },
                { "HasAttachment", hasAttachment() }
            };
        }

    }
}