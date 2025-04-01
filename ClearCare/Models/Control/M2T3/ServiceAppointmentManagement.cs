using System.Threading.Tasks;
using ClearCare.DataSource;
using ClearCare.Models;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Google.Protobuf.WellKnownTypes;
using ClearCare.Interfaces;
using System.Text.Json;
using ClearCare.Models.Interface;


namespace ClearCare.Models.Control
{
    public class ServiceAppointmentManagement : ICreateAppointment, IServiceAppointmentDB_Receive, IAppointmentTime, IServiceStatus
    {
        private readonly IServiceType _iServiceType;
        private readonly IServiceAppointmentDB_Send _dbGateway;
        public ServiceAppointmentManagement()
        {
            _iServiceType = (IServiceType) new ServiceTypeManager();
            _dbGateway = (IServiceAppointmentDB_Send)new ServiceAppointmentGateway();
            _dbGateway.Receiver = this;
        }


        // Get All Service Appointment
        public async Task<List<ServiceAppointment>> RetrieveAllAppointments()
        {
            List<ServiceAppointment> appointments = await _dbGateway.fetchAllServiceAppointments();
            return appointments;
        }

        public Task receiveServiceAppointmentList(List<ServiceAppointment> allServiceAppointments)
        {
            if (allServiceAppointments.Count == 0)
            {
                Console.WriteLine("No service appointment found.");
            }
            else
            {
                Console.WriteLine($"Received {allServiceAppointments.Count} service appointment found.");
            }
            return Task.CompletedTask;
        }

        // Get Service Appointment By ID
        public async Task<ServiceAppointment> getAppointmentByID(string appointmentId)
        {
            ServiceAppointment appointment = await _dbGateway.fetchServiceAppointmentByID(appointmentId);

            return appointment;
        }

        public Task receiveServiceAppointmentById(ServiceAppointment serviceAppointment)
        {
            Console.WriteLine("1 Service Appointment Found.");
            return Task.CompletedTask;
        }

        // Create Service Appointment
        public async Task<string> CreateAppointment(string patientId, string nurseId,
                string doctorId, string Service, string status, DateTime dateTime, int slot, string location)
        {
            // Map JSON data to model
            var appointment = ServiceAppointment.setApptDetails(
                patientId, nurseId, doctorId, Service, status, dateTime, slot, location
            );

            string appointmentID = await _dbGateway.CreateAppointment(appointment);
            return appointmentID;
        }

        public Task receiveCreatedServiceAppointmentId(string serviceAppointmentId)
        {
            if (serviceAppointmentId != "")
            {
                Console.WriteLine("1 Service Appointment Created.");
            }
            else
            {
                Console.WriteLine("Service Appointment Creation failed.");
            }
            return Task.CompletedTask;
        }

        // Update Service Appointment
        public async Task<bool> UpdateAppointment(ServiceAppointment appointment)
        {
            try
            {
                // Logging appointment data
                Console.WriteLine($"attempting to update appointment with data: {JsonSerializer.Serialize(appointment)}");

                // call gateway to update
                return await _dbGateway.UpdateAppointment(appointment);
            }
            catch (Exception e)
            {
                Console.WriteLine($"error updating appointment: {e.Message}");
                return false;
            }
        }

        public Task receiveUpdatedServiceAppointmentStatus(bool updateStatus)
        {
            if (updateStatus)
            {
                Console.WriteLine("1 Service Appointment Updated.");
            }
            else
            {
                Console.WriteLine("Service Appointment update failed.");
            }
            return Task.CompletedTask;
        }

        // Delete Service Appointment
        public async Task<bool> DeleteAppointment(string appointmentId)
        {
            try
            {
                return await _dbGateway.DeleteAppointment(appointmentId);

            }
            catch (Exception e)
            {
                Console.WriteLine($"error deleting appointment: {e.Message}");
                return false;
            }
        }

        public Task receiveDeletedServiceAppointmentStatus(bool deleteStatus)
        {
            if (deleteStatus)
            {
                Console.WriteLine("1 Service Appointment Successfully Deleted.");
            }
            else
            {
                Console.WriteLine("Service Appointment deletion failed.");
            }
            return Task.CompletedTask;
        }

        public async Task<DateTime?> getAppointmentTime(string appointmentId)
        {
            if (string.IsNullOrEmpty(appointmentId))
            {
                Console.WriteLine("Error: No AppointmentID");
                return null;
            }

            try
            {
                DateTime? datetime = await _dbGateway.fetchAppointmentTime(appointmentId);
                return datetime;
            }

            catch (Exception e)
            {
                Console.WriteLine($"Error finding service appointment time: {e.Message}");
                return null;
            }

            return null;
        }

        public Task receiveServiceAppointmentTimeById(DateTime? dateTime)
        {
            if (dateTime != null)
            {
                Console.WriteLine("1 Service Appointment Time Successfully Retrieved.");
            }
            else
            {
                Console.WriteLine("Service Appointment Time retrival failed.");
            }
            return Task.CompletedTask;
        }

        // i hardcode the "retrieval" of services, nurse and patients first, later once get from mod 1, will update
        public List<Dictionary<string, string>> GetAllDoctors()
        {
            return new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string> {{"id", "1"}, {"name", "John Doe"}},
                    new Dictionary<string, string> {{"id", "2"}, {"name", "Jane Doe"}},
                };
        }
        public List<Dictionary<string, string>> GetAllPatients()
        {
            return new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string> {{"id", "1"}, {"name", "John Doe"}},
                    new Dictionary<string, string> {{"id", "2"}, {"name", "Jane Doe"}},
                };
        }

        public List<Dictionary<string, string>> GetAllNurses()
        {
            return new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string> {{"id", "1"}, {"name", "Mike Tyson"}},
                    new Dictionary<string, string> {{"id", "2"}, {"name", "Rocky Balboa"}},
                    new Dictionary<string, string> {{"id", "USR003"}, {"name", "USR003"}},
                };
        }

        public async Task<List<string>> GetServiceTypeNames()
        {
            var services = await _iServiceType.GetServiceTypes();
            var servicesList = services.Select(service => service.Name).ToList();

            foreach (var name in servicesList)
            {
                Console.WriteLine(name); // or use a logger like _logger.LogInformation(name);
            }

            return servicesList;
        }


        // backwards compatibility
        // public List<Dictionary<string, string>> GetAllServiceTypes()
        // {
        //     // convert your simple strings to the format expected by the caller
        //     var serviceTypes = GetServiceTypeNames();
        //     var result = new List<Dictionary<string, string>>();

        //     foreach (var type in serviceTypes)
        //     {
        //         result.Add(new Dictionary<string, string> { { "id", type }, { "name", type } });
        //     }

        //     return result;
        // }

        public Task getUnscheduledPatients(List<ServiceAppointment> allServiceAppointments)
        {
            if (allServiceAppointments.Count == 0)
            {
                Console.WriteLine("No service appointment found.");
            }
            else
            {
                Console.WriteLine($"Received {allServiceAppointments.Count} service appointment found.");
            }
            return Task.CompletedTask;
        }

        public class Patient
        {
            public string PatientId { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
        }

        public async Task<(List<Dictionary<string, object>> appointments, Dictionary<string, string> patientNames)> getUnscheduledPatients()
        {
            // Call the gateway to get the unscheduled patients and patient names
            var (appointments, patientNames) = await _dbGateway.fetchAllUnscheduledPatients();

            return (appointments, patientNames);
        }

        public async Task<List<string>> getAllServices()
        {
            List<string> services = await _dbGateway.getAllServices();
            return services;
        }



        // public Task CreateAppointment() {
        //     Console.WriteLine("Hello Create Appointment Interface");
        //     return Task.CompletedTask;
        // }
    }

}
