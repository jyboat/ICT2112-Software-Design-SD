using ClearCare.Controllers;
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
        private ServiceBacklogController? _controller;
        
        public ServiceBacklogManagement()
        {
            _dbGateway = new ServiceBacklogGateway(this);
        }
        public void setController(ServiceBacklogController controller)
        {
            _controller = controller;
        }

        // Get all backlogs including details of each
        public async Task<List<ServiceBacklogViewModel>> getBacklogs()
        {
            List<Dictionary<string, string>> backlogsDTO = await _dbGateway.fetchServiceBacklogs();
            var serviceBacklogs = new List<ServiceBacklog>();

            // get all backlogs
            foreach (var backlogDTO in backlogsDTO)
            {
                var serviceBacklog = new ServiceBacklog();
                serviceBacklog.setBacklogInformation(backlogDTO["backlogId"], backlogDTO["appointmentId"]);
                serviceBacklogs.Add(serviceBacklog);
            }

            // Get all services
            var serviceManager = new ServiceAppointmentManagement();
            var allAppointments = await serviceManager.RetrieveAllAppointments();

            // Combine data into ServiceBacklogViewModel
            var serviceBacklogViewModels = new List<ServiceBacklogViewModel>();
            foreach (var serviceBacklog in serviceBacklogs)
            {
                var appointment = allAppointments.FirstOrDefault(a => (string)a["AppointmentId"] == serviceBacklog.getBacklogInformation()["appointmentId"]);
                if (appointment != null)
                {
                    var viewModel = await createViewModel(serviceBacklog, appointment);
                    serviceBacklogViewModels.Add(viewModel);
                }
            }

            return serviceBacklogViewModels;
        }

        public List<ServiceBacklog> getBacklogsByDate()
        {
            List<ServiceBacklog> backlogList = new List<ServiceBacklog>();

            // call DSL to populate sorted list

            return backlogList;
        }

        // Get single backlog details
        public async Task<ServiceBacklogViewModel> getBacklogDetails(string backlogId)
        {
            Dictionary<string, string> backlogDTO = await _dbGateway.fetchServiceBacklogById(backlogId);
            
            ServiceBacklog serviceBacklog = new ServiceBacklog();
            serviceBacklog.setBacklogInformation(backlogDTO["backlogId"], backlogDTO["appointmentId"]);

            var serviceManager = new ServiceAppointmentManagement();
            var appointment = await serviceManager.GetAppt(serviceBacklog.getBacklogInformation()["appointmentId"]);

            var serviceBacklogViewModel = await createViewModel(serviceBacklog, appointment);

            return serviceBacklogViewModel;
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

        public async Task deleteBacklog(string backlogId)
        {
            await _dbGateway.deleteServiceBacklog(backlogId);
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
                        Console.Write($"{kvp.Key}: {kvp.Value}, ");
                    }
                    Console.WriteLine("");
                }
            }

            return Task.CompletedTask;
        }

        public Task receiveBacklogDetails(Dictionary<string, string> serviceBacklog)
        {
            if (serviceBacklog == null || serviceBacklog.Count == 0)
            {
            Console.WriteLine("No backlog details found.");
            }
            else
            {
            Console.WriteLine("Received backlog details:");
            foreach (var kvp in serviceBacklog)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
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
    

        private Task<ServiceBacklogViewModel> createViewModel(ServiceBacklog serviceBacklog, Dictionary<string, object> appointment)
        {
            return Task.FromResult(new ServiceBacklogViewModel {
                BacklogId = serviceBacklog.getBacklogInformation()["backlogId"],
                AppointmentId = serviceBacklog.getBacklogInformation()["appointmentId"],
                DateTime = (DateTime)appointment["DateTime"],
                DateTimeFormatted = ((DateTime)appointment["DateTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                PatientId = (string)appointment["PatientId"],
                DoctorId = (string)appointment["DoctorId"],
                NurseId = (string)appointment["NurseId"],
                ServiceType = (string)appointment["ServiceTypeId"]
            });
        }
    }
}
