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
        public UpdateViewObserver(IHubContext<MedicalRecordHub> hubContext, IMedicalRecordSubject medicalRecordSubject)
        {
            _medhub = hubContext;
            Console.WriteLine("UpdateViewObserver created."); 

            // Add this observer to ManageMedicalRecord when instantiated
            medicalRecordSubject.addObserver(this);
        }


        public async void onMedicalRecordUpdated(List<MedicalRecord> updatedRecords)
        {
            // Debugging log to confirm the method is being called
            Console.WriteLine("UpdateViewObserver received update!");

            // Notify clients via SignalR (update UI in real-time)
            await _medhub.Clients.All.SendAsync("ReceiveMedicalRecordUpdate", updatedRecords);
        }
    }
}
