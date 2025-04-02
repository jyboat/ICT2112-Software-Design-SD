using ClearCare.Models;

namespace ClearCare.Interfaces
{
    public interface IFetchPrescriptions
    {
        /// <summary>
        ///   Fetches all prescriptions.
        /// </summary>
        /// <returns>
        ///   A task representing the asynchronous operation that returns a list
        ///   of <see cref="PrescriptionModel"/>.
        /// </returns>
        Task<List<PrescriptionModel>> fetchPrescriptions();

        /// <summary>
        ///   Fetches prescriptions for a specific patient ID.
        /// </summary>
        /// <param name="userId">The ID of the patient.</param>
        /// <returns>
        ///   A task representing the asynchronous operation that returns a list
        ///   of <see cref="PrescriptionModel"/> for the specified patient.
        /// </returns>
        Task<List<PrescriptionModel>> fetchPrescriptionsPatientId(string userId);
    }
}
