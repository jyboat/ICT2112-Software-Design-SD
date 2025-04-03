using System.Threading.Tasks;
using ClearCare.DataSource;
using ClearCare.Models;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Google.Protobuf.WellKnownTypes;
using ClearCare.Interfaces;
using ClearCare.Models.Interface;
using ClearCare.Models.Interface.M2T3;
using System.Text.Json;


namespace ClearCare.Models.Control
{
    public class ServiceAppointmentStatusManagement: AbstractSchedulingNotifier, IAppointmentStatus, IRetrieveAllAppointments
    {
        private readonly IServiceStatus IStatus;
        private readonly ICreateAppointment ICreate;
        private readonly IServiceType IType;
        
        public ServiceAppointmentStatusManagement() {
            
            IStatus = (IServiceStatus) new ServiceAppointmentManagement();
            ICreate = (ICreateAppointment) new ServiceAppointmentManagement();
            IType = (IServiceType) new ServiceTypeManager();
            // attach backlog upon creation of service.
            attach(new ServiceBacklogManagement());
        }


        public async Task<List<ServiceAppointment>> getAppointmentDetails() {
            // Fetch all appointments - no need to convert
            List<ServiceAppointment> appointments = await this.getAllServiceAppointments();

            // Filter out completed appointments
            List<ServiceAppointment> filteredAppointments = appointments
                .Where(appt => appt != null && appt.getAttribute("Status") != "Completed")
                .ToList();

            return filteredAppointments;
        }


        public async Task updateAppointmentStatus(string appointmentId) {
            ServiceAppointment appointment = await IStatus.getAppointmentByID(appointmentId);
            if (appointment == null) {
                Console.WriteLine("Failed to get appointment");
                return;
            }

            

            ServiceAppointment appt = appointment.updateServiceAppointementById(
                appointment,
                appointment.getAttribute("PatientId"), 
                appointment.getAttribute("NurseId"), 
                appointment.getAttribute("DoctorId"), 
                appointment.getAttribute("Service"), 
                "Completed", 
                appointment.getAppointmentDateTime(appointment), 
                Convert.ToInt32(appointment.getAttribute("Slot")), 
                appointment.getAttribute("Location")
            );
            Console.WriteLine($"SVCApptStatusManagement: {appt.getAppointmentDateTime(appt)}");
            
            await ICreate.updateAppointment(appt);

        }

        public async Task<List<ServiceAppointment>> getAllServiceAppointments() {
            List<ServiceAppointment> appointments = await IStatus.retrieveAllAppointments();
            appointments = await checkAndUpdateStatusAsync(appointments);
            return appointments;
        }

        public async Task<ServiceAppointment> getServiceAppointmentById(string apptId) {
            ServiceAppointment appointment = await IStatus.getAppointmentByID(apptId);
            appointment = await checkAndUpdateStatusAsync(appointment);
            return appointment;
        }

        private async Task<List<ServiceAppointment>> checkAndUpdateStatusAsync(List<ServiceAppointment> appointments)
        {
            if (appointments == null || appointments.Count == 0) return new List<ServiceAppointment>();

            foreach (var appointment in appointments)
            {
                if (checkAndMarkAsMissed(appointment)) 
                {
                    
                    bool success = await ICreate.updateAppointment(appointment); 
                    if (!success)
                    {
                        Console.WriteLine($"Failed to update appointment status to missed: {appointment.getAttribute("AppointmentId")}");
                    }
                    else {
                        Console.WriteLine($"updated {appointment.getAttribute("AppointmentId")} to {appointment.getAttribute("Status")}");
                    }
                
                }

            }

            return appointments;
        }

        private async Task<ServiceAppointment> checkAndUpdateStatusAsync(ServiceAppointment appointment) {
            if (checkAndMarkAsMissed(appointment)) {
                bool success = await ICreate.updateAppointment(appointment);
                if (!success)
                    {
                        Console.WriteLine($"Failed to update appointment status to missed: {appointment.getAttribute("AppointmentId")}");
                    }
                    else {
                        Console.WriteLine($"updated {appointment.getAttribute("AppointmentId")} to {appointment.getAttribute("Status")}");
                    }
            }
            return appointment;
        }


        public async Task<List<ServiceAppointment>> retrieveAllAppointmentsByNurse(string nurseId)
        {
            List<ServiceAppointment> allAppointments = await this.getAllServiceAppointments();
            List<ServiceAppointment> nurseAppointments = allAppointments
                .Where(a => a.getAttribute("NurseId") == nurseId)
                .ToList();

            return nurseAppointments;
        }

        private bool checkAndMarkAsMissed(ServiceAppointment appointment)
        {   
         
            if (appointment.getAttribute("Status") != "Completed" && appointment.getAppointmentDateTime(appointment) < DateTime.Now && appointment.getAttribute("Status")  != "Missed")
            {
                appointment.updateStatus("Missed");
                notify(appointment.getAttribute("AppointmentId"), "success");
                
                return true; 
            }
            return false;
        }       

        public async Task<object> suggestPatients()
        {
            var services = await IType.getServiceTypes(); // all available services
            var allAppointments = await this.getAllServiceAppointments();

            var patientList = allAppointments
                .GroupBy(appt => appt.getAttribute("PatientId"))
                .Select(patientGroup =>
                {
                    string patientId = patientGroup.Key;
                    var patientAppointments = patientGroup.ToList();

                    var patientServices = services.Select(service =>
                    {
                        var latestAppt = patientAppointments
                            .Where(appt => appt.getAttribute("Service") == service.Name)
                            .OrderByDescending(appt => appt.getAppointmentDateTime(appt))
                            .FirstOrDefault();

                        return new
                        {
                            Service = service.Name,
                            AppointmentId = latestAppt?.getAttribute("AppointmentId"),
                            NurseId = latestAppt?.getAttribute("NurseId"),
                            DoctorId = latestAppt?.getAttribute("DoctorId"),
                            Status = latestAppt?.getAttribute("Status"),
                            DateTime = latestAppt?.getAttribute("Datetime"),
                            Slot = latestAppt?.getAttribute("Slot"),
                            Location = latestAppt?.getAttribute("Location")
                        };
                    }).ToList();

                    return new
                    {
                        PatientId = patientId,
                        Services = patientServices
                    };
                })
                .ToList();

            return patientList;
        }

        public async Task<List<ServiceType_SDM>> getServices () {
            List<ServiceType_SDM> services = await IType.getServiceTypes();
            return services; 
        }

       
    }


}