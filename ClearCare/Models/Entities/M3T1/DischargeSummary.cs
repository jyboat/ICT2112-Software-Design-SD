using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;
using Google.Cloud.Location;
using Newtonsoft.Json;

namespace ClearCare.Models.Entities.M3T1
{
    public class DischargeSummary
    {
        private string Id { get; set; }
        [FirestoreProperty]
        private string Details { get; set; }
        [FirestoreProperty]
        private string Instructions { get; set; }
        [FirestoreProperty]
        private string CreatedAt { get; set; }
        [FirestoreProperty]
        private string Status { get; set; }
        [FirestoreProperty]
        private string PatientId { get; set; }

        private string getId() => Id;
        private string getDetails() => Details;
        private string getInstructions() => Instructions;
        private string getCreatedAt() => CreatedAt;
        private string getStatus() => Status;
        private string getPatientId() => PatientId;

        private void setId(string id) => Id = id;
        private void setDetails(string details) => Details = details;
        private void setInstructions(string instructions) => Instructions = instructions;
        private void setCreatedAt(string createdAt) => CreatedAt = createdAt;
        private void setStatus(string status) => Status = status;
        private void setPatientId(string patientId) => PatientId = patientId;

        public DischargeSummary() { }

        public DischargeSummary(string id, string details, string instructions, string createdAt, string status, string patientId)
        {
            Id = id;
            Details = details;
            Instructions = instructions;
            CreatedAt = createdAt;
            Status =  status;
            PatientId = patientId;
        }

        public Dictionary<string, object> GetSummaryDetails()
        {
            return new Dictionary<string, object>
            {
                { "Id", getId() },
                { "Details", getDetails() },
                { "Instructions", getInstructions() },
                { "CreatedAt", getCreatedAt() },
                {"Status", getStatus() },
                { "PatientId", getPatientId() }
            };
        }

        public void setSummaryDetails(Dictionary<string, object> summaryDetails)
        {
            if (summaryDetails == null)
            {
                throw new ArgumentNullException(nameof(summaryDetails));
            }

            if (summaryDetails.ContainsKey("Id")) setId(summaryDetails["Id"]?.ToString() ?? string.Empty);
            if (summaryDetails.ContainsKey("Details")) setDetails(summaryDetails["Details"]?.ToString() ?? string.Empty);
            if (summaryDetails.ContainsKey("Instructions")) setInstructions(summaryDetails["Instructions"]?.ToString() ?? string.Empty);
            if (summaryDetails.ContainsKey("CreatedAt")) setCreatedAt(summaryDetails["CreatedAt"]?.ToString() ?? string.Empty);
            if (summaryDetails.ContainsKey("Status")) setStatus(summaryDetails["Status"]?.ToString() ?? string.Empty);
            if (summaryDetails.ContainsKey("PatientId")) setPatientId(summaryDetails["PatientId"]?.ToString() ?? string.Empty);
        }
    }
}