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


        public async Task<List<Enquiry>> GetAllEnquiriesAsync()
        {
            // 1) Grab all docs in "Enquiry" collection
            QuerySnapshot snapshot = await _db.Collection("Enquiry").GetSnapshotAsync();

            // 2) Convert each document to an Enquiry
            var enquiries = new List<Enquiry>();
            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                if (doc.Exists)
                {
                    // If you have [FirestoreData] attributes on Enquiry, you can do:
                    Enquiry e = doc.ConvertTo<Enquiry>();

                    // Or, if you want the Firestore auto-ID:
                    // e.FirestoreId = doc.Id;  // Only if you have a FirestoreId property

                    enquiries.Add(e);
                }
            }
            return enquiries;
        }
    }
}
