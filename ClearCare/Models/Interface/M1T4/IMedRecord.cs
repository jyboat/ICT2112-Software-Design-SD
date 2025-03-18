using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;

namespace ClearCare.Models.Interface
{
    public interface IMedRecord
    {
        Task<List<dynamic>> getMedicalRecord(string patientID);
    }
}
