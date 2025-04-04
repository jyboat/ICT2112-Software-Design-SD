using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;

namespace ClearCare.Models.Interface
{
    public interface IViewPersonalDatabase
    {
        Task<List<MedicalRecord>> findMedicalRecordsByUserID(string userID);
    }
}
