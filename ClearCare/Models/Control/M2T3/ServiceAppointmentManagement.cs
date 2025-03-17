    using System.Threading.Tasks;
    using ClearCare.DataSource;
    using ClearCare.Models;
    using ClearCare.Models.Control;
    using ClearCare.Models.Entities;
    using Google.Protobuf.WellKnownTypes;
    using ClearCare.Interfaces;


    namespace ClearCare.Models.Control
    {
        public class ServiceAppointmentManagement : IRetrieveAllAppointments,  ICreateAppointment, IServiceAppointmentDB_Receive
        {
            

        private readonly IServiceAppointmentDB_Send _dbGateway;

        public ServiceAppointmentManagement(IServiceAppointmentDB_Send dbGateway)
        {
            _dbGateway = dbGateway;
        }


            // Get All Service Appointment
            public async Task<List<ServiceAppointment>> getAllServiceAppointments()
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
        public async Task<Dictionary<string, object>> getAppointmentByID(string appointmentId)
            {
                Dictionary<string, object> appointment = await _dbGateway.fetchServiceAppointmentByID(appointmentId);

                return appointment;
            }

        public Task receiveServiceAppointmentById(Dictionary<string, object> serviceAppointment) {
            Console.WriteLine("1 Service Appointment Found.");
            return Task.CompletedTask;
        }
            
        // Create Service Appointment
        public async Task<string> addServiceAppointment(string appointmentId, string patientId, string nurseId,
                string doctorId, string serviceTypeId, string status, DateTime dateTime, int slot, string location)
            {
                // Map JSON data to model
                var appointment = ServiceAppointment.setApptDetails(
                    appointmentId, patientId, nurseId, doctorId, serviceTypeId, status, dateTime, slot, location
                );

                string appointmentID = await _dbGateway.CreateAppointment(appointment);
                return appointmentID;

            }

        public Task receiveCreatedServiceAppointmentId(string serviceAppointmentId) {
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
                    // retrieve existing appt
                    var existingAppointment = await this.getAppointmentByID(appointmentId);
                    if (existingAppointment == null)
                    {
                        return false;
                    }
                    
                    // create updated appt
                    var updatedAppointment = ServiceAppointment.setApptDetails(
                        appointmentId, patientId, nurseId, doctorId, serviceTypeId, status, dateTime, slot, location
                    );
                    
                    // call gateway to update
                    return await _dbGateway.UpdateAppointment(updatedAppointment);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"error updating appointment: {e.Message}");
                    return false;
                }
            }
        public Task receiveUpdatedServiceAppointmentStatus(bool updateStatus) {
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
         public async Task<bool> DeleteAppointment (string appointmentId)
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

         public Task receiveDeletedServiceAppointmentStatus(bool deleteStatus) {
            if (deleteStatus)
                {
                    Console.WriteLine("1 Service Appointment Successfully Deleted.");
                }
                else
                {
                    Console.WriteLine("Service Appointment update failed.");
                }
                return Task.CompletedTask;
        }

            


            // i hardcode the "retrieval" of nurse and patirents first, later once get from mod 1, will update
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
                };
            }
            
            public Task CreateAppointment() {
                Console.WriteLine("Hello Create Appointment Interface");
                return Task.CompletedTask;
            }
        

            public async Task<List<Dictionary<string, object>>> RetrieveAllAppointments()
            {
                Console.WriteLine("Test Thing");

              
                List<ServiceAppointment> appointments = await this.getAllServiceAppointments(); 
                List<Dictionary<string, object>> appointmentList = appointments
                    .Select(a => a.ToFirestoreDictionary()) 
                    .ToList();

                return appointmentList;
                            
            }   

        }

    }
