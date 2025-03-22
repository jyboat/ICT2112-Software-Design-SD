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

        public AssessmentManager(IAssessmentSend gateway)
        {
            _gateway = gateway;
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

         public async Task<string> insertAssessment(string riskLevel, string recommendation, string createdAt, string patientId, string doctorId, List<string> imagePath)
        {
            return await _gateway.insertAssessment(riskLevel, recommendation, createdAt, patientId, doctorId, imagePath);
        }
         public async Task<bool> updateAssessment(string id, string riskLevel, string recommendation, string createdAt, List<string> imagePath)
         {
             return await _gateway.updateAssessment(id, riskLevel, recommendation, createdAt, imagePath);
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
    }
}
