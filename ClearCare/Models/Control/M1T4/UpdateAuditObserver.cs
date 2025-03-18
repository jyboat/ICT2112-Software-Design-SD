using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Hubs;
using ClearCare.Models.Interface;
using Microsoft.AspNetCore.SignalR;
using ClearCare.Models.Entities;

namespace ClearCare.Models.Control
{
    public class UpdateAuditLogObserver : IAuditObserver
    {
        private readonly IHubContext<AuditLogHub> _auditHub;

        public UpdateAuditLogObserver(IHubContext<AuditLogHub> hubContext, IAuditSubject auditSubject)
        {
            _auditHub = hubContext;
            auditSubject.AddObserver(this); // ✅ Attach observer
        }

        // ✅ Notify all clients 
        public async void OnAuditLogInserted(List<AuditLog> updatedAudit)
        {
            await _auditHub.Clients.All.SendAsync("ReceiveAuditLogUpdate", updatedAudit);
        }
    }
}
