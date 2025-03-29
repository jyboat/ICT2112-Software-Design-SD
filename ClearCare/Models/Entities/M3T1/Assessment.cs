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
            string doctorId, 
            string patientId, 
            List<string> imagePath, 
            Dictionary<string, bool> homeAssessmentChecklist = null)
        {
            Id = id;
            HazardType = hazardType;  // Add this assignment
            RiskLevel = riskLevel;
            Recommendation = recommendation;
            CreatedAt = createdAt;
            DoctorID = doctorId;
            PatientID = patientId;
            ImagePath = imagePath ?? new List<string>();
            HomeAssessmentChecklist = homeAssessmentChecklist ?? new Dictionary<string, bool>();
        }

        // Public getter methods
        public string getId() => Id;
        public string getRiskLevel() => RiskLevel;
        public string getRecommendation() => Recommendation;
        public DateTime getCreatedAt() => CreatedAt;
        public string getDoctorId() => DoctorID;
        public string getPatientId() => PatientID;
        public List<string> getImagePath() => ImagePath;
        public Dictionary<string, bool> getHomeAssessmentChecklist() => HomeAssessmentChecklist;
        
        public string getHazardType() => HazardType;

        // Public setter methods
        public void setId(string id) => Id = id;
        public void setRiskLevel(string riskLevel) => RiskLevel = riskLevel;
        public void setRecommendation(string recommendation) => Recommendation = recommendation;
        public void setCreatedAt(DateTime createdAt) => CreatedAt = createdAt;
        public void setDoctorId(string doctorId) => DoctorID = doctorId;
        public void setPatientId(string patientId) => PatientID = patientId;
        public void setImagePath(List<string> imagePath) => ImagePath = imagePath ?? new List<string>();
        public void setHomeAssessmentChecklist(Dictionary<string, bool> checklist) => HomeAssessmentChecklist = checklist ?? new Dictionary<string, bool>();
        public void setHazardType(string hazardType) => HazardType = hazardType;

        // Retrieve assessment details as a dictionary
        public Dictionary<string, object> getAssessmentDetails()
        {
            var details = new Dictionary<string, object>
            {
                { "Id", getId() },
                { "RiskLevel", getRiskLevel() },
                { "Recommendation", getRecommendation() },
                { "CreatedAt", getCreatedAt() },
                { "DoctorID", getDoctorId() },
                { "PatientID", getPatientId() },
                { "ImagePath", getImagePath() }
            };
            
            // Add checklist if it has items
            if (HomeAssessmentChecklist.Count > 0)
            {
                details.Add("HomeAssessmentChecklist", HomeAssessmentChecklist);
            }
            
            return details;
        }

        // Update assessment details from a dictionary
        public void SetAssessmentDetails(Dictionary<string, object> assessmentDetails)
        {
            if (assessmentDetails == null)
            {
                throw new ArgumentNullException(nameof(assessmentDetails));
            }

            if (assessmentDetails.ContainsKey("Id")) setId(assessmentDetails["Id"]?.ToString() ?? string.Empty);
            if (assessmentDetails.ContainsKey("RiskLevel")) setRiskLevel(assessmentDetails["RiskLevel"]?.ToString() ?? string.Empty);
            if (assessmentDetails.ContainsKey("Recommendation")) setRecommendation(assessmentDetails["Recommendation"]?.ToString() ?? string.Empty);
            if (assessmentDetails.ContainsKey("CreatedAt")) setCreatedAt(Convert.ToDateTime(assessmentDetails["CreatedAt"] ?? DateTime.Now));
            if (assessmentDetails.ContainsKey("DoctorID")) setDoctorId(assessmentDetails["DoctorID"]?.ToString() ?? string.Empty);
            if (assessmentDetails.ContainsKey("PatientID")) setPatientId(assessmentDetails["PatientID"]?.ToString() ?? string.Empty);
            if (assessmentDetails.ContainsKey("ImagePath")) setImagePath(assessmentDetails["ImagePath"] as List<string> ?? new List<string>());
            
            // Add checklist setting
            if (assessmentDetails.ContainsKey("HomeAssessmentChecklist"))
            {
                setHomeAssessmentChecklist(assessmentDetails["HomeAssessmentChecklist"] as Dictionary<string, bool> ?? new Dictionary<string, bool>());
            }
        }

        // Allow doctors to update their reviews only
        public void setDoctorReview(Dictionary<string, object> reviewDetails)
        {
            if (reviewDetails == null)
            {
                throw new ArgumentNullException(nameof(reviewDetails));
            }

            if (reviewDetails.ContainsKey("Recommendation")) setRecommendation(reviewDetails["Recommendation"]?.ToString() ?? string.Empty);
        }

        // Allow patients to update their assessment details
        public void setPatientAssessment(Dictionary<string, object> patientDetails)
        {
            if (patientDetails == null)
            {
                throw new ArgumentNullException(nameof(patientDetails));
            }

            if (patientDetails.ContainsKey("RiskLevel")) setRiskLevel(patientDetails["RiskLevel"]?.ToString() ?? string.Empty);
            if (patientDetails.ContainsKey("ImagePath")) setImagePath(patientDetails["ImagePath"] as List<string> ?? new List<string>());
        }
    }
}