using ClearCare.DataSource;
using ClearCare.Interfaces;
using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Models.Control
{
    public class ServiceBacklogManagement: IServiceBacklogDB_Receive
    {
        private readonly IServiceBacklogDB_Send _dbGateway;

        public ServiceBacklogManagement()
        {
            _dbGateway = new ServiceBacklogGateway(this);
        }

        public void getBacklogs()
        {
            // List<ServiceBacklog> backlogList = new List<ServiceBacklog>();

            _dbGateway.fetchServiceBacklogs();
        }

        public List<ServiceBacklog> getBacklogsByDate()
        {
            List<ServiceBacklog> backlogList = new List<ServiceBacklog>();
            
            // call DSL to populate sorted list

            return backlogList;
        }

        public Dictionary<string, object> getBacklogDetails(string backlogId)
        {            
            return new Dictionary<string, object>();
        }

        public bool reassignBacklog()
        {
            // if there was an error / cannot schedule, return false

            // if assignment successful
            return true;
        }

        public void addBacklog(string serviceAppointmentId)
        {
            ServiceBacklog backlog = new ServiceBacklog(serviceAppointmentId);
            _dbGateway.createServiceBacklog(backlog);
        }

        public void deleteBacklog(string backlogId)
        {
            // send to DSL
        }

        public Task receiveBacklogList(List<Dictionary<string, string>> backlogList)
        {
            if (backlogList.Count == 0)
            {
                Console.WriteLine("No backlogs found.");
            }
            else
            {
                Console.WriteLine($"Received {backlogList.Count} backlogs.");
                
                foreach (var backlog in backlogList)
                {
                    foreach (var kvp in backlog)
                    {
                        Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                    }
                }
            }
            return Task.CompletedTask;
        }


        public Task receiveAddStatus(string status)
        {
            if (status == "Success")
            {
                Console.WriteLine("Backlog added successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to add backlog: {status}");
            }
            return Task.CompletedTask;
        }

        public Task receiveDeleteStatus(string status)
        {
            if (status == "Success")
            {
                Console.WriteLine("Backlog deleted successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to delete backlog: {status}");
            }
            return Task.CompletedTask;
        }
    }
}
