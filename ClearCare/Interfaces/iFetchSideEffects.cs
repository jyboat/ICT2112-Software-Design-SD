using ClearCare.Models;

namespace ClearCare.Interfaces
{
    public interface IFetchSideEffects
    {
        /// <summary>
        ///   Fetches the side effects for a given drug name.
        /// </summary>
        /// <param name="drugName">The name of the drug to fetch side effects
        ///   for.</param>
        /// <returns>
        ///   A task representing the asynchronous operation that returns a
        ///   string containing the side effects.
        /// </returns>
        Task<string> fetchDrugSideEffect(string drugName);
    }
}
