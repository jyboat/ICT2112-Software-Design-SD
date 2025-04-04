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
            Console.WriteLine("ServiceTypeManager: Observer triggered! Data received from Repository.");

            if (data is List<ServiceType> serviceTypes)
            {
                Console.WriteLine($"ServiceTypeManager: Received {serviceTypes.Count} service types.");
                _cachedServiceTypes = serviceTypes;
            }
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

        public async Task<List<ServiceType>> getServiceTypes()
        {
            await fetchServiceTypes(); // trigger async update and populate cache
            return _cachedServiceTypes;
        }

        public async Task createServiceType(string name, int duration, string requirements, string modality)
        {
            await fetchServiceTypes();
            var existing = _cachedServiceTypes;

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

        public async Task<List<string>> getUpcomingAppointmentIdsAsync(string serviceName)
        {
            var allAppointments = await new ServiceAppointmentStatusManagement().getAppointmentDetails();

            var upcomingApptIds = allAppointments
                .Where(appt => appt.getAttribute("Service") == serviceName)
                .Select(appt => appt.getAttribute("AppointmentId"))
                .ToList();

            return upcomingApptIds;
        }
    }
}
