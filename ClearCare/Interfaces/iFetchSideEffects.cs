using ClearCare.Models;

namespace ClearCare.Interfaces
{
    public interface IFetchSideEffects
    {
        Task<string> fetchDrugSideEffect(string drugName);
    }
}
