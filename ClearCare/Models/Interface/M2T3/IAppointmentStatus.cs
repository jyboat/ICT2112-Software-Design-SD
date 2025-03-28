using System.Threading.Tasks;
using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;

namespace ClearCare.Models.Interface
{
    public interface IAppointmentStatus {
        Task<List<ServiceAppointment>> getAllAppointmentDetails();
        Task<List<ServiceAppointment>> getAppointmentDetails();
        Task updateAppointmentStatus(string appointmentId);
    }
}