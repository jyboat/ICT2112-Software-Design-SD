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
    public class ServiceAppointmentStatusManagement: IAppointmentStatus
    {
        private readonly IServiceStatus _iServiceStatus;
        public ServiceAppointmentStatusManagement() {
            
            _iServiceStatus = (IServiceStatus) new ServiceAppointmentManagement();
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
            
            await _iServiceStatus.UpdateAppointment(appt);

        }

       
    }


}