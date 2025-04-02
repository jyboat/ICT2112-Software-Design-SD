using Google.Cloud.Firestore;
using System;

namespace ClearCare.Models
{
    [FirestoreData]
    public class Reply
    {
        [FirestoreDocumentId]
        public string FirestoreId { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string EnquiryId { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string Subject { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string Message { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string RecipientName { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string RecipientEmail { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string OriginalMessage { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string UserUUID { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

         // New property for sender's name
        [FirestoreProperty]
        public string SenderName { get; set; } = string.Empty;
    }
}
