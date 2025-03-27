using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;

namespace ClearCare.Models.Control
{
    public class ServiceTypeManager
    {
        private ServiceTypeRepository _serviceTypeRepository = new ServiceTypeRepository();

        public async Task<List<ServiceType_SDM>> GetServiceTypes()
        {
            return await _serviceTypeRepository.GetServiceTypes();
        }

        public async Task CreateServiceType(string name, int duration, string requirements, string modality)
        {
            var existing = await _serviceTypeRepository.GetServiceTypes();
            var newId = existing.Count + 1;
            var newService = new ServiceType_SDM(newId, name, duration, requirements, modality);
            await _serviceTypeRepository.AddServiceType(newService);
        }

        public async Task UpdateServiceType(int id, string name, int duration, string requirements, string modality)
        {
            var updated = new ServiceType_SDM(id, name, duration, requirements, modality);
            await _serviceTypeRepository.UpdateServiceType(id, updated);
        }


        public async Task DiscontinueServiceType(int id)
        {
            // TODO: Check appointments before continuing
            await _serviceTypeRepository.DiscontinueServiceType(id);
        }

    }
}
