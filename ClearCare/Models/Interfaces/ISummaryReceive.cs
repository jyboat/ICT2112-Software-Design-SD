using ClearCare.Models.Entities;

namespace ClearCare.Models.Interfaces
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
