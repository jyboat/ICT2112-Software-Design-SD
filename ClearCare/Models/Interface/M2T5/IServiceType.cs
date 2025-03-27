using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities;

namespace ClearCare.Models.Interface
{
    public interface IServiceType
    {
        Task<List<ServiceType_SDM>> GetServiceTypes();
        Task CreateServiceType(string name, int duration, string requirements);
        Task UpdateServiceType(int id, string name, int duration, string requirements);
        Task DiscontinueServiceType(int id);
    }
}
