using ClearCare.Models.Entities;

namespace ClearCare.Models.Control
{
    public interface IDeleteAppointment
    {
        public Task<bool> DeleteAppointment(string appointmentId);
    }
}
