using ClearCare.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Interfaces
{
    public interface IFetchPrescriptions
    {
        Task<List<PrescriptionModel>> FetchPrescriptions();
    }
}
