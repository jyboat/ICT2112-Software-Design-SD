// defines the callback methods for receiving results or status updates from the database operations
// implemented by NurseAvailabilityManagement; used by NurseAvailabilityGateway

using System.Collections.Generic;
using ClearCare.Models.Entities;
using System.Threading.Tasks;

namespace ClearCare.Interfaces
{
    public interface IServiceBacklogDB_Receive
    {
        Task receiveBacklogList(List<Dictionary<string, string>> backlogList);
        Task receiveBacklogDetails(Dictionary<string, string> serviceBacklog);
        Task receiveAddStatus(string status);

        Task receiveDeleteStatus(string status);
    }
}
