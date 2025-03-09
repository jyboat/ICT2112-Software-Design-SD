using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Models.Control
{
    public interface IRetrieveAllAppointments
    {
        Task<List<Dictionary<string, object>>> RetrieveAllAppointments();
    }
}
