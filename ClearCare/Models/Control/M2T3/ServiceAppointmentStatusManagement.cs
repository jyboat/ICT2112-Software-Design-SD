using System.Threading.Tasks;
using ClearCare.DataSource;
using ClearCare.Models;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Google.Protobuf.WellKnownTypes;
using ClearCare.Interfaces;
using ClearCare.Models.Interface;


namespace ClearCare.Models.Control
{
    public class ServiceAppointmentStatusManagement: IAppointmentStatus, IRetrieveAllAppointments
    {
        private readonly IServiceStatus _iServiceStatus;
        private readonly ICreateAppointment _iCreateAppointment;
        public ServiceAppointmentStatusManagement() {
            
            _iServiceStatus = (IServiceStatus) new ServiceAppointmentManagement();
            _iCreateAppointment = (ICreateAppointment) new ServiceAppointmentManagement();
        }

        public async Task<List<ServiceAppointment>> getAllAppointmentDetails() {
        // Fetch all appointments - no need to convert
        List<ServiceAppointment> appointments = await _iServiceStatus.RetrieveAllAppointments();
        return appointments;
        }

        public async Task<List<ServiceAppointment>> getAppointmentDetails() {
            // Fetch all appointments - no need to convert
            List<ServiceAppointment> appointments = await _iServiceStatus.RetrieveAllAppointments();

            // Filter out completed appointments
            List<ServiceAppointment> filteredAppointments = appointments
                .Where(appt => appt != null && appt.GetAttribute("Status") != "Completed")
                .ToList();

            return filteredAppointments;
        }


        public async Task updateAppointmentStatus(string appointmentId) {
            ServiceAppointment appointment = await _iServiceStatus.getAppointmentByID(appointmentId);
            if (appointment == null) {
                Console.WriteLine("Failed to get appointment");
                return;
            }

            

            ServiceAppointment appt = appointment.updateServiceAppointementById(
                appointment,
                appointment.GetAttribute("PatientId"), 
                appointment.GetAttribute("NurseId"), 
                appointment.GetAttribute("DoctorId"), 
                appointment.GetAttribute("Service"), 
                "Completed", 
                appointment.GetAppointmentDateTime(appointment), 
                Convert.ToInt32(appointment.GetAttribute("Slot")), 
                appointment.GetAttribute("Location")
            );
            Console.WriteLine($"SVCApptStatusManagement: {appt.GetAppointmentDateTime(appt)}");
            
            await _iCreateAppointment.UpdateAppointment(appt);

        }

        public async Task<List<ServiceAppointment>> getAllServiceAppointments() {
            List<ServiceAppointment> appointments = await _iServiceStatus.RetrieveAllAppointments();
            appointments = await CheckAndUpdateStatusAsync(appointments);
            return appointments;
        }

        public async Task<ServiceAppointment> getServiceAppointmentById(string apptId) {
            ServiceAppointment appointment = await _iServiceStatus.getAppointmentByID(apptId);
            appointment = await CheckAndUpdateStatusAsync(appointment);
            return appointment;
        }

        private async Task<List<ServiceAppointment>> CheckAndUpdateStatusAsync(List<ServiceAppointment> appointments)
        {
            if (appointments == null || appointments.Count == 0) return new List<ServiceAppointment>();

            foreach (var appointment in appointments)
            {
                if (appointment.CheckAndMarkAsMissed()) 
                {
                    Console.WriteLine($"🔍 Appointment ID: {appointment.GetAttribute("AppointmentId")}, Status: {appointment.GetAttribute("Status")}, DateTime: {appointment.GetAttribute("Datetime")}");

                    bool success = await _iCreateAppointment.UpdateAppointment(appointment); 
                    if (!success)
                    {
                        Console.WriteLine($"Failed to update appointment status to missed: {appointment.GetAttribute("AppointmentId")}");
                    }
                    else {
                        Console.WriteLine($"updated {appointment.GetAttribute("AppointmentId")} to {appointment.GetAttribute("Status")}");
                    }
                
                }

            }

            return appointments;
        }

        private async Task<ServiceAppointment> CheckAndUpdateStatusAsync(ServiceAppointment appointment) {
            if (appointment.CheckAndMarkAsMissed()) {
                bool success = await _iCreateAppointment.UpdateAppointment(appointment);
                if (!success)
                    {
                        Console.WriteLine($"Failed to update appointment status to missed: {appointment.GetAttribute("AppointmentId")}");
                    }
                    else {
                        Console.WriteLine($"updated {appointment.GetAttribute("AppointmentId")} to {appointment.GetAttribute("Status")}");
                    }
            }
            return appointment;
        }


        public async Task<List<ServiceAppointment>> RetrieveAllAppointmentsByNurse(string nurseId)
        {
            List<ServiceAppointment> allAppointments = await this.getAllServiceAppointments();
            List<ServiceAppointment> nurseAppointments = allAppointments
                .Where(a => a.GetAttribute("NurseId") == nurseId)
                .ToList();

            return nurseAppointments;
        }


       
    }


}