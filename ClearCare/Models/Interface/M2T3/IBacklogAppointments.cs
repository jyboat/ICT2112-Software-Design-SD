using System.Threading.Tasks;


namespace ClearCare.Interfaces
{
    public interface IBacklogAppointments
    {
        Task<List<string>> getAllBacklogAppointmentID();
    }
}