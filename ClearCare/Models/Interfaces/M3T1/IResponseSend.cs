using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface IResponseSend
    {
        Task<List<FeedbackResponse>> fetchResponses();

        Task<FeedbackResponse> fetchResponseByFeedbackId(string feedbackId);

        Task<string> insertResponse(string feedbackId, string response, string userId, string dateResponded);

        Task<bool> updateResponse(string id, string response, string userId, string dateResponded);

        Task<bool> deleteResponse(string id);
    }
}
