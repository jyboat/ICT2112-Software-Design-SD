using ClearCare.Models.Entities.M3T1;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface IResourceReceive
    {
        Task receiveResources(List<Resource> resources);
        Task receiveResource(Resource resource);
        Task receiveInsertStatus(bool success);
        Task receiveUpdateStatus(bool success);
        Task receiveDeleteStatus(bool success);
    }
}
