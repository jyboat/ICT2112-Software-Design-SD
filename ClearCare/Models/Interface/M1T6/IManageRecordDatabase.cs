using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;

namespace ClearCare.Models.Interface
{
    public interface IManageRecordDatabase
    {
        Task<MedicalRecord> insertMedicalRecord(string doctorNote, string patientID, byte[] fileBytes, string fileName, string doctorID);
    }
}

