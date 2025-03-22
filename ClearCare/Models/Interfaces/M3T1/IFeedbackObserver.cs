using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface IFeedbackObserver
    {
        void Update(string feedbackId);
    }
}
