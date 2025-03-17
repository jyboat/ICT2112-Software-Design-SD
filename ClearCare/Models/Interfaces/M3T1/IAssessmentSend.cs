using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface IAssessmentSend
    {
        Task<List<Assessment>> fetchAssessments();

        Task<Assessment> fetchAssessmentById(string id);

        Task<string> insertAssessment(string riskLevel, string recommendation, string createdAt, string patientId, string doctorId, List<string> imagePath);

        Task<bool> updateAssessment(string id, string riskLevel, string recommendation, string createdAt, List<string> imagePath);
        
        Task<bool> deleteAssessment(string id);
    }
}
