using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using Google.Cloud.Firestore;

namespace ClearCare.Models.Control
{
    public class ErratumManagement
    {
        private ErratumGateway ErratumGateway;
        private readonly UserGateway UserGateway;
        private readonly EncryptionManagement encryptionManagement;
        string encryptedErratumDetails = string.Empty;

        public ErratumManagement()
        {
            ErratumGateway = new ErratumGateway();
            UserGateway = new UserGateway();
            encryptionManagement = new EncryptionManagement();
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
                string decryptedErratumDetails = encryptionManagement.DecryptMedicalData((string)erratumDetails["ErratumDetails"]);

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
            encryptedErratumDetails = encryptionManagement.EncryptMedicalData(erratumDetails);

            var result = await ErratumGateway.InsertErratum(medicalRecordID, encryptedErratumDetails, doctorID);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to create erratum.");
            }
            return result;
        }
    }
}