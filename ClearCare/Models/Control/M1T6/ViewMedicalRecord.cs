using System.Text;
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
        private readonly IEncryption encryptionService;

        public ViewMedicalRecord(IEncryption encryptionService)
        {
            MedicalRecordGateway = new MedicalRecordGateway();
            UserGateway = new UserGateway();
            this.encryptionService = encryptionService;
        }

        // Retrieve all medical records and process them for display
        public async Task<List<dynamic>> getAllMedicalRecords()
        {
            var medicalRecords = await MedicalRecordGateway.retrieveAllMedicalRecords();
            var processedRecords = new List<dynamic>();

            foreach (var record in medicalRecords)
            {
                var recordDetails = record.getRecordDetails();
                string patientName = await UserGateway.findUserNameByID((string)recordDetails["PatientID"]);
                string doctorName = await UserGateway.findUserNameByID((string)recordDetails["DoctorID"]);

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
        public async Task<dynamic> getAdjustedRecordByID(string recordID)
        {
            var medicalRecord = await MedicalRecordGateway.retrieveMedicalRecordById(recordID);
            if (medicalRecord == null)
            {
                return null;
            }

            var recordDetails = medicalRecord.getRecordDetails();
            string patientName = await UserGateway.findUserNameByID((string)recordDetails["PatientID"]);
            string doctorName = await UserGateway.findUserNameByID((string)recordDetails["DoctorID"]);

            // Decrypt the doctor note before returning it
            string decryptedDoctorNote = encryptionService.decryptMedicalData((string)recordDetails["DoctorNote"]);

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

        public async Task<MedicalRecord> getOriginalRecordByID(string recordID)
        {
            return await MedicalRecordGateway.retrieveMedicalRecordById(recordID);
        }

        // Export medical record to CSV
        public async Task<string> exportMedicalRecord(string recordID)
        {
            // Retrieve the medical record
            var medicalRecord = await getAdjustedRecordByID(recordID);
            if (medicalRecord == null)
            {
                return "Medical record not found.";
            }

            // Prepare the CSV file content
            StringBuilder csvContent = new StringBuilder();
            csvContent.AppendLine("MedicalRecordID,PatientID,CreatedBy,Date,DoctorNote,AttachmentName,HasAttachment");

            // Add medical record details to the CSV content
            csvContent.AppendLine($"{medicalRecord.MedicalRecordID},{medicalRecord.PatientID},{medicalRecord.CreatedBy},{medicalRecord.Date},{medicalRecord.DoctorNote},{medicalRecord.AttachmentName},{medicalRecord.HasAttachment}");

            // Specify the file path where the CSV will be saved
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), $"{recordID}_MedicalRecord.csv");

            // Write the content to the file
            await File.WriteAllTextAsync(filePath, csvContent.ToString());

            // Return the file path to indicate success
            return $"Medical record exported to {filePath}.";
        }

    }

}