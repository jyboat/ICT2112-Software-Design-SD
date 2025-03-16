using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface ISummarySend
    {
        Task<List<DischargeSummary>> fetchSummaries();

        Task<DischargeSummary> fetchSummaryById(string id);

        Task<string> insertSummary(string details, string instructions, string createdAt, string patientId);

        Task<bool> updateSummary(string id, string details, string instructions, string createdAt, string patientId);

        Task<bool> deleteSummary(string id);
    }
}
