using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace ClearCare.Models
{
    // Simple class to hold one drug+dosage pair
    [FirestoreData] 
    public class DrugDosage
    {
        /// <summary>
        ///   Gets or sets the name of the drug.
        /// </summary>
        [FirestoreProperty]
        public string DrugName { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the dosage information for the drug.
        /// </summary>
        [FirestoreProperty]
        public string Dosage { get; set; } = string.Empty;
    }

    [FirestoreData]
    public class PrescriptionModel
    {
        /// <summary>
        ///   Gets or sets the ID of the patient associated with the
        ///   prescription.
        /// </summary>
        [FirestoreProperty]
        public string PatientId { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the ID of the doctor who issued the prescription.
        /// </summary>
        [FirestoreProperty]
        public string DoctorId { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the list of medications and their dosages in the
        ///   prescription.
        /// </summary>
        // Replaces the old List<string> DrugList and single DosageInfo
        [FirestoreProperty]
        public List<DrugDosage> Medications { get; set; } = new List<DrugDosage>();

        /// <summary>
        ///   Gets or sets the date the prescription was issued.
        /// </summary>
        [FirestoreProperty]
        public DateTime DateIssued { get; set; } = DateTime.UtcNow;
    }
}
