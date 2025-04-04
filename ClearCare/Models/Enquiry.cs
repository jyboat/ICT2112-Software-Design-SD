using Google.Cloud.Firestore;

namespace ClearCare.Models
{
    [FirestoreData] // Tells Firestore this class is serializable
    public class Enquiry
    {
        /// <summary>
        ///   Gets or sets the Firestore document ID for the enquiry.
        /// </summary>
        [FirestoreDocumentId] 
        public string FirestoreId { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the name of the person making the enquiry.
        /// </summary>
        [FirestoreProperty]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the message content of the enquiry.
        /// </summary>
        [FirestoreProperty]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the UUID of the user making the enquiry.
        /// </summary>
        [FirestoreProperty]
        public string UserUUID { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the UUID of the doctor associated with the enquiry.
        /// </summary>
        // New property for doctor UUID
        [FirestoreProperty]
        public string DoctorUUID { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the topic of the enquiry.
        /// </summary>
        // New Topic property
        [FirestoreProperty]
        public string Topic { get; set; } = string.Empty;
    }
}
