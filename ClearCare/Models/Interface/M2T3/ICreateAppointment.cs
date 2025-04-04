using System.Collections.Generic;
using ClearCare.Models.Entities;
using System.Threading.Tasks;

namespace ClearCare.Interfaces
{
    public interface ICreateAppointment
    {
        Task<string> createAppointment(string patientId, string nurseId, string doctorId, string Service, string status, DateTime dateTime, int slot, string location);
        Task<bool> updateAppointment(ServiceAppointment appointment);
        Task<bool> deleteAppointment(string appointmentId);
    }
}