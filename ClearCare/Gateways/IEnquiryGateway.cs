using ClearCare.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Gateways
{
    public interface IEnquiryGateway
    {
        Task SaveEnquiryAsync(Enquiry enquiry);
        Task<List<Enquiry>> GetEnquiriesForUserAsync(string userUuid);
        Task<Enquiry> GetEnquiryByIdAsync(string documentId);
        Task<string> SaveReplyAsync(string enquiryId, Reply reply);
        Task<List<Reply>> GetRepliesForEnquiryAsync(string enquiryId);
        Task<List<SideEffectModel>> GetSideEffectsAsync();
        Task<List<Enquiry>> GetAllEnquiriesAsync(); // New method we'll implement
    }
}
