using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;

namespace ClearCare.Models.Interface
{
    public interface IAuditLogDatabase
    {
        Task<List<AuditLog>> retrieveAllAuditLogs();
        Task<AuditLog> insertAuditLog(string action, string performedBy);
    }
}

