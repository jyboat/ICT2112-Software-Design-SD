using Microsoft.AspNetCore.SignalR;

namespace ClearCare.Models.Hubs
{
    public class MedicalRecordHub : Hub
    {
        // This method sends an update to all clients
        public async Task SendMedicalRecordUpdate()
        {
            await Clients.All.SendAsync("ReceiveMedicalRecordUpdate");
        }
    }
}
