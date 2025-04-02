using ClearCare.Models;

namespace ClearCare.Interfaces
{
    public interface IFetchPrescriptions
    {
        Task<List<PrescriptionModel>> fetchPrescriptions();

        Task<List<PrescriptionModel>> fetchPrescriptionsPatientId(string userId);
    }
}
