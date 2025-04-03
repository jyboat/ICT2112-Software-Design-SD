using ClearCare.Models.Entities;

namespace ClearCare.Models.Interface
{
    public interface IAuditObserver
    {
        void OnAuditLogInserted(List<AuditLog> updatedAudit);
    }
}
