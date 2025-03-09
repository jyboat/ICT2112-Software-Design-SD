using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Models.Control
{
    public interface IRetrieveAll
    {
        Task<List<Dictionary<string, object>>> RetrieveAll();
    }
}
