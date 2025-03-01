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

        private static bool newMedicalRecordAdded = false;

        // List of observers
        private static List<IMedicalRecordObserver> observers = new List<IMedicalRecordObserver>();

        public ViewMedicalRecord(IEncryption encryptionService)
        {
            MedicalRecordGateway = new MedicalRecordGateway();
            UserGateway = new UserGateway();
            this.encryptionService = encryptionService;
        }

        // Add observer
        public void AddObserver(IMedicalRecordObserver observer)
        {
            if (!observers.Any(o => o.GetType() == observer.GetType()))
            {
                Console.WriteLine($"✅ Observer added: {observer.GetType().Name}");
                observers.Add(observer);
            }
            else
            {
                Console.WriteLine($"⚠️ Observer {observer.GetType().Name} already registered, skipping...");
            }
        }


        // Remove observer
        public void RemoveObserver(IMedicalRecordObserver observer)
        {
            if (observers.Contains(observer))
                observers.Remove(observer);
        }

        public async Task NotifyObservers()
        {
            Console.WriteLine($" Notifying {observers.Count} observers..."); // Debugging log
            var updatedRecords = await MedicalRecordGateway.RetrieveAllMedicalRecords();

            // Set update flag to true
            newMedicalRecordAdded = true;

            foreach (var observer in observers)
            {
                Console.WriteLine($" Notifying observer: {observer.GetType().Name}"); // Debugging log
                observer.OnMedicalRecordUpdated(updatedRecords);
                Console.WriteLine($" Observer {observer.GetType().Name} updated successfully!");
            }
        }

        // Expose an endpoint to check if an update happened
        public static bool HasNewMedicalRecords()
        {
            return newMedicalRecordAdded;
        }

        // Reset the flag after a refresh request
        public static void ResetMedicalRecordFlag()
        {
            newMedicalRecordAdded = false;
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
            string decryptedDoctorNote = encryptionService.DecryptMedicalData((string)recordDetails["DoctorNote"]);

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

        // Export medical record to CSV
        public async Task<string> ExportMedicalRecord(string recordID)
        {
            // Retrieve the medical record
            var medicalRecord = await GetMedicalRecordByID(recordID);
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