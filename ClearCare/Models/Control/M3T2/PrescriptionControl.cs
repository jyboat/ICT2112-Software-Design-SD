using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.DataSource.M3T2;
using ClearCare.Models.Interfaces.M3T2;
using ClearCare.Models.Entities.M3T2;
using ClearCare.Gateways;

namespace ClearCare.Models.Control.M3T2
{
    // Mark as Singleton to match your UML
    public class PrescriptionControl : IFetchPrescriptions
    {
        // single instance (per UML's "singleton" mention)
        private static PrescriptionControl _instance;

        // the mapper to talk to Firestore
        private readonly PrescriptionMapper _mapper;

        // This must be public for DI to call it
        public PrescriptionControl(PrescriptionMapper mapper)
        {
            _mapper = mapper;
        }



        // The interface method to fetch all prescriptions
        public async Task<List<PrescriptionModel>> FetchPrescriptions()
        {
            return await _mapper.GetAllPrescriptionsAsync();
        }

        // If you want to match your diagram exactly, you can add:
        public async Task CreatePrescription(string medicationPlan)
        {
            // you might parse the plan & store it, or call the mapper
            await _mapper.SavePrescriptionsAsync(medicationPlan);
        }

        public string CheckDrugInteractions(string drugName)
        {
            // put real logic here
            return $"No known interactions for {drugName}.";
        }

        public async Task SharePrescription(string email)
        {
            // e.g., share logic or store in a 'SharedPrescriptions' collection
        }

        public async Task FetchPrescriptions(string userId)
        {
            // fetch only shared or user-specific prescriptions, if needed
            await _mapper.FetchSharedPrescriptionsAsync(userId);
        }

        public async Task<List<PrescriptionModel>> GetAllPrescriptionsAsync()
        {
            return await _mapper.GetAllPrescriptionsAsync();
        }

        public async Task AddPrescriptionAsync(PrescriptionModel model)
        {
            // For example, fix any DateTime issues or run validations:
            model.DateIssued = DateTime.SpecifyKind(model.DateIssued, DateTimeKind.Utc);

            // Then delegate to the mapper:
            await _mapper.AddPrescriptionAsync(model);
        }
    }
}
