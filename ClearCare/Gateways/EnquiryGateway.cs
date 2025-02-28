using ClearCare.Models;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClearCare.Gateways
{
    public class EnquiryGateway : IEnquiryGateway  // Add the interface implementation here
    {
        private readonly FirestoreDb _db;

        public EnquiryGateway()
        {
            // If the default Firebase app is not initialized, initialize it:
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.GetApplicationDefault()
                });
            }

            // Replace "my-project-id" with your actual Firebase / GCP project ID
            _db = FirestoreDb.Create("ict2112");
        }

        // Add this new method to get all enquiries
        public async Task<List<Enquiry>> GetAllEnquiriesAsync()
        {
            var enquiries = new List<Enquiry>();
            var snapshot = await _db.Collection("Enquiry").GetSnapshotAsync();

            foreach (var doc in snapshot.Documents)
            {
                if (doc.Exists)
                {
                    var enquiry = doc.ConvertTo<Enquiry>();
                    enquiry.FirestoreId = doc.Id;  // Make sure to set the ID
                    enquiries.Add(enquiry);
                }
            }

            return enquiries;
        }

        public async Task SaveEnquiryAsync(Models.Enquiry enquiry)
        {
            // Save to the "Enquiry" collection with an auto-generated document ID
            await _db.Collection("Enquiry").AddAsync(enquiry);
        }

        public async Task<List<Enquiry>> GetEnquiriesForUserAsync(string userUuid)
        {
            // 1) Build a query that looks for documents where UserUUID == userUuid
            Query query = _db.Collection("Enquiry").WhereEqualTo("UserUUID", userUuid);

            // 2) Execute the query
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            // 3) Convert each matching document into an Enquiry
            var enquiries = new List<Enquiry>();
            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                if (doc.Exists)
                {
                    Enquiry e = doc.ConvertTo<Enquiry>();
                    e.FirestoreId = doc.Id;  // Make sure to set the ID
                    enquiries.Add(e);
                }
            }
            return enquiries;
        }

        public async Task<Enquiry> GetEnquiryByIdAsync(string documentId)
        {
            // Get a reference to the document
            DocumentReference docRef = _db.Collection("Enquiry").Document(documentId);

            // Get the document snapshot
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            // Check if the document exists
            if (!snapshot.Exists)
            {
                return null;
            }

            // Convert to Enquiry model
            Enquiry enquiry = snapshot.ConvertTo<Enquiry>();

            // Set the FirestoreId property
            enquiry.FirestoreId = documentId;

            return enquiry;
        }

        public async Task<string> SaveReplyAsync(string enquiryId, Reply reply)
        {
            // Set the creation timestamp
            reply.CreatedAt = DateTime.UtcNow;

            // Reference the specific Enquiry document using the provided enquiryId
            DocumentReference enquiryDocRef = _db.Collection("Enquiry").Document(enquiryId);

            // Add the reply to a subcollection called "Replies" under the specific Enquiry document
            CollectionReference repliesRef = enquiryDocRef.Collection("Replies");
            DocumentReference docRef = await repliesRef.AddAsync(reply);

            // Return the document ID of the newly created reply
            return docRef.Id;
        }

        public async Task<List<Reply>> GetRepliesForEnquiryAsync(string enquiryId)
        {
            try
            {
                // Reference the Replies subcollection for the specific enquiry
                var repliesRef = _db.Collection("Enquiry").Document(enquiryId).Collection("Replies");

                // Fetch all documents in the Replies subcollection
                var snapshot = await repliesRef.GetSnapshotAsync();

                // Convert the documents to a list of Reply objects
                var replies = snapshot.Documents
                    .Select(doc => {
                        var reply = doc.ConvertTo<Reply>();
                        reply.FirestoreId = doc.Id;  // Make sure to set the ID
                        return reply;
                    })
                    .ToList();

                return replies;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching replies for enquiry {enquiryId}: {ex.Message}");
                return new List<Reply>();
            }
        }

        public async Task<List<SideEffectModel>> GetSideEffectsAsync()
        {
            var sideEffects = new List<SideEffectModel>();

            // Reference the "SideEffects" collection in Firestore
            var collection = _db.Collection("SideEffects");
            var snapshot = await collection.GetSnapshotAsync();

            foreach (var document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    // Convert Firestore document to SideEffectModel
                    var sideEffect = document.ConvertTo<SideEffectModel>();
                    sideEffects.Add(sideEffect);

                    // Log the side effect details to the console
                    Console.WriteLine($"Document ID: {document.Id}");
                    Console.WriteLine($"Name: {sideEffect.Name}");
                    Console.WriteLine($"Description: {sideEffect.Description}");
                    Console.WriteLine($"Date: {sideEffect.Date}");
                }
            }

            return sideEffects;
        }
    }
}
