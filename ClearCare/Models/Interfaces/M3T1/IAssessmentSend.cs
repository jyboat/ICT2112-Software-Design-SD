using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface IAssessmentSend
    {
        Task<List<Assessment>> fetchAssessments();
        Task<Assessment> fetchAssessmentById(string id);
        
        Task<string> insertAssessment(string hazardType, string doctorId, List<string> imagePath,string riskLevel, string recommendation, string createdAt);
        
        Task<bool> updateAssessment(string id, string doctorId, List<string> imagePath, string riskLevel, string recommendation, string createdAt,Dictionary<string, bool> checklist = null);
            
        Task<bool> deleteAssessment(string id);
    }
}