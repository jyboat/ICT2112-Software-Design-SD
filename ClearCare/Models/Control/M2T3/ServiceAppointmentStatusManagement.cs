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

        public async Task<List<ServiceAppointment>> getAppointmentDetails() {
            // Fetch all appointments
            List<Dictionary<string, object>> appointmentDictionaries = await _iServiceStatus.retrieveAllAppointments();

            // Convert to ServiceAppointment objects before filtering
            List<ServiceAppointment> appointments = appointmentDictionaries
                .Select(dict => ServiceAppointment.FromFirestoreData(dict["AppointmentId"].ToString(), dict))
                .ToList();

            // Filter out completed appointments
            List<ServiceAppointment> filteredAppointments = appointments
                .Where(appt => appt != null && appt.GetAttribute("Status") != "Completed")
                .ToList();

            return filteredAppointments;
        }


        public async Task updateAppointmentStatus(string appointmentId) {
             Dictionary<string, object> appointmentDict = await _iServiceStatus.getAppointmentByID(appointmentId);
            if (appointmentDict == null) {
                Console.WriteLine("Failed to get appointment");
                return;
            }
            
            ServiceAppointment appointment = ServiceAppointment.FromFirestoreData(appointmentId, appointmentDict);

              // Ensure DateTime is properly converted
            const string DateTimeFormat = "d/M/yyyy h:mm:ss tt";

            DateTime localTime = DateTime.ParseExact(
                appointment.GetAttribute("Datetime"),  // Ensure correct key
                DateTimeFormat,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.AdjustToUniversal // Ensures UTC conversion
            );


            await _iServiceStatus.UpdateAppointment(
                appointmentId, 
                appointment.GetAttribute("PatientId"), 
                appointment.GetAttribute("NurseId"), 
                appointment.GetAttribute("DoctorId"), 
                appointment.GetAttribute("ServiceTypeId"), 
                "Completed", 
                localTime.ToUniversalTime(), 
                Convert.ToInt32(appointment.GetAttribute("Slot")), 
                appointment.GetAttribute("Location"));
        }

       
    }


}