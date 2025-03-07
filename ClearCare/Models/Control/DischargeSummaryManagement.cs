using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.DataSource;
using ClearCare.Models.Entities;


namespace ClearCare.Models.Controls
{
    public class DischargeSummaryManager
    {
        private readonly SummaryGateway _gateway;

        public DischargeSummaryManager()
        {
            _gateway = new SummaryGateway();
        }

        public async Task<bool> updateSummary(string id, string details, string instructions, string createdAt, string patientId)
        {
            return await _gateway.updateSummary(id, details, instructions, createdAt, patientId);
        }

        public async Task<string> generateSummary(string details, string instructions, string createdAt, string patientId)
        {
            return await _gateway.insertSummary(details, instructions, createdAt, patientId);
        }

        public async Task<List<DischargeSummary>> getSummaries()
        {
            return await _gateway.fetchSummaries();
        }

        public async Task<DischargeSummary> getSummary(string id)
        {
            return await _gateway.fetchSummaryById(id);
        }

        public async Task<bool> deleteSummary(string id)
        {
            return await _gateway.deleteSummary(id);
        }
    }
}
