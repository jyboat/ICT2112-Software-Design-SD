using System.Collections.Generic;
using ClearCare.Models.Entities;
using System.Threading.Tasks;
using ClearCare.Models.Entities;

namespace ClearCare.Interfaces
{
    public interface IRescheduleAppointment
    {
        Task<bool> rescheduleAppointment(
            string appointmentId,
            string patientId,
            string nurseId,
            string doctorId,
            string Service,
            string status,
            DateTime dateTime,
            int slot,
            string location);
    }
}