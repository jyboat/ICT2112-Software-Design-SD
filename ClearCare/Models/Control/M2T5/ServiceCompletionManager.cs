using ClearCare.Interfaces;
using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClearCare.Models.Interface;


namespace ClearCare.Models.Control
{
    public class ServiceCompletionManager
    {
        private readonly IAppointmentStatus _appointmentStatus;
        private readonly INotification _notificationManager; // Add INotification dependency
        private readonly ServiceHistoryManager _serviceHistoryManager; // Add ServiceHistoryManager dependency

        // Inject IAppointmentStatus and INotification through the constructor
        public ServiceCompletionManager(IAppointmentStatus appointmentStatus, INotification notificationManager 
        ,ServiceHistoryManager serviceHistoryManager)

        {
            _appointmentStatus = appointmentStatus;
            _notificationManager = notificationManager; // Assign the injected INotification instance
            _serviceHistoryManager = serviceHistoryManager;
        }

        // Method to get all appointment details
        public async Task<List<ServiceAppointment>> GetAllAppointmentDetails()
        {
            // Call the getAppointmentDetails method from IAppointmentStatus
            var appointmentDetails = await _appointmentStatus.getAppointmentDetails();
            return appointmentDetails;
        }

        // Method to update the status of a specific appointment
        public async Task UpdateAppointmentStatus(string appointmentId)
        {
            // Call the updateAppointmentStatus method from IAppointmentStatus
            await _appointmentStatus.updateAppointmentStatus(appointmentId);
            Console.WriteLine($"Appointment status updated for appointment ID: {appointmentId}");
            
            // After updating the status, create a notification
            string notificationContent = $"The status of your appointment (ID: {appointmentId}) has been updated.";
            string userId = "USR007"; // Example user ID, this should be dynamically determined
            await _notificationManager.createNotification(userId, notificationContent); // Create the notification
        }

        public async Task<string> CreateServiceHistory(string appointmentId, string service, string patientId, 
            string nurseId, string doctorId, DateTime serviceDate, string location, string serviceOutcomes)
        {

            // Log all input variables
            Console.WriteLine($"Step 1: Creating service history with details:");
            Console.WriteLine($"  - Appointment ID: {appointmentId}");
            Console.WriteLine($"  - Service Type: {service}");
            Console.WriteLine($"  - Patient ID: {patientId}");
            Console.WriteLine($"  - Nurse ID: {nurseId}");
            Console.WriteLine($"  - Doctor ID: {doctorId}");
            Console.WriteLine($"  - Service Date: {serviceDate}");
            Console.WriteLine($"  - Location: {location}");
            Console.WriteLine($"  - Service Outcomes: {serviceOutcomes}");

            // Step 2: Create service history
            string serviceHistoryId = await _serviceHistoryManager.createServiceHistory(
                appointmentId, service, patientId, nurseId, doctorId, serviceDate, location, serviceOutcomes);

            Console.WriteLine($"Step 2: Service history created with ID: {serviceHistoryId} for appointment ID: {appointmentId}");
            
            return serviceHistoryId; // ✅ Return the generated ID
        }

    }
}