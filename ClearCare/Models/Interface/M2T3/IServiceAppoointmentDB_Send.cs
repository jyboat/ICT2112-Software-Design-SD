// defines the methods for sending (or initiating) database operations such as fetching, creating, updating, and deleting nurse availabilities.
// implemented by NurseAvailabilityGateway; used by NurseAvailabilityManagement 

using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.DataSource;

namespace ClearCare.Interfaces
{
    public interface IServiceAppointmentDB_Send
   {
    IServiceAppointmentDB_Receive Receiver { get; set; }
    // fetch all service appointments from db, implemented by gateway
    Task<List<ServiceAppointment>> fetchAllServiceAppointments();
    Task<ServiceAppointment> fetchServiceAppointmentByID(string appointmentId); 
    Task<string> CreateAppointment(ServiceAppointment appointment);
    Task<bool> UpdateAppointment(ServiceAppointment appointment);
    Task<bool> DeleteAppointment (string appointmentId);
    Task<DateTime?> fetchAppointmentTime(string appointmentId);
    
    Task<(List<Dictionary<string, object>> appointments, Dictionary<string, string> patientNames)> fetchAllUnscheduledPatients();
    
    // To be changed delete once got interface from other teams
    Task<List<string>> getAllServices();
   }
}
