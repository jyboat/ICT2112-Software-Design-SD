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
        private string PatientId { get; set; }

        [FirestoreProperty]
        private string ImagePath { get; set; }

        // NEW: Home Assessment Checklist property
        [FirestoreProperty]
        private Dictionary<string, bool> HomeAssessmentChecklist { get; set; } = new Dictionary<string, bool>();

        [FirestoreProperty]
        private string HazardType { get; set; }



        public Assessment(
            string id, 
            string hazardType,  // Add this parameter
            string riskLevel, 
            string recommendation, 
            DateTime createdAt, 
            string patientId, 
            string imagePath, 
            Dictionary<string, bool> homeAssessmentChecklist = null)
        {
            Id = id;
            HazardType = hazardType;  // Add this assignment
            RiskLevel = riskLevel;
            Recommendation = recommendation;
            CreatedAt = createdAt;
            PatientId = patientId;
            ImagePath = imagePath ?? "";
            HomeAssessmentChecklist = homeAssessmentChecklist ?? new Dictionary<string, bool>();
        }

        // Public getter methods
        private string getId() => Id;
        private string getRiskLevel() => RiskLevel;
        private string getRecommendation() => Recommendation;
        private DateTime getCreatedAt() => CreatedAt;
        private string getPatientId() => PatientId;
        private string getImagePath() => ImagePath;
        private Dictionary<string, bool> getHomeAssessmentChecklist() => HomeAssessmentChecklist;
        
        public string getHazardType() => HazardType;

        // Public setter methods
        private void setId(string id) => Id = id;
        private void setRiskLevel(string riskLevel) => RiskLevel = riskLevel;
        private void setRecommendation(string recommendation) => Recommendation = recommendation;
        private void setCreatedAt(DateTime createdAt) => CreatedAt = createdAt;
        private void setPatientId(string patientId) => PatientId = patientId;
        private void setImagePath(string imagePath) => ImagePath = imagePath ?? "";
        private void setHomeAssessmentChecklist(Dictionary<string, bool> checklist) => HomeAssessmentChecklist = checklist ?? new Dictionary<string, bool>();
        private void setHazardType(string hazardType) => HazardType = hazardType;

        // Retrieve assessment details as a dictionary
        public Dictionary<string, object> getAssessmentDetails()
        {
            var details = new Dictionary<string, object>
            {
                { "Id", getId() },
                { "RiskLevel", getRiskLevel() },
                { "Recommendation", getRecommendation() },
                { "CreatedAt", getCreatedAt() },
                { "PatientId", getPatientId() },
                { "ImagePath", getImagePath() },
                { "HazardType", getHazardType() }
            };
            
            // Add checklist if it has items
            if (getHomeAssessmentChecklist().Count > 0)
            {
                details.Add("HomeAssessmentChecklist", getHomeAssessmentChecklist());
            }
            
            return details;
        }

    }
}