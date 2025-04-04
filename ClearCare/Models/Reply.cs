using Google.Cloud.Firestore;
using System;

namespace ClearCare.Models
{
    [FirestoreData]
    public class Reply
    {
        /// <summary>
        ///   Gets or sets the Firestore document ID for the reply.
        /// </summary>
        [FirestoreDocumentId]
        public string FirestoreId { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the ID of the enquiry that this reply is associated
        ///   with.
        /// </summary>
        [FirestoreProperty]
        public string EnquiryId { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the subject of the reply.
        /// </summary>
        [FirestoreProperty]
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the message content of the reply.
        /// </summary>
        [FirestoreProperty]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the name of the recipient of the reply.
        /// </summary>
        [FirestoreProperty]
        public string RecipientName { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the email address of the recipient of the reply.
        /// </summary>
        [FirestoreProperty]
        public string RecipientEmail { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the original message that this reply is in response
        ///   to.
        /// </summary>
        [FirestoreProperty]
        public string OriginalMessage { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the UUID of the user who created the reply.
        /// </summary>
        [FirestoreProperty]
        public string UserUUID { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the timestamp indicating when the reply was created.
        /// </summary>
        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        ///   Gets or sets the name of the sender of the reply.
        /// </summary>
        // New property for sender's name
        [FirestoreProperty]
        public string SenderName { get; set; } = string.Empty;
    }
}
