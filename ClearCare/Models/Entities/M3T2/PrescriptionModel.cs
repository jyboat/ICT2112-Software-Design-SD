using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace ClearCare.Models.Entities.M3T2
{
    [FirestoreData]
    public class PrescriptionModel
    {
        [FirestoreProperty]
        public int PatientId { get; set; }

        [FirestoreProperty]
        public int DoctorId { get; set; }

        [FirestoreProperty]
        public List<string> DrugList { get; set; }

        [FirestoreProperty]
        public DateTime DateIssued { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public string DosageInfo { get; set; }
    }
}
