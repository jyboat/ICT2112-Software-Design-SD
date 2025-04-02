using Google.Cloud.Firestore;

namespace ClearCare.Models
{
    [FirestoreData]
    public class SideEffectModel
    {
        [FirestoreProperty]
        public string PatientId { get; set; } = string.Empty;

        [FirestoreProperty]
        public string DrugName { get; set; } = string.Empty;

        [FirestoreProperty]
        public string DrugInformation { get; set; }= string.Empty;

        [FirestoreProperty]
        public string SideEffects { get; set; } = string.Empty;
    }
}