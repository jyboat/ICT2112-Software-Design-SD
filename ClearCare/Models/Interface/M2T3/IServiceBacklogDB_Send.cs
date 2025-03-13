// defines the methods for sending (or initiating) database operations such as fetching, creating, updating, and deleting nurse availabilities.
// implemented by NurseAvailabilityGateway; used by NurseAvailabilityManagement 

using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Interfaces
{
    public interface IServiceBacklogDB_Send
    {
        public Task createServiceBacklog(ServiceBacklog backlog);

        public Task fetchServiceBacklogs();
        public Task deleteServiceBacklog(int backlogId);
    }
}
