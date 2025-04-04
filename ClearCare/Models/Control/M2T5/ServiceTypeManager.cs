using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using ClearCare.Models.Interface;
using ClearCare.Interfaces;

namespace ClearCare.Models.Control
{
    public class ServiceTypeManager: IServiceType, IDatabaseObserver
    {
        private List<ServiceType> _cachedServiceTypes = new List<ServiceType>();

        public ServiceTypeManager()
        {
            _serviceTypeRepository = new ServiceTypeRepository();
            _serviceTypeRepository.attachObserver(this);
        }
        public void update(Subject subject, object data)
        {
            Console.WriteLine("✅ Observer triggered! Data received from repository.");

            if (data is List<ServiceType> serviceTypes)
            {
                Console.WriteLine($"✅ Received {serviceTypes.Count} service types.");
                processServiceTypes(serviceTypes);
            }
        }
        private void processServiceTypes(List<ServiceType> types)
        {
            Console.WriteLine("🔧 Processing service types inside manager.");
            _cachedServiceTypes = types;
        }

        private ServiceTypeRepository _serviceTypeRepository = new ServiceTypeRepository();

        public async Task fetchServiceTypes()
        {
            await _serviceTypeRepository.getServiceTypesAsync();
        }

        public List<ServiceType> getCachedServiceTypes()
        {
            return _cachedServiceTypes;
        }

        public Task<List<ServiceType>> getServiceTypes()
        {
            return Task.FromResult(getCachedServiceTypes());
        }

        public async Task createServiceType(string name, int duration, string requirements, string modality)
        {
            await fetchServiceTypes();
            var existing = getCachedServiceTypes();

            var newId = existing.Count + 1;
            var newService = new ServiceType(newId, name, duration, requirements, modality);
            await _serviceTypeRepository.addServiceType(newService);
        }

        public async Task updateServiceType(int id, string name, int duration, string requirements, string modality)
        {
            ServiceType updatedService = new ServiceType(id, name, duration, requirements, modality);
            updatedService.Modality = modality;
            await _serviceTypeRepository.updateServiceType(id, updatedService);
        }



        public async Task discontinueServiceType(int id)
        {
            // TODO: Check appointments before continuing
            await _serviceTypeRepository.discontinueServiceType(id);
        }

    }
}
