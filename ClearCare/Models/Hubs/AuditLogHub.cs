using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ClearCare.Models.Hubs
{
    public class AuditLogHub : Hub
    {
        // ✅ Notify all clients that an audit log update has occurred
        public async Task SendAuditLogUpdate()
        {
            await Clients.All.SendAsync("ReceiveAuditLogUpdate");
        }
    }
}
