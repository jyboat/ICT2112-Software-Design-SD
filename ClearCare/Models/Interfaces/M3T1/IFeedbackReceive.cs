using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface IFeedbackReceive
    {
        Task receiveFeedbacks(List<Feedback> feedbacks);

        Task receiveFeedbackById(Feedback feedback);

        Task receiveFeedbacksByUserId(List<Feedback> feedback);

        Task receiveAddStatus(bool success);

        Task receiveDeleteStatus(bool success);
    }
}
