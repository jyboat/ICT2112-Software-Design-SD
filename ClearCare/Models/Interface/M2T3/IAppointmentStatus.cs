using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Interfaces
{
    public interface IAppointmentStatus {
        Task<List<ServiceAppointment>> getAppointmentDetails();
        Task updateAppointmentStatus(string appointmentId);
    }
}