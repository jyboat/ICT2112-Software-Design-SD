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

        public async Task<List<dynamic>> GetAllErratum()
        {
            var errata = await ErratumGateway.RetrieveAllErratums();
            var processedErratum = new List<dynamic>();

            foreach (var erratum in errata)
            {
                var erratumDetails = erratum.GetErratumDetails();
                string doctorName = await UserGateway.FindUserNameByID((string)erratumDetails["DoctorID"]);

                // Decrypt the erratum details before returning it
                string decryptedErratumDetails = encryptionService.DecryptMedicalData((string)erratumDetails["ErratumDetails"]);

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

        public async Task<Erratum> CreateErratum(string medicalRecordID, string erratumDetails, string doctorID)
        {
            encryptedErratumDetails = encryptionService.EncryptMedicalData(erratumDetails);

            var result = await ErratumGateway.InsertErratum(medicalRecordID, encryptedErratumDetails, doctorID);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to create erratum.");
            }
            return result;
        }
    }
}