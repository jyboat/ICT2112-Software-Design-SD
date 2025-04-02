using ClearCare.Models.Entities.M3T2;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Models.Interfaces.M3T2
{
    public interface IFetchPrescriptions
    {
        Task<List<PrescriptionModel>> fetchPrescriptions();
    }
}
