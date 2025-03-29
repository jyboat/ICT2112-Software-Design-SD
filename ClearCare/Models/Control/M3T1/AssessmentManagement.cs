using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.DataSource;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T1;

namespace ClearCare.Models.Control.M3T1
{
    public class AssessmentManager : IAssessmentReceive
    {
        private readonly IAssessmentSend _gateway;
        private IHazardChecklistStrategy _strategy;


        public AssessmentManager(IAssessmentSend gateway)
        {
            _gateway = gateway;

        }


                // In AssessmentManager.cs
        public void setHazardType(string hazardType)
        {
            _strategy = hazardType?.ToLower() switch
            {
                "fire safety" => new FireSafetyChecklistStrategy(),
                "fall risk" => new FallRiskChecklistStrategy(),
                "wet condition" => new WetConditionChecklistStrategy(),
                _ => throw new ArgumentException($"Invalid hazard type: {hazardType}. Valid types are: Fire Safety, Fall Risk, Wet Condition")
            };
        }

        public List<Dictionary<string, string>> getQualifiedDoctors()
        {
            if (_strategy == null) throw new InvalidOperationException("Hazard type not set");
            return _strategy.getQualifiedDoctors();
        }

         public Dictionary<string, bool> getDefaultChecklist()
        {
            if (_strategy == null) 
                throw new InvalidOperationException("Hazard type not set. Call setHazardType() first.");
                
            return _strategy.getDefaultChecklist();
        }

        public Task receiveAssessments(List<Assessment> assessments)
        {
            if (assessments.Count > 0)
            {
                Console.WriteLine($"Received {assessments.Count} assessments");
            }
            else
            {
                Console.WriteLine("No assessments received");
            }
            return Task.CompletedTask;
        }

        public Task receiveAssessment(Assessment assessment)
        {
            if (assessment != null)
            {
                Console.WriteLine("Received assessment");
            }
            else
            {
                Console.WriteLine("No assessment received");
            }
            return Task.CompletedTask;
        }

        public Task receiveInsertStatus(bool success)
        {
            if (success)
            {
                Console.WriteLine("Inserted assessment successfully");
            }
            else
            {
                Console.WriteLine("Failed to insert assessment");
            }
            return Task.CompletedTask;
        }

        public Task receiveUpdateStatus(bool success)
        {
            if (success)
            {
                Console.WriteLine("Updated assessment successfully");
            }
            else
            {
                Console.WriteLine("Failed to update assessment");
            }
            return Task.CompletedTask;
        }

        public Task receiveDeleteStatus(bool success)
        {
            if (success)
            {
                Console.WriteLine("Deleted assessment successfully");
            }
            else
            {
                Console.WriteLine("Failed to delete assessment");
            }
            return Task.CompletedTask;
        }

        public async Task<string> insertAssessment(string hazardType, string doctorId, List<string> imagePath)
        {
            
            return await _gateway.insertAssessment(
                hazardType: hazardType,
                doctorId: doctorId,
                imagePath: imagePath,
                riskLevel: "Medium", // Default value
                recommendation: "No recommendation", // Default value
                createdAt: DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
            );
        }

        public async Task<bool> updateAssessment(string id, string doctorId, List<string> imagePath)
        {
            var assessment = await _gateway.fetchAssessmentById(id);
            if (assessment == null) return false;

            return await _gateway.updateAssessment(
                id: id,
                doctorId: doctorId,
                imagePath: imagePath,
                riskLevel: assessment.getRiskLevel(),
                recommendation: assessment.getRecommendation(),
                createdAt: assessment.getCreatedAt().ToString("yyyy-MM-dd")
            );
        }
        
        public async Task<List<Assessment>> getAssessments()
        {
            return await _gateway.fetchAssessments();
        }

        public async Task<Assessment> getAssessment(string id)
        {
            return await _gateway.fetchAssessmentById(id);
        }

        public async Task<bool> deleteAssessment(string id)
        {
            return await _gateway.deleteAssessment(id);
        }

        public async Task<bool> updateDoctorAssessment(string id, string riskLevel, string recommendation, Dictionary<string, bool> checklist)
        {
            var assessment = await _gateway.fetchAssessmentById(id);
            if (assessment == null) return false;

            return await _gateway.updateAssessment(
                id: id,
                doctorId: assessment.getDoctorId(),
                imagePath: assessment.getImagePath(),
                riskLevel: riskLevel,
                recommendation: recommendation,
                createdAt: assessment.getCreatedAt().ToString("yyyy-MM-dd"),
                checklist: checklist
            );
        }
    }
}