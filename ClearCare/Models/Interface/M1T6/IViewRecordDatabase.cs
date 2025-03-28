using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;

namespace ClearCare.Models.Interface
{
    public interface IViewRecordDatabase
    {
        Task<List<MedicalRecord>> retrieveAllMedicalRecords();
        Task<MedicalRecord> retrieveMedicalRecordById(string recordID);
    }
}

