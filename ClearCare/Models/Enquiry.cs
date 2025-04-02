using Google.Cloud.Firestore;

namespace ClearCare.Models
{
    [FirestoreData]  // Tells Firestore this class is serializable
    public class Enquiry
    {
        [FirestoreDocumentId] // Maps the document ID to this property
        public string FirestoreId { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Name { get; set; }= string.Empty;

        [FirestoreProperty]
        public string Message { get; set; } = string.Empty;

        [FirestoreProperty]
        public string UserUUID { get; set; } = string.Empty;
        
        // New property for doctor UUID
        [FirestoreProperty]
        public string DoctorUUID { get; set; } = string.Empty;

        // New Topic property
        [FirestoreProperty]
        public string Topic { get; set; } = string.Empty; 
    }
}
