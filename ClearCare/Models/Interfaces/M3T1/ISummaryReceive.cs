using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface ISummaryReceive
    {
        Task receiveSummaries(List<DischargeSummary> summaries);
        Task receiveSummary(DischargeSummary summary);
        Task receiveAddStatus(bool success);
        Task receiveUpdateStatus(bool success);
        Task receiveDeleteStatus(bool success);
    }
}
