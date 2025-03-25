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

        // Inject IAppointmentStatus and INotification through the constructor
        public ServiceCompletionManager(IAppointmentStatus appointmentStatus, INotification notificationManager)
        {
            _appointmentStatus = appointmentStatus;
            _notificationManager = notificationManager; // Assign the injected INotification instance
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

            // After updating the status, create a notification
            string notificationContent = $"The status of your appointment (ID: {appointmentId}) has been updated.";
            string userId = "USR2"; // Example user ID, this should be dynamically determined
            await _notificationManager.createNotification(userId, notificationContent); // Create the notification
        }
    }
}