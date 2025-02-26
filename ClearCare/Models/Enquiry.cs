using Google.Cloud.Firestore;

namespace ClearCare.Models
{
    [FirestoreData]  // Tells Firestore this class is serializable
    public class Enquiry
    {
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
