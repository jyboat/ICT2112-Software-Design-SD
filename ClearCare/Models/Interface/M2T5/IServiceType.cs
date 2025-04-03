using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities;

namespace ClearCare.Models.Interface
{
public interface IServiceType
{
    Task<List<ServiceType_SDM>> getServiceTypes();
    Task CreateServiceType(string name, int duration, string requirements, string modality);
    Task UpdateServiceType(int id, string name, int duration, string requirements, string modality);
    Task DiscontinueServiceType(int id);
}

}
