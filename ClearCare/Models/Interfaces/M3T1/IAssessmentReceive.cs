using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface IAssessmentReceive
    {

        Task receiveAssessments(List<Assessment> assessments);
        Task receiveAssessment(Assessment assessment);
        Task receiveInsertStatus(bool status);
        Task receiveUpdateStatus(bool status);
        Task receiveDeleteStatus(bool status);
    }
}
