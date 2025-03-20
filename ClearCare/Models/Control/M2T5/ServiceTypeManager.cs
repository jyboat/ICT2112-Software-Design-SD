using System.Collections.Generic;
using ClearCare.Models.Entities;
using ClearCare.DataSource;

namespace ClearCare.Models.Control
{
    public class ServiceTypeManager
    {
        private ServiceType_TDG serviceTypeTdg = new ServiceType_TDG();

        public List<ServiceType_SDM> GetServiceTypes()
        {
            return serviceTypeTdg.FetchServiceTypes();
        }

        public void CreateServiceType(string name, int duration, string requirements)
        {
            var newService = new ServiceType_SDM(serviceTypeTdg.FetchServiceTypes().Count + 1, name, duration, requirements);
            serviceTypeTdg.CreateServiceType(newService);
        }

        public void UpdateServiceType(int id, string name, int duration, string requirements)
        {
            var updatedService = new ServiceType_SDM(id, name, duration, requirements);
            serviceTypeTdg.UpdateServiceType(updatedService);
        }

        public void DeleteServiceType(int id)
        {
            serviceTypeTdg.DeleteServiceType(id);
        }
    }
}
