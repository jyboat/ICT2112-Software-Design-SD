using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace ClearCare.Models
{
    [FirestoreData]
    public class PrescriptionModel
    {
        [FirestoreProperty]
        public string PatientId { get; set; }

        [FirestoreProperty]
        public string DoctorId { get; set; }

        [FirestoreProperty]
        public List<string> DrugList { get; set; }

        [FirestoreProperty]
        public DateTime DateIssued { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public string DosageInfo { get; set; }


    }
}
