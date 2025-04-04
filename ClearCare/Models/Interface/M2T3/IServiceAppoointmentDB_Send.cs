using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.DataSource;

namespace ClearCare.Interfaces
{
    public interface IServiceAppointmentDB_Send
    {
        IServiceAppointmentDB_Receive Receiver { get; set; }
        Task<List<ServiceAppointment>> fetchAllServiceAppointments();
        Task<ServiceAppointment> fetchServiceAppointmentByID(string appointmentId);
        Task<string> createAppointment(ServiceAppointment appointment);
        Task<bool> updateAppointment(ServiceAppointment appointment);
        Task<bool> deleteAppointment(string appointmentId);
        Task<DateTime?> fetchAppointmentTime(string appointmentId);
        Task<(List<Dictionary<string, object>> appointments, Dictionary<string, string> patientNames)> fetchAllUnscheduledPatients();
    }
}
