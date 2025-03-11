using ClearCare.Models.Entities;

namespace ClearCare.Models.Interfaces
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
