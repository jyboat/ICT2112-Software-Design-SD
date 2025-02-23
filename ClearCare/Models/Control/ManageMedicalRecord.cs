using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using Google.Cloud.Firestore;

namespace ClearCare.Models.Control
{
    public class ManageMedicalRecord
    {
        private MedicalRecordGateway MedicalRecordGateway;
        private readonly EncryptionManagement encryptionManagement;
        string encryptedText;

        public ManageMedicalRecord()
        {
            MedicalRecordGateway = new MedicalRecordGateway();
            encryptionManagement = new EncryptionManagement();
        }

        public async Task<MedicalRecord> AddMedicalRecord(string doctorNote, string patientID)
        {
            encryptedText = encryptionManagement.EncryptMedicalData(doctorNote);

            return await MedicalRecordGateway.InsertMedicalRecord(encryptedText, patientID);
        }

    }

}