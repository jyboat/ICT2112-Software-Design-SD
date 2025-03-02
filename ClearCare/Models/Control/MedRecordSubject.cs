// MedicalRecordManager.cs
using ClearCare.DataSource;
using ClearCare.Models.Interface;
using System.Collections.Generic;
using System.Linq;

namespace ClearCare.Models.Control
{
    public class MedRecordSubject : IMedicalRecordSubject
    {
        // List of observers
        private static List<IMedicalRecordObserver> MedObserver = new List<IMedicalRecordObserver>();
        private MedicalRecordGateway MedicalRecordGateway;

        public MedRecordSubject()
        {
            MedicalRecordGateway = new MedicalRecordGateway();
            Console.WriteLine("Observer Manager is created.");
        }

        // Add observer
        public void AddObserver(IMedicalRecordObserver observer)
        {
            if (!MedObserver.Any(o => o.GetType() == observer.GetType()))
            {
                MedObserver.Add(observer);
                Console.WriteLine($"Observer {observer.GetType().Name} added.");
            }
        }

        // Remove observer
        public void RemoveObserver(IMedicalRecordObserver observer)
        {
            if (MedObserver.Contains(observer))
                MedObserver.Remove(observer);
        }

        public async Task NotifyObservers()
        {
            var updatedRecords = await MedicalRecordGateway.RetrieveAllMedicalRecords();

            foreach (var observer in MedObserver)
            {
                Console.WriteLine("Activating OnMedicalRecordUpdated!");
                observer.OnMedicalRecordUpdated(updatedRecords);
            }
        }
    }
}
