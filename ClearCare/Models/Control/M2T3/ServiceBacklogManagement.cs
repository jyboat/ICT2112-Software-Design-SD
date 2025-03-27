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

            var serviceBacklogViewModels = new List<ServiceBacklogViewModel>();
            var serviceAppointmentManagement = new ServiceAppointmentManagement();

            // Get details for each service backlog
            foreach (var serviceBacklog in serviceBacklogs)
            {
                var appointmentId = serviceBacklog.getBacklogInformation()["appointmentId"];
                var appointment = await serviceAppointmentManagement.getAppointmentByID(appointmentId);
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

        public async Task<bool> reassignBacklog(
            string BacklogId,
            string AppointmentId,
            string PatientId,
            string DoctorId,
            string ServiceType,
            string NurseId,
            DateTime DateTime,
            int Slot,
            string Location
        )
        {
            try
            {
                // TODO change to manual scheduler's method once it's up
                var svcMgr = new ServiceAppointmentManagement();
                var svc = await svcMgr.getAppointmentByID(AppointmentId);
                svc.updateServiceAppointementById(
                    appointment:svc,
                    patientId: PatientId,
                    nurseId: NurseId,
                    doctorId: DoctorId,
                    serviceTypeId: ServiceType,
                    status: "Scheduled",
                    dateTime: DateTime.ToUniversalTime(),
                    slot: Slot,
                    location: Location
                );
                bool updateSuccess = await svcMgr.UpdateAppointment(svc);


                // Delete service backlog if updating is successful
                bool deleteSuccess = false;
                if (updateSuccess)
                {
                    deleteSuccess = await _dbGateway.deleteServiceBacklog(BacklogId);
                }
                if (updateSuccess && deleteSuccess)
                {
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reassigning backlog: {ex.Message}");
                return false;
            }

        }

        public async Task addBacklog(string serviceAppointmentId)
        {
            ServiceBacklog backlog = new ServiceBacklog(serviceAppointmentId);
            await _dbGateway.createServiceBacklog(backlog);
        }

        public async Task<bool> deleteBacklog(string backlogId)
        {
            await _dbGateway.deleteServiceBacklog(backlogId);
            return true;
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
    

        private Task<ServiceBacklogViewModel> createViewModel(ServiceBacklog serviceBacklog, ServiceAppointment appointment)
        {
            return Task.FromResult(new ServiceBacklogViewModel {
                BacklogId = serviceBacklog.getBacklogInformation()["backlogId"],
                AppointmentId = serviceBacklog.getBacklogInformation()["appointmentId"],
                DateTime = (DateTime)appointment.GetAppointmentDateTime(appointment),
                // DateTimeFormatted = ((DateTime)appointment["DateTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                PatientId = (string)appointment.GetAttribute("PatientId"),
                DoctorId = (string)appointment.GetAttribute("DoctorId"),
                NurseId = (string)appointment.GetAttribute("NurseId"),
                ServiceType = (string)appointment.GetAttribute("ServiceTypeId")
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
