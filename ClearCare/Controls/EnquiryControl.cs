using ClearCare.Models;
using ClearCare.Gateways; // For EnquiryGateway, if you're using it
using Microsoft.Extensions.Logging;

namespace ClearCare.Controls
{
    public class EnquiryControl
    {
        private readonly ILogger<EnquiryControl> _logger;
        private readonly EnquiryGateway _enquiryGateway;

        // Here you can maintain an in-memory list or rely on Firestore
        // For demonstration, let's do both:
        public static List<Enquiry> Enquiries { get; set; } = new List<Enquiry>();

        public EnquiryControl(ILogger<EnquiryControl> logger, EnquiryGateway enquiryGateway)
        {
            _logger = logger;
            _enquiryGateway = enquiryGateway;
        }

        /// <summary>
        /// Create a new Enquiry (store in memory or Firestore).
        /// </summary>
        public async Task CreateEnquiryAsync(Enquiry enquiry)
        {
            _logger.LogInformation($"Creating enquiry from {enquiry.Name} with email {enquiry.Email}.");

            // Example: In-memory ID assignment
            enquiry.Id = Guid.NewGuid().ToString();

            // Add to local list
            Enquiries.Add(enquiry);

            // Also persist to Firestore (if using Firestore)
            await _enquiryGateway.SaveEnquiryAsync(enquiry);
        }

        public async Task SaveReplyAsync(string enquiryId, Reply reply)
        {
            // Delegate to the Gateway
            await _enquiryGateway.SaveReplyAsync(enquiryId, reply);
        }


        // EnquiryControl.cs
        public async Task<List<Reply>> FetchRepliesForEnquiryAsync(string enquiryId)
        {
            return await _enquiryGateway.GetRepliesForEnquiryAsync(enquiryId);
        }


        /// <summary>
        /// Fetch all Enquiries (from in-memory list or Firestore).
        /// </summary>
        public List<Enquiry> FetchAllEnquiries()
        {
            // Return in-memory, or you could fetch from Firestore if you prefer
            return Enquiries;
        }

        /// <summary>
        /// Fetch all Enquiries for a specific user from Firestore.
        /// </summary>
        public async Task<List<Enquiry>> FetchEnquiriesByUserAsync(string userUUID)
        {
            // Using Firestore gateway to retrieve
            var userEnquiries = await _enquiryGateway.GetEnquiriesForUserAsync(userUUID);
            return userEnquiries;
        }

        /// <summary>
        /// Fetch a single enquiry by its Firestore ID (for replying, etc.).
        /// </summary>
        public async Task<Enquiry> FetchEnquiryByFirestoreIdAsync(string firestoreId)
        {
            return await _enquiryGateway.GetEnquiryByIdAsync(firestoreId);
        }

        // You can also add other methods like replying to an enquiry, etc.
        // Or you can keep them in a separate "ReplyControl" if you like.
    }
}
