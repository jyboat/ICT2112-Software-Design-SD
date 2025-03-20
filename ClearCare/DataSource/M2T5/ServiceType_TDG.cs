using System.Collections.Generic;
using System.Linq;
using ClearCare.Models.Entities;

namespace ClearCare.DataSource
{
    public class ServiceType_TDG
    {
        private static List<ServiceType_SDM> serviceTypes = new List<ServiceType_SDM>();

        public List<ServiceType_SDM> FetchServiceTypes()
        {
            return serviceTypes;
        }

        public void CreateServiceType(ServiceType_SDM type)
        {
            serviceTypes.Add(type);
        }

        public void UpdateServiceType(ServiceType_SDM type)
        {
            var existing = serviceTypes.FirstOrDefault(st => st.ServiceTypeId == type.ServiceTypeId);
            if (existing != null)
            {
                existing.Name = type.Name;
                existing.Duration = type.Duration;
                existing.Requirements = type.Requirements;
            }
        }

        public void DeleteServiceType(int serviceTypeId)
        {
            var type = serviceTypes.FirstOrDefault(st => st.ServiceTypeId == serviceTypeId);
            if (type != null)
            {
                serviceTypes.Remove(type);
            }
        }
    }
}
