using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;

namespace ClearCare.Models.Interface
{
    public interface IAuditLog
    {
        Task<string> InsertAuditLog(string action, string performedBy);
    }
}
