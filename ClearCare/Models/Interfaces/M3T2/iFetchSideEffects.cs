using ClearCare.Models;

namespace ClearCare.Models.Interfaces.M3T2
{
    public interface IFetchSideEffects
    {
        Task<string> fetchDrugSideEffect(string drugName);
    }
}
