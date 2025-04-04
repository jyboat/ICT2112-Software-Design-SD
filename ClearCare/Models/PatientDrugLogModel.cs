using Google.Cloud.Firestore;

namespace ClearCare.Models
{
    [FirestoreData]
    public class PatientDrugLogModel
    {
        /// <summary>
        ///   Gets or sets the ID of the patient associated with the drug log
        ///   entry.
        /// </summary>
        [FirestoreProperty]
        public string PatientId { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the name of the drug.
        /// </summary>
        [FirestoreProperty]
        public string DrugName { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets any additional information about the drug.
        /// </summary>
        [FirestoreProperty]
        public string DrugInformation { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the side effects associated with the drug.
        /// </summary>
        [FirestoreProperty]
        public string SideEffects { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the status of the drug log entry.
        /// </summary>
        [FirestoreProperty]
        public string Status { get; set; } = string.Empty;
    }
}
