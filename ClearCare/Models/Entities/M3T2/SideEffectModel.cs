using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities.M3T2
{
    [FirestoreData]
    public class SideEffectModel
    {
        [FirestoreProperty]
        public string PatientId { get; set; } // Updated to string to match Firebase

        [FirestoreProperty]
        public string DrugName { get; set; }

        [FirestoreProperty]
        public string DrugInformation { get; set; }

        [FirestoreProperty]
        public string SideEffects { get; set; }
    }
}