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


        // Getter and Setters
        public string getDoctorNote() => DoctorNote;
        public Timestamp getDate() => Date;
        public string getPatientID() => PatientID;

        public void setDoctorNote(string doctorNote) => DoctorNote = doctorNote;
        public void setDate(Timestamp date) => Date = date;
        public void setPatientID(string patientID) => PatientID = patientID;

        

        public MedicalRecord(string doctorNote, Timestamp date, string patientID)
        {
            DoctorNote = doctorNote;
            Date = date;
            PatientID = patientID;
        }

        public MedicalRecord() {}

    }
}