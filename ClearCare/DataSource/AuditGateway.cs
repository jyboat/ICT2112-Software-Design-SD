using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseAdmin;
using Google.Cloud.Firestore;
using ClearCare.Models.Entities;

namespace ClearCare.DataSource
{
    public class AuditGateway
    {
        private FirestoreDb db;

        public AuditGateway()
        {
            // Initialize Firebase Firestore database using FirebaseAdmin
            db = FirebaseService.Initialize();
        }

        // Retrieve all audit logs
        public async Task<List<AuditLog>> retrieveAllAuditLogs()
        {
            List<AuditLog> auditLogs = new List<AuditLog>();
            QuerySnapshot snapshot = await db.Collection("AuditLogs").OrderByDescending("EntryDate").GetSnapshotAsync();

            if (snapshot.Documents.Count == 0)
            {
                Console.WriteLine("No audit logs found in Firestore.");
                return auditLogs;
            }

            foreach (var doc in snapshot.Documents)
            {
                // Retrieve data from Firestore document
                string auditLogID = doc.Id;
                string action = doc.GetValue<string>("Action");
                string performedBy = doc.GetValue<string>("PerformedBy");
                Timestamp entryDate = doc.GetValue<Timestamp>("EntryDate");

                // Create an AuditLog object
                AuditLog log = new AuditLog(action, performedBy, entryDate.ToDateTime())
                {
                    AuditLogID = auditLogID
                };

                auditLogs.Add(log);
            }

            return auditLogs;
        }

        // Retrieve an audit log by ID
        public async Task<AuditLog> retrieveAuditLogById(string auditLogID)
        {
            DocumentReference docRef = db.Collection("AuditLogs").Document(auditLogID);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Console.WriteLine($"Audit log {auditLogID} not found in Firestore.");
                return null;
            }

            string action = snapshot.GetValue<string>("Action");
            string performedBy = snapshot.GetValue<string>("PerformedBy");
            Timestamp entryDate = snapshot.GetValue<Timestamp>("EntryDate");

            return new AuditLog(action, performedBy, entryDate.ToDateTime())
            {
                AuditLogID = auditLogID
            };
        }

        // Insert an audit log
        public async Task<AuditLog> insertAuditLog(string action, string performedBy)
        {
            try
            {
                CollectionReference auditLogsRef = db.Collection("AuditLogs");

                // Generate current timestamp
                Timestamp currentTimestamp = Timestamp.FromDateTime(DateTime.UtcNow);

                // Find the highest existing ALx number
                QuerySnapshot allLogsSnapshot = await auditLogsRef.GetSnapshotAsync();
                int highestID = 0;

                foreach (var doc in allLogsSnapshot.Documents)
                {
                    string docID = doc.Id; // Example: "AL3"
                    if (docID.StartsWith("AL") && int.TryParse(docID.Substring(2), out int id))
                    {
                        highestID = Math.Max(highestID, id);
                    }
                }

                // Generate new Audit Log ID
                string auditLogID = $"AL{highestID + 1}";

                // Create document reference in Firestore
                DocumentReference docRef = auditLogsRef.Document(auditLogID);

                var auditLogData = new Dictionary<string, object>
                {
                    { "Action", action },
                    { "PerformedBy", performedBy },
                    { "EntryDate", currentTimestamp }
                };

                await docRef.SetAsync(auditLogData);

                Console.WriteLine($"Audit log inserted successfully with Firestore ID: {auditLogID}");

                return new AuditLog(action, performedBy, currentTimestamp.ToDateTime())
                {
                    AuditLogID = auditLogID
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inserting audit log: " + ex.Message);
                return null;
            }
        }
    }
}
