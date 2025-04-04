using System.Collections.Generic;
using ClearCare.Models.Entities;
using ClearCare.Models.Control;

namespace ClearCare.Interfaces
{
    public interface IAppointmentTime {
    Task<DateTime?> getAppointmentTime(string appointmentId);
    }
}