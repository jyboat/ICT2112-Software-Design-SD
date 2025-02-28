using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;
using Google.Cloud.Location;
using Newtonsoft.Json;

namespace ClearCare.Models.Entities
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
        private string PatientId { get; set; }

        private string GetId() => Id;
        private string GetDetails() => Details;
        private string GetInstructions() => Instructions;
        private string GetCreatedAt() => CreatedAt;
        private string GetPatientId() => PatientId;

        private void SetId(string id) => Id = id;
        private void SetDetails(string details) => Details = details;
        private void SetInstructions(string instructions) => Instructions = instructions;
        private void SetCreatedAt(string createdAt) => CreatedAt = createdAt;
        private void SetPatientId(string patientId) => PatientId = patientId;

        public DischargeSummary() { }

        public DischargeSummary(string id, string details, string instructions, string createdAt, string patientId)
        {
            Id = id;
            Details = details;
            Instructions = instructions;
            CreatedAt = createdAt;
            PatientId = patientId;
        }

        public Dictionary<string, object> GetSummaryDetails()
        {
            return new Dictionary<string, object>
            {
                { "Id", GetId() },
                { "Details", GetDetails() },
                { "Instructions", GetInstructions() },
                { "CreatedAt", GetCreatedAt() },
                { "PatientId", GetPatientId() }
            };
        }

        public void setSummaryDetails(Dictionary<string, object> summaryDetails)
        {
            if (summaryDetails == null)
            {
                throw new ArgumentNullException(nameof(summaryDetails));
            }

            if (summaryDetails.ContainsKey("Id")) SetId(summaryDetails["Id"]?.ToString() ?? string.Empty);
            if (summaryDetails.ContainsKey("Details")) SetDetails(summaryDetails["Details"]?.ToString() ?? string.Empty);
            if (summaryDetails.ContainsKey("Instructions")) SetInstructions(summaryDetails["Instructions"]?.ToString() ?? string.Empty);
            if (summaryDetails.ContainsKey("CreatedAt")) SetCreatedAt(summaryDetails["CreatedAt"]?.ToString() ?? string.Empty);
            if (summaryDetails.ContainsKey("PatientId")) SetPatientId(summaryDetails["PatientId"]?.ToString() ?? string.Empty);
        }
    }
}