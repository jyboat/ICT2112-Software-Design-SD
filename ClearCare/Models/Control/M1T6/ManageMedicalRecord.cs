using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using Google.Cloud.Firestore;
using ClearCare.Models.Interface;

namespace ClearCare.Models.Control
{
    public class ManageMedicalRecord : IMedicalRecordSubject
    {
        private MedicalRecordGateway MedicalRecordGateway;
        private readonly IEncryption encryptionService;
        private readonly IAuditLog auditService;
        string encryptedText = string.Empty;
        private static List<IMedicalRecordObserver> MedObservers = new List<IMedicalRecordObserver>();

        public ManageMedicalRecord(IEncryption encryptionService, IAuditLog auditService)
        {
            MedicalRecordGateway = new MedicalRecordGateway();
            this.encryptionService = encryptionService;
            this.auditService = auditService;
        }

        public async Task<MedicalRecord> addMedicalRecord(string doctorNote, string patientID, byte[] fileBytes, string fileName, string doctorID)
        {
            string encryptedText = encryptionService.encryptMedicalData(doctorNote);

            // Insert record
            var newRecord = await MedicalRecordGateway.insertMedicalRecord(encryptedText, patientID, fileBytes, fileName, doctorID);

            await auditService.InsertAuditLog("Created new medical record", doctorID);
            
            if (newRecord != null)
            {
                // Notify observers after adding a new record
                await notifyObservers(); 
            }

            return newRecord;
        }

        // Observer pattern implementation
        public void addObserver(IMedicalRecordObserver observer)
        {
            if (!MedObservers.Any(o => o.GetType() == observer.GetType()))
            {
                MedObservers.Add(observer);
                Console.WriteLine($"Observer {observer.GetType().Name} added.");
            }
        }

        public void removeObserver(IMedicalRecordObserver observer)
        {
            if (MedObservers.Contains(observer))
                MedObservers.Remove(observer);
        }

        public async Task notifyObservers()
        {
            var updatedRecords = await MedicalRecordGateway.retrieveAllMedicalRecords();
            foreach (var observer in MedObservers)
            {
                Console.WriteLine("Notifying observers about medical record updates...");
                observer.onMedicalRecordUpdated(updatedRecords);
            }
        }

    }

}