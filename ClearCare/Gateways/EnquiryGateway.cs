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
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.GetApplicationDefault()
                });
            }

            _db = FirestoreDb.Create("ict2112");
        }

        public async Task saveEnquiryAsync(Enquiry enquiry)
        {
            await _db.Collection("Enquiry").AddAsync(enquiry);
        }

        public async Task<List<Enquiry>> getEnquiriesForUserAsync(string userUuid)
        {
            Query query = _db.Collection("Enquiry").WhereEqualTo("UserUUID", userUuid);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

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
        

            public async Task<List<Enquiry>> getEnquiriesForDoctorAsync(string userUuid)
        {
            Query query = _db.Collection("Enquiry").WhereEqualTo("DoctorUUID", userUuid);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

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

        public async Task<Enquiry> getEnquiryByIdAsync(string documentId)
        {
            DocumentReference docRef = _db.Collection("Enquiry").Document(documentId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                return null;
            }

            Enquiry enquiry = snapshot.ConvertTo<Enquiry>();
            enquiry.FirestoreId = documentId;

            return enquiry;
        }

        public async Task<string> saveReplyAsync(string enquiryId, Reply reply)
        {
            reply.CreatedAt = DateTime.UtcNow;

            DocumentReference enquiryDocRef = _db.Collection("Enquiry").Document(enquiryId);
            CollectionReference repliesRef = enquiryDocRef.Collection("Replies");
            DocumentReference docRef = await repliesRef.AddAsync(reply);

            return docRef.Id;
        }

        public async Task<List<Reply>> getRepliesForEnquiryAsync(string enquiryId)
        {
            try
            {
                var repliesRef = _db.Collection("Enquiry").Document(enquiryId).Collection("Replies");
                var snapshot = await repliesRef.GetSnapshotAsync();

                var replies = snapshot.Documents
                    .Select(doc => doc.ConvertTo<Reply>())
                    .ToList();

                return replies;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching replies for enquiry {enquiryId}: {ex.Message}");
                return new List<Reply>();
            }
        }
    }
}
