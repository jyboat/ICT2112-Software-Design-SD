using Google.Cloud.Firestore;
using System;

namespace ClearCare.Models
{
    [FirestoreData]
    public class Reply
    {
        [FirestoreDocumentId]
        public string FirestoreId { get; set; }
        
        [FirestoreProperty]
        public string EnquiryId { get; set; }
        
        [FirestoreProperty]
        public string Subject { get; set; }
        
        [FirestoreProperty]
        public string Message { get; set; }
        
        [FirestoreProperty]
        public string RecipientName { get; set; }
        
        [FirestoreProperty]
        public string RecipientEmail { get; set; }
        
        [FirestoreProperty]
        public string OriginalMessage { get; set; }
        
        [FirestoreProperty]
        public string UserUUID { get; set; }
        
        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }

         // New property for sender's name
        [FirestoreProperty]
        public string SenderName { get; set; }
    }
}
