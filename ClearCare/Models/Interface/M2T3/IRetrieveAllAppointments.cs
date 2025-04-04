using ClearCare.Models.Entities;

namespace ClearCare.Models.Control
{
    public interface IRetrieveAllAppointments
    {
        Task<List<ServiceAppointment>> getAllServiceAppointments();
        Task<ServiceAppointment> getServiceAppointmentById(string apptId);
        Task<List<ServiceAppointment>> retrieveAllAppointmentsByNurse(string nurseId);
        Task<object> suggestPatients();
    }
}