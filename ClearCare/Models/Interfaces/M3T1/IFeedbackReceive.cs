using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface IFeedbackReceive
    {
        Task receiveFeedbacks(List<Feedback> feedbacks);
        Task receiveFeedback(Feedback feedback);
        Task receiveFeedbacksByPatientId(List<Feedback> feedback);
        Task receiveAddStatus(bool success);
        Task receiveUpdateStatus(bool success);
        Task receiveResponseStatus(bool success);
        Task receiveDeleteStatus(bool success);
    }
}
