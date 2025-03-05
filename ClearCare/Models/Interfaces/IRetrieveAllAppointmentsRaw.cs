using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Models.Control
{
    public interface IRetrieveAllAppointmentsRaw
    {
        Task<List<Dictionary<string, object>>> RetrieveAllAppointmentsRaw();
    }
}
