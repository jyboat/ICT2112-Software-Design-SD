// UpdateViewObserver.cs
using ClearCare.Models.Entities;
using ClearCare.Models.Hubs;
using ClearCare.Models.Interface;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Models.Control
{
    public class UpdateViewObserver : IMedicalRecordObserver
    {
        private readonly IHubContext<MedicalRecordHub> _medhub;
        public UpdateViewObserver(IHubContext<MedicalRecordHub> hubContext, IMedicalRecordSubject medRecordSubject)
        {
            _medhub = hubContext;

            Console.WriteLine("UpdateViewObserver created."); 

            // Add this observer to the MedRecordSubject when instantiated
            medRecordSubject.AddObserver(this);
        }

        public async void OnMedicalRecordUpdated(List<MedicalRecord> updatedRecords)
        {
            // Debugging log to confirm the method is being called
            Console.WriteLine("UpdateViewObserver received update!");

            // Notify clients via SignalR (update UI in real-time)
            await _medhub.Clients.All.SendAsync("ReceiveMedicalRecordUpdate", updatedRecords);
        }
    }
}
