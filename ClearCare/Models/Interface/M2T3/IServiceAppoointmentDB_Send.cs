// defines the methods for sending (or initiating) database operations such as fetching, creating, updating, and deleting nurse availabilities.
// implemented by NurseAvailabilityGateway; used by NurseAvailabilityManagement 

using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Interfaces
{
    public interface IServiceAppointmentDB_Send
   {
    // fetch all service appointments from db, implemented by gateway
    Task<List<ServiceAppointment>> fetchAllServiceAppointments();
    Task<Dictionary<string, object>> fetchServiceAppointmentByID(string appointmentId); 
    Task<string> CreateAppointment(ServiceAppointment appointment);
    Task<bool> UpdateAppointment(ServiceAppointment appointment);
    Task<bool> DeleteAppointment (string appointmentId);
    
   }
}
