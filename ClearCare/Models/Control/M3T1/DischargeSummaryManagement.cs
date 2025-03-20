using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.DataSource;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T1;
using ClearCare.Models.Interfaces.M3T2;
using ClearCare.Models.Entities.M3T2;


namespace ClearCare.Models.Control.M3T1
{
    public class DischargeSummaryManager : ISummaryReceive
    {
        private readonly ISummarySend _gateway;
        private readonly IFetchPrescriptions _fetchPrescriptions;

        public DischargeSummaryManager(ISummarySend gateway, IFetchPrescriptions fetcher)
        {
            _gateway = gateway;
            _fetchPrescriptions = fetcher;
        }

        public Task receiveSummaries(List<DischargeSummary> summaries)
        {
            if (summaries.Count > 0)
            {
                Console.WriteLine($"Received {summaries.Count} summaries");
            }
            else
            {
                Console.WriteLine("No summaries received");
            }
            return Task.CompletedTask;
        }

        public Task receiveSummary(DischargeSummary summary)
        {
            if (summary != null)
            {
                Console.WriteLine($"Received summary");
            }
            else
            {
                Console.WriteLine("No summary received)");
            }
            return Task.CompletedTask;
        }

        public Task receiveAddStatus(bool success)
        {
            if (success)
            {
                Console.WriteLine("Inserted summary successfully");
            }
            else
            {
                Console.WriteLine("Failed to insert summary");
            }
            return Task.CompletedTask;
        }

        public Task receiveUpdateStatus(bool success)
        {
            if (success)
            {
                Console.WriteLine("Updated summary successfully");
            }
            else
            {
                Console.WriteLine("Failed to update summary");
            }
            return Task.CompletedTask;
        }

        public Task receiveDeleteStatus(bool success)
        {
            if (success)
            {
                Console.WriteLine("Deleted summary successfully");
            }
            else
            {
                Console.WriteLine("Failed to delete summary");
            }
            return Task.CompletedTask;
        }
        public async Task<PrescriptionModel?> getPrescription(string patientId)
        {
            List<PrescriptionModel> prescriptions = await _fetchPrescriptions.FetchPrescriptions();
            PrescriptionModel patientPrescript = null;
            foreach (var prescription in prescriptions)
            {
                if (prescription.PatientId.ToString() == patientId)
                {
                    patientPrescript = prescription;
                }
            }
            return patientPrescript;
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
