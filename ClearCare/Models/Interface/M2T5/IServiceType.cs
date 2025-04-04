using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities;

namespace ClearCare.Models.Interface
{
public interface IServiceType
{
    Task<List<ServiceType>> getServiceTypes();
    Task createServiceType(string name, int duration, string requirements, string modality);
    Task updateServiceType(int id, string name, int duration, string requirements, string modality);
    Task discontinueServiceType(int id);
}

}
