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

            // Adjust the entryDate to UTC+8 (local time zone)
            DateTime localEntryDate = entryDate.Kind == DateTimeKind.Utc 
                ? entryDate.AddHours(8)  // If the DateTime is already in UTC, add 8 hours
                : entryDate.ToUniversalTime().AddHours(8);  // Convert to UTC first, then add 8 hours

            // Convert the adjusted local DateTime to Firestore Timestamp
            EntryDate = Timestamp.FromDateTime(localEntryDate);
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
