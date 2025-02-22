using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using Google.Cloud.Firestore;

namespace ClearCare.Models.Control
{
    public class ManageMedicalRecord
    {
        private MedicalRecordGateway MedicalRecordGateway;

        public ManageMedicalRecord()
        {
            MedicalRecordGateway = new MedicalRecordGateway();
        }

        public async Task<MedicalRecord> AddMedicalRecord(string doctorNote, string patientID)
        {
            return await MedicalRecordGateway.InsertMedicalRecord(doctorNote, patientID);
        }

    }

}