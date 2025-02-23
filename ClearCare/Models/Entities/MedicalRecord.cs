using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities
{
    [FirestoreData]
    public class MedicalRecord
    {
        [FirestoreProperty]
        public string DoctorNote { get; set; }

        [FirestoreProperty]
        public Timestamp Date { get; set; }

        [FirestoreProperty]
        public string PatientID { get; set; }

        [FirestoreProperty]
        public string MedicalRecordID { get; set; }

        [FirestoreProperty]
        public byte[] Attachment { get; set; } // Stores the file as a byte array, easier retrieval

        [FirestoreProperty]
        public string AttachmentName { get; set; } // store the name of the file


        // Getter and Setters
        public string getDoctorNote() => DoctorNote;
        public Timestamp getDate() => Date;
        public string getPatientID() => PatientID;

        public string getMedicalRecordID() => MedicalRecordID;
        public byte[] getAttachment() => Attachment;
        public string getAttachmentName() => AttachmentName;

        public void setDoctorNote(string doctorNote) => DoctorNote = doctorNote;
        public void setDate(Timestamp date) => Date = date;
        public void setPatientID(string patientID) => PatientID = patientID;
        public void setAttachment(byte[] attachment) => Attachment = attachment;
        public void setAttachmentName(string attachmentName) => AttachmentName = attachmentName;

        public void setMedicalRecordID(string medicalrecordID) => MedicalRecordID = medicalrecordID;

        public MedicalRecord() {}

        // Object Creation
        public MedicalRecord(string doctorNote, Timestamp date, string patientID, string medicalrecordID, byte[] attachment, string attachmentName)
        {
            DoctorNote = doctorNote;
            Date = date;
            PatientID = patientID;
            MedicalRecordID = medicalrecordID;
            Attachment = attachment;
            AttachmentName = attachmentName;
        }

    }
}