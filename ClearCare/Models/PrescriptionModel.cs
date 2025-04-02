using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace ClearCare.Models
{
    // Simple class to hold one drug+dosage pair
    [FirestoreData]  // <-- Annotate the nested class
    public class DrugDosage
    {
        [FirestoreProperty]
        public string DrugName { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string Dosage { get; set; } = string.Empty;
    }

    [FirestoreData]
    public class PrescriptionModel
    {
        [FirestoreProperty]
        public string PatientId { get; set; } = string.Empty;

        [FirestoreProperty]
        public string DoctorId { get; set; } = string.Empty;

        // Replaces the old List<string> DrugList and single DosageInfo
        [FirestoreProperty]
        public List<DrugDosage> Medications { get; set; } = new List<DrugDosage>();

        [FirestoreProperty]
        public DateTime DateIssued { get; set; } = DateTime.UtcNow;
    }
}
