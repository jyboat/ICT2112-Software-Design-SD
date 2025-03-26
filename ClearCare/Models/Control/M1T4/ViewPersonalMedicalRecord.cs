using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.Models.Interface;
using ClearCare.DataSource;

namespace ClearCare.Models.Control
{
    public class ViewPersonalMedicalRecord : IMedRecord
    {
        private readonly MedicalRecordGateway _medicalRecordGateway;
        private readonly EncryptionManagement _encryptionManagement;

        public ViewPersonalMedicalRecord()
        {
            _medicalRecordGateway = new MedicalRecordGateway();
            _encryptionManagement = new EncryptionManagement();
        }

        public async Task<List<dynamic>> getMedicalRecord(string userID)
        {
            var medicalRecords = await _medicalRecordGateway.findMedicalRecordsByUserID(userID);
            var processedRecords = new List<dynamic>();

            if (medicalRecords == null || medicalRecords.Count == 0)
            {
                return processedRecords;
            }

            foreach (var record in medicalRecords)
            {
                var recordDetails = record.getRecordDetails(); 

                var (attachmentBytes, attachmentFileName) = record.retrieveAttachment();

                Google.Cloud.Firestore.Timestamp firestoreTimestamp = (Google.Cloud.Firestore.Timestamp)recordDetails["Date"];
                DateTime dateTime = firestoreTimestamp.ToDateTime();
                string formattedDate = dateTime.ToString("dd MMM yyyy, HH:mm");

                string decryptedDoctorNote;
                if (recordDetails["DoctorNote"] is string encryptedNote && !string.IsNullOrEmpty(encryptedNote))
                {
                    try
                    {
                        decryptedDoctorNote = _encryptionManagement.decryptMedicalData(encryptedNote);
                    }
                    catch (Exception ex)
                    {
                        decryptedDoctorNote = "Error decrypting note";
                    }
                }
                else
                {
                    decryptedDoctorNote = "No Notes Available";
                }

                string attachmentName = attachmentFileName ?? "No Attachment";
                string attachmentBase64 = null;
                string attachmentMimeType = null;

                if (attachmentBytes != null && attachmentBytes.Length > 0)
                {
                    
                    // Determine MIME type based on file extension
                    string fileExtension = attachmentName.Split('.').Last().ToLower();
                    switch (fileExtension)
                    {
                        case "jpg": case "jpeg": attachmentMimeType = "image/jpeg"; break;
                        case "png": attachmentMimeType = "image/png"; break;
                        case "pdf": attachmentMimeType = "application/pdf"; break;
                        default: attachmentMimeType = "application/octet-stream"; break;
                    }

                    attachmentBase64 = Convert.ToBase64String(attachmentBytes);
                }

                processedRecords.Add(new
                {
                    MedicalRecordID = recordDetails["MedicalRecordID"],
                    Date = formattedDate,
                    DoctorNote = decryptedDoctorNote,
                    DoctorID = recordDetails["DoctorID"],
                    AttachmentName = attachmentName,
                    AttachmentData = attachmentBase64, 
                    AttachmentType = attachmentMimeType 
                });
            }

            return processedRecords;
        }



    }
}
