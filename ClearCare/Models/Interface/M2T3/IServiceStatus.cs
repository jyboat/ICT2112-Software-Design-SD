using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Interfaces
{
    public interface IServiceStatus
    {
        Task<List<ServiceAppointment>> retrieveAllAppointments();
        Task<ServiceAppointment> getAppointmentByID(string appointmentId);
    }
}