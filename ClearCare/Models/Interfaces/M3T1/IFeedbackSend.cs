using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface IFeedbackSend
    {
        Task<List<Feedback>> fetchFeedbacks();

        Task<List<Feedback>> fetchFeedbacksByUserId(string userId);

        Task<string> insertFeedback(string content, int rating, string userId, string dateCreated);
    }
}
