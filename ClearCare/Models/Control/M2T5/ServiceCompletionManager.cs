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

        // Inject IAppointmentStatus through the constructor
        public ServiceCompletionManager(IAppointmentStatus appointmentStatus)
        {
            _appointmentStatus = appointmentStatus;
        }

        // Method to get all appointment details
        public async Task<List<ServiceAppointment>> GetAllAppointmentDetails()
        {
            // Call the getAppointmentDetails method from IAppointmentStatus
            var appointmentDetails = await _appointmentStatus.getAppointmentDetails();
            return appointmentDetails;
        }
    }
}
