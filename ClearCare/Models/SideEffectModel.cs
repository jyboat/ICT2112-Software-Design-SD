using Google.Cloud.Firestore;

namespace ClearCare.Models
{
    [FirestoreData]
    public class SideEffectModel
    {
        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Description { get; set; }

        [FirestoreProperty]
        public string Date { get; set; } // Ensure this matches the Firestore field name
    }
}
