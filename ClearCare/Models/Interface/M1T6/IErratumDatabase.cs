using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;

namespace ClearCare.Models.Interface
{
    public interface IErratumDatabase
    {
        Task<List<Erratum>> retrieveAllErratums();

        Task<Erratum?> getErratumByID(string erratumID);

        Task<Erratum?> insertErratum(string medicalRecordID, string erratumDetails, string doctorID, byte[] fileBytes, string fileName);
    }
}

