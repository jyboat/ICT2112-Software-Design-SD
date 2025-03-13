using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;

namespace ClearCare.Models.Interface
{
    public interface IMedicalRecord
    {
        Task<List<dynamic>> getAllMedicalRecords();
        Task<dynamic> getAdjustedRecordByID(string recordID);
    }
}

