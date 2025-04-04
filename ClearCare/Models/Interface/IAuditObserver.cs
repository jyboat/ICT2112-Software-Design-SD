using ClearCare.Models.Entities;

namespace ClearCare.Models.Interface
{
    public interface IAuditObserver
    {
        void onAuditLogInserted(List<AuditLog> updatedAudit);
    }
}