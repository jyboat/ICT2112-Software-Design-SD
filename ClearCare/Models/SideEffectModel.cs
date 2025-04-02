using Google.Cloud.Firestore;

namespace ClearCare.Models
{
    [FirestoreData]
    public class SideEffectModel
    {
        /// <summary>
        ///   Gets or sets the ID of the patient who experienced the side
        ///   effect.
        /// </summary>
        [FirestoreProperty]
        public string PatientId { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the name of the drug associated with the side
        ///   effect.
        /// </summary>
        [FirestoreProperty]
        public string DrugName { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets additional information about the drug.
        /// </summary>
        [FirestoreProperty]
        public string DrugInformation { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the description of the side effects experienced.
        /// </summary>
        [FirestoreProperty]
        public string SideEffects { get; set; } = string.Empty;
    }
}
