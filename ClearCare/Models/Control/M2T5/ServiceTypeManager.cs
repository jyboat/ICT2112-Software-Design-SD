using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.Firebase;

namespace ClearCare.Models.Control
{
    public class ServiceTypeManager
    {
        private ServiceTypeRepository _serviceTypeRepository = new ServiceTypeRepository();

        public async Task<List<ServiceType_SDM>> GetServiceTypes()
        {
            return await _serviceTypeRepository.GetServiceTypes();
        }

        public async Task CreateServiceType(string name, int duration, string requirements)
        {
            List<ServiceType_SDM> existingServices = await _serviceTypeRepository.GetServiceTypes();
            int newId = existingServices.Count + 1; // Auto-increment ID
            ServiceType_SDM newService = new ServiceType_SDM(newId, name, duration, requirements);
            await _serviceTypeRepository.AddServiceType(newService);
        }

        public async Task UpdateServiceType(int id, string name, int duration, string requirements)
        {
            ServiceType_SDM updatedService = new ServiceType_SDM(id, name, duration, requirements);
            await _serviceTypeRepository.UpdateServiceType(id, updatedService);
        }

        public async Task DeleteServiceType(int id)
        {
            await _serviceTypeRepository.DeleteServiceType(id);
        }
    }
}
