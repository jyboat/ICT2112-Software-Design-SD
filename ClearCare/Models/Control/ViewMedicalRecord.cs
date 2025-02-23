using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using Google.Cloud.Firestore;

namespace ClearCare.Models.Control
{
    public class ViewMedicalRecord
    {
        private MedicalRecordGateway MedicalRecordGateway;

        public ViewMedicalRecord()
        {
            MedicalRecordGateway = new MedicalRecordGateway();
        }

        // Retrieve all medical records
        public async Task<List<MedicalRecord>> GetAllMedicalRecords()
        {
            return await MedicalRecordGateway.RetrieveAllMedicalRecords();
        }

        public async Task<MedicalRecord> GetMedicalRecordById(string recordID)
        {
            return await MedicalRecordGateway.RetrieveMedicalRecordById(recordID);
        }

    }

}