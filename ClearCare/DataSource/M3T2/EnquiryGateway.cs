using ClearCare.Models.Entities.M3T2;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;

namespace ClearCare.DataSource.M3T2
{
    public class EnquiryGateway
    {
        private readonly FirestoreDb _db;

        /// <summary>
        ///   Initializes a new instance of the <see cref="EnquiryGateway"/>
        ///   class.
        /// </summary>
        public EnquiryGateway()
        {
            _db = FirebaseService.Initialize();
        }

        /// <summary>
        ///   Saves an enquiry to Firestore.
        /// </summary>
        /// <param name="enquiry">The enquiry to save.</param>
        public async Task saveEnquiryAsync(Enquiry enquiry)
        {
            await _db.Collection("Enquiry").AddAsync(enquiry);
        }

        /// <summary>
        ///   Retrieves all enquiries for a specific user from Firestore.
        /// </summary>
        /// <param name="userUuid">The UUID of the user.</param>
        /// <returns>A list of <see cref="Enquiry"/> for the specified
        ///   user.</returns>
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

        /// <summary>
        ///   Retrieves all enquiries for a specific doctor from Firestore.
        /// </summary>
        /// <param name="userUuid">The UUID of the doctor.</param>
        /// <returns>A list of <see cref="Enquiry"/> for the specified
        ///   doctor.</returns>
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

        /// <summary>
        ///   Retrieves a specific enquiry from Firestore by its document ID.
        /// </summary>
        /// <param name="documentId">The ID of the document in Firestore.</param>
        /// <returns>
        ///   The <see cref="Enquiry"/> with the specified ID, or null if not
        ///   found.
        /// </returns>
        public async Task<Enquiry?> getEnquiryByIdAsync(string documentId)
        {
            DocumentReference docRef = _db.Collection("Enquiry").Document(documentId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                // null return is allowed because of 'Enquiry?' return type
                return null;
            }

            Enquiry enquiry = snapshot.ConvertTo<Enquiry>();
            enquiry.FirestoreId = documentId;

            return enquiry;
        }

        /// <summary>
        ///   Saves a reply to a specific enquiry in Firestore.
        /// </summary>
        /// <param name="enquiryId">The ID of the enquiry to reply to.</param>
        /// <param name="reply">The <see cref="Reply"/> to save.</param>
        /// <returns>The ID of the newly created reply document.</returns>
        public async Task<string> saveReplyAsync(string enquiryId, Reply reply)
        {
            reply.CreatedAt = DateTime.UtcNow;

            DocumentReference enquiryDocRef = _db.Collection("Enquiry").Document(enquiryId);
            CollectionReference repliesRef = enquiryDocRef.Collection("Replies");
            DocumentReference docRef = await repliesRef.AddAsync(reply);

            return docRef.Id;
        }

        /// <summary>
        ///   Retrieves all replies for a specific enquiry from Firestore.
        /// </summary>
        /// <param name="enquiryId">The ID of the enquiry.</param>
        /// <returns>A list of <see cref="Reply"/> for the specified
        ///   enquiry.</returns>
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
