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
            var errata = await ErratumGateway.FindErratum();
            var processedErratum = new List<dynamic>();

            foreach (var erratum in errata)
            {
                var erratumDetails = erratum.GetErratumDetails();
                string userName = await UserGateway.FindUserNameByID((string)erratumDetails["CreatedByUserID"]);

                processedErratum.Add(new
                {
                    ErratumID = erratumDetails["ErratumID"],
                    MedicalRecordID = erratumDetails["MedicalRecordID"],
                    CreatedBy = userName,
                    ErratumDetails = erratumDetails["ErratumDetails"]
                });
            }
            return processedErratum;
        }

        public async Task<Erratum> CreateErratum(string medicalRecordID, string erratumDetails, string userID)
        {
            encryptedErratumDetails = encryptionManagement.EncryptMedicalData(erratumDetails);

            var result = await ErratumGateway.InsertErratum(medicalRecordID, encryptedErratumDetails, userID);
            
            if (result == null)
            {
                throw new InvalidOperationException("Failed to create erratum.");
            }
            return result;
        }
    }
}