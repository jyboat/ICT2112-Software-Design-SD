using ClearCare.Controllers;
using ClearCare.DataSource;
using ClearCare.Interfaces;
using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Models.Control
{
    public class ServiceBacklogManagement: IServiceBacklogDB_Receive, ISchedulingListener
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

        private async Task<List<ServiceBacklog>> getAllBacklogs()
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

            return serviceBacklogs;
        }

        // Get all backlogs including details of each service
        public async Task<List<ServiceBacklogViewModel>> getAllBacklogDetails()
        {
            // get all backlogs
            List<ServiceBacklog> serviceBacklogs = await getAllBacklogs();
            // Get all services
            var allAppointments = await new ServiceAppointmentManagement().retrieveAllAppointments();

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

            var appointment = await  new ServiceAppointmentManagement().getAppointmentByID(serviceBacklog.getBacklogInformation()["appointmentId"]);

            var serviceBacklogViewModel = await createViewModel(serviceBacklog, appointment);

            return serviceBacklogViewModel;
        }

        public bool reassignBacklog(ServiceBacklogViewModel viewModel)
        {
            try
            {
                // Print the view model data
                Console.WriteLine("Reassigning Backlog:");
                Console.WriteLine($"BacklogId: {viewModel.BacklogId}");
                Console.WriteLine($"AppointmentId: {viewModel.AppointmentId}");
                Console.WriteLine($"DateTime: {viewModel.DateTime}");
                Console.WriteLine($"PatientId: {viewModel.PatientId}");
                Console.WriteLine($"DoctorId: {viewModel.DoctorId}");
                Console.WriteLine($"NurseId: {viewModel.NurseId}");
                Console.WriteLine($"ServiceType: {viewModel.ServiceType}");

                // Perform reassignment logic here
                // For example, update the database or call another service

                // If reassignment fails, return false
                // return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reassigning backlog: {ex.Message}");
                return false;
            }
            // if there was an error / cannot schedule, return false

            // if assignment successful
            return true;
        }

        public async Task addBacklog(string serviceAppointmentId)
        {
            ServiceBacklog backlog = new ServiceBacklog(serviceAppointmentId);
            await _dbGateway.createServiceBacklog(backlog);
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
                
                // foreach (var backlog in backlogList)
                // {
                //     foreach (var kvp in backlog)
                //     {
                //         Console.Write($"{kvp.Key}: {kvp.Value}, ");
                //     }
                //     Console.WriteLine("");
                // }
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
                // DateTimeFormatted = ((DateTime)appointment["DateTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                PatientId = (string)appointment["PatientId"],
                DoctorId = (string)appointment["DoctorId"],
                NurseId = (string)appointment["NurseId"],
                ServiceType = (string)appointment["ServiceTypeId"]
            });
        }

        public async Task update(string appointmentID, string eventType)
        {
            if (eventType == "success")
            {
                // get all backlogs
                List<ServiceBacklog> allBacklogs = await getAllBacklogs();

                
                var backlog = allBacklogs.FirstOrDefault(b => b.getBacklogInformation()["appointmentId"] == appointmentID);
                if (backlog != null)
                {
                    // remove the backlog if it exists
                    await deleteBacklog(backlog.getBacklogInformation()["backlogId"]);
                    Console.WriteLine($"Backlog with appointment ID {appointmentID} has been removed.");
                }
            }
            else if (eventType == "fail")
            {
                // get all backlogs
                List<ServiceBacklog> allBacklogs = await getAllBacklogs();

                // if backlog doesn't already exist
                var backlog = allBacklogs.FirstOrDefault(b => b.getBacklogInformation()["appointmentId"] == appointmentID);
                if (backlog == null)
                {
                    // add backlog
                    await addBacklog(appointmentID);
                }
            }
        }
    }
}
