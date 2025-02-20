using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.DataSource;
using ClearCare.Models;


namespace ClearCare.Controls
{
    public class DischargeSummaryManager
    {
        private readonly SummaryGateway _gateway;

        public DischargeSummaryManager()
        {
            _gateway = new SummaryGateway();
        }

        public async Task<bool> updateSummary(string id, DischargeSummary updated)
        {
            return await _gateway.updateSummary(id, updated);
        }

        public async Task<string> generateSummary(DischargeSummary summary)
        {
            return await _gateway.insertSummary(summary);
        }

        public async Task<List<DischargeSummary>> getSummaries()
        {
            return await _gateway.fetchSummaries();
        }

        public async Task<DischargeSummary> getSummary(string id)
        {
            return await _gateway.fetchSummaryById(id);
        }
    }
}
