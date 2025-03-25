using System.Threading.Tasks;
using ClearCare.DataSource;
using ClearCare.Models;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Google.Protobuf.WellKnownTypes;
using ClearCare.Interfaces;


namespace ClearCare.Models.Control
{
    public class ServiceAppointmentManagement : IRetrieveAllAppointments, ICreateAppointment, IServiceAppointmentDB_Receive, IAppointmentTime, IServiceStatus
    {


        private readonly IServiceAppointmentDB_Send _dbGateway;
        public ServiceAppointmentManagement()
        {
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

        public async Task<List<ServiceAppointment>> RetrieveAllAppointmentsByNurse(string nurseId)
        {
            List<ServiceAppointment> allAppointments = await _dbGateway.fetchAllServiceAppointments();
            
            // List<Dictionary<string, object>> nurseAppointments = allAppointments
            //     .Where(a => a.GetAttribute("NurseId") == nurseId)
            //     .Select(a => a.ToFirestoreDictionary())
            //     .ToList();

            List<ServiceAppointment> nurseAppointments = allAppointments
                .Where(a => a.GetAttribute("NurseId") == nurseId)
                .ToList();

            return nurseAppointments;
        }

        // Create Service Appointment
        public async Task<string> CreateAppointment(string patientId, string nurseId,
                string doctorId, string serviceTypeId, string status, DateTime dateTime, int slot, string location)
        {
            // Map JSON data to model
            var appointment = ServiceAppointment.setApptDetails(
                patientId, nurseId, doctorId, serviceTypeId, status, dateTime, slot, location
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
        public async Task<bool> UpdateAppointment(string appointmentId, string patientId, string nurseId,
    string doctorId, string serviceTypeId, string status, DateTime dateTime, int slot, string location)
        {
            try
            {
                // convert UTC time to Singapore time (UTC+8)
                DateTime sgDateTime = dateTime.AddHours(8);

                Console.WriteLine($"Original UTC time: {dateTime}");
                Console.WriteLine($"Singapore time (UTC+8): {sgDateTime}");

                // retrieve existing appt
                var existingAppointment = await this.getAppointmentByID(appointmentId);
                if (existingAppointment == null)
                {
                    return false;
                }

                // create updated appt
                var updatedAppointment = new ServiceAppointment();

                // set appointment id first
                typeof(ServiceAppointment)
                    .GetProperty("AppointmentId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(updatedAppointment, appointmentId);

                // set other properties using the existing method with singapore time
                updatedAppointment = ServiceAppointment.setApptDetails(
                    patientId, nurseId, doctorId, serviceTypeId, status, sgDateTime, slot, location
                );

                // set appointment id again since it gets overwritten
                typeof(ServiceAppointment)
                    .GetProperty("AppointmentId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(updatedAppointment, appointmentId);

                // call gateway to update
                return await _dbGateway.UpdateAppointment(updatedAppointment);
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



        public List<Dictionary<string, string>> GetAllServiceTypes()
        {
            return new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string> {{"id", "1"}, {"name", "FINANCIAL COUNSELING"}},
                    new Dictionary<string, string> {{"id", "2"}, {"name", "PHYSICAL THERAPY"}},
                    new Dictionary<string, string> {{"id", "3"}, {"name", "WOUND CARE"}},
                };
        }

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

        public async Task<List<ServiceAppointmentGateway.Patient>> getUnscheduledPatients()
        {
            // Call the gateway to get the unscheduled patients.
            List<ServiceAppointmentGateway.Patient> patients = await _dbGateway.fetchAllUnscheduledPatients();
            return patients;
        }
    }

}
