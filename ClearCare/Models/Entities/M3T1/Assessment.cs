using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;
using Google.Cloud.Location;
using Newtonsoft.Json;

namespace ClearCare.Models.Entities.M3T1
{
    public class Assessment
    {
        // Private properties
        private string Id { get; set; }
        
        [FirestoreProperty]
        private string RiskLevel { get; set; }

        [FirestoreProperty]
        private string Recommendation { get; set; }

        [FirestoreProperty]
        private DateTime CreatedAt { get; set; }

        [FirestoreProperty]
        private string DoctorID { get; set; }

        [FirestoreProperty]
        private string PatientID { get; set; }

        [FirestoreProperty]
        private List<string> ImagePath { get; set; } = new List<string>();

        // Constructor
        public Assessment() { }

        public Assessment(string id, string riskLevel, string recommendation, DateTime createdAt, string doctorId, string patientId, List<string> imagePath)
        {
            Id = id;
            RiskLevel = riskLevel;
            Recommendation = recommendation;
            CreatedAt = createdAt;
            DoctorID = doctorId;
            PatientID = patientId;
            ImagePath = imagePath ?? new List<string>();
        }

        // Public getter methods
        public string GetId() => Id;
        public string GetRiskLevel() => RiskLevel;
        public string GetRecommendation() => Recommendation;
        public DateTime GetCreatedAt() => CreatedAt;
        public string GetDoctorId() => DoctorID;
        public string GetPatientId() => PatientID;
        public List<string> GetImagePath() => ImagePath;

        // Public setter methods
        public void SetId(string id) => Id = id;
        public void SetRiskLevel(string riskLevel) => RiskLevel = riskLevel;
        public void SetRecommendation(string recommendation) => Recommendation = recommendation;
        public void SetCreatedAt(DateTime createdAt) => CreatedAt = createdAt;
        public void SetDoctorId(string doctorId) => DoctorID = doctorId;
        public void SetPatientId(string patientId) => PatientID = patientId;
        public void SetImagePath(List<string> imagePath) => ImagePath = imagePath ?? new List<string>();

        // Retrieve assessment details as a dictionary
        public Dictionary<string, object> GetAssessmentDetails()
        {
            return new Dictionary<string, object>
            {
                { "Id", GetId() },
                { "RiskLevel", GetRiskLevel() },
                { "Recommendation", GetRecommendation() },
                { "CreatedAt", GetCreatedAt() },
                { "DoctorID", GetDoctorId() },
                { "PatientID", GetPatientId() },
                { "ImagePath", GetImagePath() }
            };
        }

        // Update assessment details from a dictionary
        public void SetAssessmentDetails(Dictionary<string, object> assessmentDetails)
        {
            if (assessmentDetails == null)
            {
                throw new ArgumentNullException(nameof(assessmentDetails));
            }

            if (assessmentDetails.ContainsKey("Id")) SetId(assessmentDetails["Id"]?.ToString() ?? string.Empty);
            if (assessmentDetails.ContainsKey("RiskLevel")) SetRiskLevel(assessmentDetails["RiskLevel"]?.ToString() ?? string.Empty);
            if (assessmentDetails.ContainsKey("Recommendation")) SetRecommendation(assessmentDetails["Recommendation"]?.ToString() ?? string.Empty);
            if (assessmentDetails.ContainsKey("CreatedAt")) SetCreatedAt(Convert.ToDateTime(assessmentDetails["CreatedAt"] ?? DateTime.Now));
            if (assessmentDetails.ContainsKey("DoctorID")) SetDoctorId(assessmentDetails["DoctorID"]?.ToString() ?? string.Empty);
            if (assessmentDetails.ContainsKey("PatientID")) SetPatientId(assessmentDetails["PatientID"]?.ToString() ?? string.Empty);
            if (assessmentDetails.ContainsKey("ImagePath")) SetImagePath(assessmentDetails["ImagePath"] as List<string> ?? new List<string>());
        }

        // Allow doctors to update their reviews only
        public void SetDoctorReview(Dictionary<string, object> reviewDetails)
        {
            if (reviewDetails == null)
            {
                throw new ArgumentNullException(nameof(reviewDetails));
            }

            if (reviewDetails.ContainsKey("Recommendation")) SetRecommendation(reviewDetails["Recommendation"]?.ToString() ?? string.Empty);
        }

        // Allow patients to update their assessment details
        public void SetPatientAssessment(Dictionary<string, object> patientDetails)
        {
            if (patientDetails == null)
            {
                throw new ArgumentNullException(nameof(patientDetails));
            }

            if (patientDetails.ContainsKey("RiskLevel")) SetRiskLevel(patientDetails["RiskLevel"]?.ToString() ?? string.Empty);
            if (patientDetails.ContainsKey("ImagePath")) SetImagePath(patientDetails["ImagePath"] as List<string> ?? new List<string>());
        }
    }
}
