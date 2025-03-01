using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using Google.Cloud.Firestore;
using ClearCare.Models.Interface;

namespace ClearCare.Models.Control
{
    public class ManageMedicalRecord
    {
        private MedicalRecordGateway MedicalRecordGateway;

        private readonly ViewMedicalRecord viewMedicalRecord;
        private readonly IEncryption encryptionService;
        string encryptedText = string.Empty;

        public ManageMedicalRecord(IEncryption encryptionService, ViewMedicalRecord viewMedicalRecord)
        {
            MedicalRecordGateway = new MedicalRecordGateway();
            this.viewMedicalRecord = viewMedicalRecord;
            this.encryptionService = encryptionService;
        }

        public async Task<MedicalRecord> AddMedicalRecord(string doctorNote, string patientID, byte[] fileBytes, string fileName, string doctorID)
        {
            string encryptedText = encryptionService.EncryptMedicalData(doctorNote);

            // Insert record
            var newRecord = await MedicalRecordGateway.InsertMedicalRecord(encryptedText, patientID, fileBytes, fileName, doctorID);
            
            if (newRecord != null)
            {
                Console.WriteLine("Calling NotifyObservers...");  // Debugging log
                // Notify observers after adding a new record
                await viewMedicalRecord.NotifyObservers(); 
            }

            return newRecord;
        }


    }

}