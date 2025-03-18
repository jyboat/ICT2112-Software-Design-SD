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

        // patientID = userID
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
            
                // Extract record details
                var recordDetails = record.getRecordDetails();

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
                        Console.WriteLine($"Error decrypting DoctorNote for Record ID: {recordDetails["MedicalRecordID"]} - {ex.Message}");
                        decryptedDoctorNote = "Error decrypting note";
                    }
                }
                else
                {
                    decryptedDoctorNote = "No Notes Available";
                }
               
                string attachmentName = recordDetails["AttachmentName"]?.ToString() ?? "No Attachment";

                processedRecords.Add(new
                {
                    MedicalRecordID = recordDetails["MedicalRecordID"],
                    Date = formattedDate,  
                    DoctorNote = decryptedDoctorNote, 
                    DoctorID = recordDetails["DoctorID"],
                    AttachmentName = attachmentName
                });
            }

            return processedRecords;
        }
    }
}
