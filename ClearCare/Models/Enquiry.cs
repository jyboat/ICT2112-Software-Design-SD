using Google.Cloud.Firestore;

namespace ClearCare.Models
{
    [FirestoreData]  // Tells Firestore this class is serializable
    public class Enquiry
    {
        [FirestoreDocumentId] // Maps the document ID to this property
        public string FirestoreId { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Message { get; set; }

        [FirestoreProperty]
        public string UserUUID { get; set; }
        
        // New property for doctor UUID
        [FirestoreProperty]
        public string DoctorUUID { get; set; }
    }
}
