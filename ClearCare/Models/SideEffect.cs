using Google.Cloud.Firestore;

namespace ClearCare.Models
{
    [FirestoreData]
    public class SideEffect
    {
        // Stores the Firestore doc ID
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Description { get; set; }

    }
}
