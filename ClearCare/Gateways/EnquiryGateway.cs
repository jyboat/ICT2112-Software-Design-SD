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
    }
}
