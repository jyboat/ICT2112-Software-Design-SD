using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using Google.Cloud.Firestore;
using ClearCare.Models.Interface;

namespace ClearCare.Models.Control
{
    public class ErratumManagement
    {
        private ErratumGateway ErratumGateway;
        private readonly UserGateway UserGateway;
        private readonly IEncryption encryptionService;
        string encryptedErratumDetails = string.Empty;

        public ErratumManagement(IEncryption encryptionService)
        {
            ErratumGateway = new ErratumGateway();
            UserGateway = new UserGateway();
            this.encryptionService = encryptionService;
        }

        public async Task<List<dynamic>> getAllErratum()
        {
            var errata = await ErratumGateway.retrieveAllErratums();
            var processedErratum = new List<dynamic>();

            foreach (var erratum in errata)
            {
                var erratumDetails = erratum.getErratumData();
                string doctorName = await UserGateway.findUserNameByID((string)erratumDetails["DoctorID"]);

                // Decrypt the erratum details before returning it
                string decryptedErratumDetails = encryptionService.decryptMedicalData((string)erratumDetails["ErratumDetails"]);

                processedErratum.Add(new
                {
                    ErratumID = erratumDetails["ErratumID"],
                    MedicalRecordID = erratumDetails["MedicalRecordID"],
                    Date = erratumDetails["Date"],
                    CreatedBy = doctorName,
                    ErratumDetails = decryptedErratumDetails
                });
            }
            return processedErratum;
        }

        public async Task<Erratum> createErratum(string medicalRecordID, string erratumDetails, string doctorID)
        {
            encryptedErratumDetails = encryptionService.encryptMedicalData(erratumDetails);

            var result = await ErratumGateway.insertErratum(medicalRecordID, encryptedErratumDetails, doctorID);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to create erratum.");
            }
            return result;
        }
    }
}