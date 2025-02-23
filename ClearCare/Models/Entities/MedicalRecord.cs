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


        // Getter and Setters
        public string getDoctorNote() => DoctorNote;
        public Timestamp getDate() => Date;
        public string getPatientID() => PatientID;

        public string getMedicalRecordID() => MedicalRecordID;

        public void setDoctorNote(string doctorNote) => DoctorNote = doctorNote;
        public void setDate(Timestamp date) => Date = date;
        public void setPatientID(string patientID) => PatientID = patientID;

        public void setMedicalRecordID(string medicalrecordID) => MedicalRecordID = medicalrecordID;

        
        // Object Creation
        public MedicalRecord(string doctorNote, Timestamp date, string patientID, string medicalrecordID)
        {
            DoctorNote = doctorNote;
            Date = date;
            PatientID = patientID;
            MedicalRecordID = medicalrecordID;
        }

    }
}