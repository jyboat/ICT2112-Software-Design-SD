using ClearCare.Models.Entities;

namespace ClearCare.Models.Interfaces
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
