using Google.Cloud.Firestore;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using ClearCare.Models.Entities;

namespace ClearCare.DataSource
{
    public class SummaryGateway
    {
        private readonly FirestoreDb _db;

        public SummaryGateway()
        {
            // Initialize Firebase
            _db = FirebaseService.Initialize();
        }

        public async Task<string> insertSummary(string details, string instructions, string createdAt, string patientId)
        {
            DocumentReference docRef = _db.Collection("DischargeSummaries").Document();

            var summary = new Dictionary<string, object>
            {
                { "Details", details },
                { "Instructions", instructions },
                { "CreatedAt", createdAt },
                { "PatientId", patientId }
            };

            await docRef.SetAsync(summary);

            return docRef.Id;
        }

        public async Task<bool> updateSummary(string id, string details, string instructions, string createdAt, string patientId)
        {
            DocumentReference docRef = _db.Collection("DischargeSummaries").Document(id);

            var updatedData = new Dictionary<string, object>
            {
                { "Details", details },
                { "Instructions", instructions },
                { "CreatedAt", createdAt },
                { "PatientId", patientId }
            };

            // Check if document exists before updating
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                return false; // Document not found
            }

            await docRef.UpdateAsync(updatedData);
            return true;
        }

        public async Task<List<DischargeSummary>> fetchSummaries()
        {
            List<DischargeSummary> summaries = new List<DischargeSummary>();

            CollectionReference summariesRef = _db.Collection("DischargeSummaries");
            QuerySnapshot snapshot = await summariesRef.GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                if (doc.Exists) {
                    try
                    {
                        string id = doc.ContainsField("Id") ? doc.GetValue<string>("Id") : doc.Id;
                        string details = doc.ContainsField("Details") ? doc.GetValue<string>("Details") : "";
                        string instructions = doc.ContainsField("Instructions") ? doc.GetValue<string>("Instructions") : "";
                        string createdAt = doc.ContainsField("CreatedAt") ? doc.GetValue<string>("CreatedAt") : "";
                        string patientId = doc.ContainsField("PatientId") ? doc.GetValue<string>("PatientId") : "";

                        DischargeSummary summary = new DischargeSummary(id, details, instructions, createdAt, patientId);
                        summaries.Add(summary);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting summary {doc.Id}: {ex.Message}");
                    }
                }
            }

            return summaries;
        }

        public async Task<DischargeSummary> fetchSummaryById(string id)
        {
            DocumentReference docRef = _db.Collection("DischargeSummaries").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Console.WriteLine($"No summary found {snapshot.Id}");
                return null;
            }

            string details = snapshot.GetValue<string>("Details");
            string instructions = snapshot.GetValue<string>("Instructions");
            string createdAt = snapshot.GetValue<string>("CreatedAt");
            string patientId = snapshot.GetValue<string>("PatientId");

            DischargeSummary summary = new DischargeSummary(id, details, instructions, createdAt, patientId);
            return summary;
        }

        public async Task<bool> deleteSummary(string id)
        {
            DocumentReference docRef = _db.Collection("DischargeSummaries").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Console.WriteLine($"No summary found {snapshot.Id}");
                return false;
            }

            await docRef.DeleteAsync();
            return true;
        }
    }
}
