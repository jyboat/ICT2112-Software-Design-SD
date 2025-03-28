using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Interfaces
{
    public interface IServiceStatus 
    {
        Task<List<ServiceAppointment>> RetrieveAllAppointments();
        Task<ServiceAppointment> getAppointmentByID(string appointmentId);
    }
}