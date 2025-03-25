using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Models.Control
{
    public interface IServiceHistory
    {
        Task<List<Dictionary<string, object>>> getAllServiceHistory();
    }
}