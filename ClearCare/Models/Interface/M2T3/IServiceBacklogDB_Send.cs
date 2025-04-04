using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Interfaces
{
    public interface IServiceBacklogDB_Send
    {
        Task createServiceBacklog(ServiceBacklog backlog);
        Task<List<Dictionary<string, string>>> fetchServiceBacklogs();
        Task<Dictionary<string, string>> fetchServiceBacklogById(string backlogId);
        Task<bool> deleteServiceBacklog(string backlogId);
    }
}