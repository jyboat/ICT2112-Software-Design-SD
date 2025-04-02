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

        // Public constructor for DI
        public PrescriptionControl(PrescriptionMapper mapper)
        {
            _mapper = mapper;
        }

        // Interface method to fetch all prescriptions
        public async Task<List<PrescriptionModel>> fetchPrescriptions()
        {
            return await _mapper.getAllPrescriptionsAsync();
        }

        // Match UML method for saving a medication plan
        public async Task createPrescription(string medicationPlan)
        {
            await _mapper.savePrescriptionsAsync(medicationPlan);
        }

        public string checkDrugInteractions(string drugName)
        {
            return $"No known interactions for {drugName}.";
        }


        public async Task fetchPrescriptions(string userId)
        {
            await _mapper.fetchSharedPrescriptionsAsync(userId);
        }

        public async Task<List<PrescriptionModel>> getAllPrescriptionsAsync(string userRole, string userUUID)
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

        public async Task addPrescriptionAsync(PrescriptionModel model)
        {
            model.DateIssued = DateTime.SpecifyKind(model.DateIssued, DateTimeKind.Utc);
            await _mapper.addPrescriptionAsync(model);
        }

        Task<List<PrescriptionModel>> IFetchPrescriptions.fetchPrescriptionsPatientId(string userId)
        {
            return (Task<List<PrescriptionModel>>)_mapper.fetchSharedPrescriptionsAsync(userId);
        }

    }
}
