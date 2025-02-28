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
        private readonly IEncryption encryptionService;
        string encryptedText = string.Empty;

        public ManageMedicalRecord(IEncryption encryptionService)
        {
            MedicalRecordGateway = new MedicalRecordGateway();
            this.encryptionService = encryptionService;
        }

        public async Task<MedicalRecord> AddMedicalRecord(string doctorNote, string patientID, byte[] fileBytes, string fileName, string doctorID)
        {
            encryptedText = encryptionService.EncryptMedicalData(doctorNote);

            return await MedicalRecordGateway.InsertMedicalRecord(encryptedText, patientID, fileBytes, fileName, doctorID);
        }


    }

}