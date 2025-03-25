using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Interfaces
{
    public interface IServiceStatus 
    {
        Task<List<ServiceAppointment>> RetrieveAllAppointments();
        Task<ServiceAppointment> getAppointmentByID(string appointmentId);
        Task<bool> UpdateAppointment(string appointmentId, string patientId, string nurseId, string doctorId, string serviceTypeId, string status, DateTime dateTime, int slot, string location);
    }
}