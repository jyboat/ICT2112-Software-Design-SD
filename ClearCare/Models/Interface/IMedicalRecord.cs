using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;

namespace ClearCare.Models.Interface
{
    public interface IMedicalRecord
    {
        Task<List<dynamic>> GetAllMedicalRecords();
        Task<dynamic> GetMedicalRecordByID(string recordID);
    }
}

