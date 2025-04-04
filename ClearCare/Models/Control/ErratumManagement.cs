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
        private readonly IAuditSubject auditService;
        string encryptedErratumDetails = string.Empty;

        public ErratumManagement(IEncryption encryptionService, IAuditSubject auditService)
        {
            ErratumGateway = new ErratumGateway();
            UserGateway = new UserGateway();
            this.encryptionService = encryptionService;
            this.auditService = auditService;
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
                    ErratumDetails = decryptedErratumDetails,
                    ErratumAttachmentName = erratumDetails["ErratumAttachmentName"],
                    HasErratumAttachment = erratumDetails["HasErratumAttachment"]
                });
            }
            return processedErratum;
        }

        public async Task<Erratum> getErratumByID(string erratumID)
        {
            var erratum = await ErratumGateway.getErratumByID(erratumID) ?? throw new InvalidOperationException($"Erratum with ID {erratumID} not found.");
            return erratum;
        }

        public async Task<Erratum> createErratum(string medicalRecordID, string erratumDetails, string doctorID, byte[] fileBytes, string fileName)
        {
            encryptedErratumDetails = encryptionService.encryptMedicalData(erratumDetails);

            var result = await ErratumGateway.insertErratum(medicalRecordID, encryptedErratumDetails, doctorID, fileBytes, fileName) ?? throw new InvalidOperationException("Failed to create erratum.");

            await auditService.InsertAuditLog("Filed new erratum", doctorID);

            return result;
        }
    }
}