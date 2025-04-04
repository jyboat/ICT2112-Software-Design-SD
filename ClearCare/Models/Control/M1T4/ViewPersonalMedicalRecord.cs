using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.Models.Interface;
using ClearCare.DataSource;

namespace ClearCare.Models.Control
{
    public class ViewPersonalMedicalRecord
    {
        private readonly MedicalRecordGateway _medicalRecordGateway;
        private readonly EncryptionManagement _encryptionManagement;
        private readonly UserGateway _userGateway;

        public ViewPersonalMedicalRecord()
        {
            _medicalRecordGateway = new MedicalRecordGateway();
            _encryptionManagement = new EncryptionManagement();
            _userGateway = new UserGateway();
        }
        public async Task<User> getAssignedPatient(string userID)
        {
            var assignedUser = await _userGateway.findUserByID(userID);
            //var assignedPatientID = (string) assignedUser.getProfileData()["AssignedPatientID"];
            return assignedUser;
        }

        public async Task<List<dynamic>> getMedicalRecord(string userID)
        {
            var medicalRecords = await _medicalRecordGateway.findMedicalRecordsByUserID(userID);
            var processedRecords = new List<dynamic>();

            if (medicalRecords == null || medicalRecords.Count == 0)
            {
                return processedRecords;
            }

            var tempList = new List<(DateTime SortKey, dynamic Record)>();

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
                    catch (Exception)
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

                var recordObject = new
                {
                    MedicalRecordID = recordDetails["MedicalRecordID"],
                    Date = formattedDate,
                    DoctorNote = decryptedDoctorNote,
                    DoctorID = recordDetails["DoctorID"],
                    AttachmentName = attachmentName,
                    AttachmentData = attachmentBase64,
                    AttachmentType = attachmentMimeType
                };

                tempList.Add((dateTime, recordObject));
            }

            var sortedRecords = tempList
                .OrderByDescending(item => item.SortKey)
                .Select(item => item.Record)
                .ToList();

            return sortedRecords;
        }
    }
}
