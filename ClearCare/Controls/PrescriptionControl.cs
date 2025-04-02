using ClearCare.Interfaces;
using ClearCare.Models;
using ClearCare.Gateways;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Controls
{
    // Marked as Singleton to match your UML
    public class PrescriptionControl : IFetchPrescriptions
    {
        // Mapper to talk to Firestore
        private readonly PrescriptionMapper _mapper;

        /// <summary>
        ///   Initializes a new instance of the
        ///   <see cref="PrescriptionControl"/> class.
        /// </summary>
        /// <param name="mapper">The PrescriptionMapper instance for data
        ///   access.</param>
        // Public constructor for DI
        public PrescriptionControl(PrescriptionMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        ///   Fetches all prescriptions.
        /// </summary>
        /// <returns>A list of <see cref="PrescriptionModel"/> representing all
        ///   prescriptions.</returns>
        // Interface method to fetch all prescriptions
        public async Task<List<PrescriptionModel>> fetchPrescriptions()
        {
            return await _mapper.getAllPrescriptionsAsync();
        }

        /// <summary>
        ///   Creates a new prescription with the given medication plan.
        /// </summary>
        /// <param name="medicationPlan">The medication plan to save.</param>
        // Match UML method for saving a medication plan
        public async Task createPrescription(string medicationPlan)
        {
            await _mapper.savePrescriptionsAsync(medicationPlan);
        }

        /// <summary>
        ///   Checks for drug interactions for a given drug name.  Currently,
        ///   this method always returns "No known interactions".
        /// </summary>
        /// <param name="drugName">The name of the drug to check for
        ///   interactions.</param>
        /// <returns>A string indicating that no known interactions were
        ///   found.</returns>
        public string checkDrugInteractions(string drugName)
        {
            return $"No known interactions for {drugName}.";
        }

        /// <summary>
        ///   Fetches prescriptions for a given user ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        public async Task fetchPrescriptions(string userId)
        {
            await _mapper.fetchSharedPrescriptionsAsync(userId);
        }

        /// <summary>
        ///   Retrieves all prescriptions based on the user's role (Patient or
        ///   Doctor) and UUID.
        /// </summary>
        /// <param name="userRole">The role of the user (Patient or Doctor).</param>
        /// <param name="userUUID">The UUID of the user.</param>
        /// <returns>
        ///   A list of <see cref="PrescriptionModel"/> based on the user's
        ///   role.
        /// </returns>
        public async Task<List<PrescriptionModel>> getAllPrescriptionsAsync(
            string userRole,
            string userUUID
        )
        {
            var allPrescription = await _mapper.getAllPrescriptionsAsync();

            if (userRole == "Patient")
            {
                return allPrescription.Where(se => se.PatientId == userUUID).ToList();
            }
            else if (userRole == "Doctor")
            {
                return allPrescription.Where(se => se.DoctorId == userUUID).ToList();
            }

            return allPrescription;

            // return await _mapper.getAllPrescriptionsAsync();
        }

        /// <summary>
        ///   Adds a new prescription to the system.
        /// </summary>
        /// <param name="model">The <see cref="PrescriptionModel"/> containing
        ///   the prescription data.</param>
        public async Task addPrescriptionAsync(PrescriptionModel model)
        {
            model.DateIssued = DateTime.SpecifyKind(
                model.DateIssued,
                DateTimeKind.Utc
            );
            await _mapper.addPrescriptionAsync(model);
        }

        /// <summary>
        ///   Fetches prescriptions for a given patient ID (explicit interface
        ///   implementation).
        /// </summary>
        /// <param name="userId">The ID of the patient.</param>
        /// <returns>
        ///   A task representing the asynchronous operation that returns a list
        ///   of <see cref="PrescriptionModel"/>.
        /// </returns>
        Task<List<PrescriptionModel>> IFetchPrescriptions.fetchPrescriptionsPatientId(
            string userId
        )
        {
            return (Task<List<PrescriptionModel>>)_mapper.fetchSharedPrescriptionsAsync(
                userId
            );
        }
    }
}
