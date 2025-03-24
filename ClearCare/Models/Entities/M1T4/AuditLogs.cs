using Google.Cloud.Firestore;
using System;

namespace ClearCare.Models.Entities
{
    [FirestoreData]
    public class AuditLog
    {
        [FirestoreDocumentId]
        public string AuditLogID { get; set; }  // Firestore document ID

        [FirestoreProperty]
        public string Action { get; private set; }   // Action performed

        [FirestoreProperty]
        public string PerformedBy { get; private set; }   // User who performed action

        [FirestoreProperty]
        public Timestamp EntryDate { get; private set; }  // Time of action

        public AuditLog() { }

        public AuditLog(string action, string performedBy, DateTime entryDate)
        {
            Action = action;
            PerformedBy = performedBy;
            EntryDate = Timestamp.FromDateTime(entryDate);
        }

        public Dictionary<string, object> GetAuditDetails()
        {
            return new Dictionary<string, object>
            {
                { "AuditLogID", AuditLogID },
                { "Action", Action },
                { "PerformedBy", PerformedBy },
                { "EntryDate", EntryDate.ToDateTime() }
            };
        }
    }
}
