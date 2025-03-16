using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities.M3T2
{
    [FirestoreData]  // Tells Firestore this class is serializable
    public class Enquiry
    {
        [FirestoreDocumentId] // This attribute tells Firestore to map the document ID to this property
        public string FirestoreId { get; set; }

        // [FirestoreProperty]
        // public string Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Email { get; set; }

        [FirestoreProperty]
        public string Message { get; set; }

        [FirestoreProperty]
        public string UserUUID { get; set; }
    }
}
