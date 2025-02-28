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

        

        

    }
}
