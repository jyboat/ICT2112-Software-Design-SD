// defines the methods for sending (or initiating) database operations such as fetching, creating, updating, and deleting nurse availabilities.
// implemented by NurseAvailabilityGateway; used by NurseAvailabilityManagement 

using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Interfaces
{
    public interface IServiceBacklogDB_Send
    {
        Task<List<Dictionary<string,string>>> fetchServiceBacklogs();
        Task<Dictionary<string,string>> fetchServiceBacklogById(string backlogId);
        Task createServiceBacklog(ServiceBacklog backlog);

        Task deleteServiceBacklog(string backlogId);
    }
}
