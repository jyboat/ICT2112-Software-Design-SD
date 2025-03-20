using ClearCare.Models;


namespace ClearCare.Interfaces
{
    public interface IFetchPrescriptions
    {
        Task<List<PrescriptionModel>> FetchPrescriptions();
    }
}
