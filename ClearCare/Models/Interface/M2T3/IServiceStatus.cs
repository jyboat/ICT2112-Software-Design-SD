using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Interfaces
{
    public interface IServiceStatus 
    {
        Task<List<Dictionary<string, object>>> retrieveAllAppointments();
        Task<Dictionary<string, object>> getAppointmentByID(string appointmentId);
        Task<bool> UpdateAppointment(string appointmentId, string patientId, string nurseId, string doctorId, string serviceTypeId, string status, DateTime dateTime, int slot, string location);
    }
}