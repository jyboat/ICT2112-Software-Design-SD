using Google.Cloud.Firestore;

namespace ClearCare.Models
{
    [FirestoreData]  // Tells Firestore this class is serializable
    public class Enquiry
    {

        [FirestoreDocumentId] // Optional — if you want Firestore to fill in the doc ID
        public string FirestoreId { get; set; }

        [FirestoreProperty]
        public int Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Email { get; set; }

        [FirestoreProperty]
        public string Message { get; set; }
    }
}
