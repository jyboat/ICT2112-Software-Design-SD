using Google.Cloud.Firestore;
using ClearCare.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

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

        public async Task<string> insertSummary(DischargeSummary summary)
        {
            DocumentReference docRef = _db.Collection("DischargeSummaries").Document();

            summary.setId(docRef.Id);

            Dictionary<string, object> data = summary.getDetails();

            await docRef.SetAsync(data);

            return docRef.Id;
        }

        public async Task<bool> updateSummary(string id, DischargeSummary updatedSummary)
        {
            DocumentReference docRef = _db.Collection("DischargeSummaries").Document(id);

            Dictionary<string, object> updatedData = updatedSummary.getDetails();

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
                    var data = doc.ToDictionary();
                    DischargeSummary summary = DischargeSummary.FromFirestoreData(doc.Id, data);   
                    summaries.Add(summary);
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
                return null;
            }

            var data = snapshot.ToDictionary();
            return DischargeSummary.FromFirestoreData(id, data);
        }
    }
}
