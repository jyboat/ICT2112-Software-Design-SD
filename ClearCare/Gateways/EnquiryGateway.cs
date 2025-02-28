using ClearCare.Models;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;

namespace ClearCare.Gateways
{
    public class EnquiryGateway
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
            // Query the Replies collection for documents where EnquiryId matches
            Query query = _db.Collection("Replies")
                                     .WhereEqualTo("EnquiryId", enquiryId)
                                     .OrderByDescending("CreatedAt");

            // Execute the query
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            // Convert the results to Reply objects
            List<Reply> replies = new List<Reply>();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    Reply reply = documentSnapshot.ConvertTo<Reply>();
                    reply.FirestoreId = documentSnapshot.Id;
                    replies.Add(reply);
                }
            }

            return replies;
        }




    }
}
