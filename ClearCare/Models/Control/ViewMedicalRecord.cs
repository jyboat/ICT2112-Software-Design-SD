using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using Google.Cloud.Firestore;
using ClearCare.Models.Interface;

namespace ClearCare.Models.Control
{
    public class ViewMedicalRecord : IMedicalRecord
    {
        private MedicalRecordGateway MedicalRecordGateway;
        private readonly UserGateway UserGateway;
        private readonly EncryptionManagement encryptionManagement;

        public ViewMedicalRecord()
        {
            MedicalRecordGateway = new MedicalRecordGateway();
            UserGateway = new UserGateway();
            encryptionManagement = new EncryptionManagement();
        }

        // Retrieve all medical records and process them for display
        public async Task<List<dynamic>> GetAllMedicalRecords()
        {
            var medicalRecords = await MedicalRecordGateway.RetrieveAllMedicalRecords();
            var processedRecords = new List<dynamic>();

            foreach (var record in medicalRecords)
            {
                var recordDetails = record.GetRecordDetails();
                string patientName = await UserGateway.FindUserNameByID((string)recordDetails["PatientID"]);
                string doctorName = await UserGateway.FindUserNameByID((string)recordDetails["DoctorID"]);

                processedRecords.Add(new
                {
                    MedicalRecordID = recordDetails["MedicalRecordID"],
                    PatientID = patientName,
                    CreatedBy = doctorName,
                    RecordID = recordDetails["MedicalRecordID"]
                });
            }
            return processedRecords;
        }

        // Retrieve medical record by ID
        public async Task<dynamic> GetMedicalRecordByID(string recordID)
        {
            var medicalRecord = await MedicalRecordGateway.RetrieveMedicalRecordById(recordID);
            if (medicalRecord == null)
            {
                return null;
            }

            var recordDetails = medicalRecord.GetRecordDetails();
            string patientName = await UserGateway.FindUserNameByID((string)recordDetails["PatientID"]);
            string doctorName = await UserGateway.FindUserNameByID((string)recordDetails["DoctorID"]);

            // Decrypt the doctor note before returning it
            string decryptedDoctorNote = encryptionManagement.DecryptMedicalData((string)recordDetails["DoctorNote"]);

            return new
            {
                MedicalRecordID = recordDetails["MedicalRecordID"],
                PatientID = patientName,
                CreatedBy = doctorName,
                Date = recordDetails["Date"],
                DoctorNote = decryptedDoctorNote,
                AttachmentName = recordDetails["AttachmentName"],
                HasAttachment = recordDetails["HasAttachment"]
            };
        }

        public async Task<MedicalRecord> GetMedicalRecordById(string recordID)
        {
            return await MedicalRecordGateway.RetrieveMedicalRecordById(recordID);
        }

    }

}