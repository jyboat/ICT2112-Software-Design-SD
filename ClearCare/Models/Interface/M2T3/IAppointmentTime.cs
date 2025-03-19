using System.Collections.Generic;
using ClearCare.Models.Entities;
using ClearCare.Models.Control;

namespace ClearCare.Interfaces
{
    public interface IAppointmentTime {
    // Implemented by M2T3
    // Used by M3T1
    Task<DateTime?> getAppointmentTime(string appointmentId);
    }
}