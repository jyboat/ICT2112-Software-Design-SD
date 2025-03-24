using ClearCare.Models.Entities;

namespace ClearCare.Models.Control
{
    public interface IRetrieveAllAppointments
    {
        Task<List<ServiceAppointment>> RetrieveAllAppointments();
    }
}
