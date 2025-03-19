using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClearCare.DataSource
{
    public class AnalyticsGateway
    {
        private FirestoreDb db;

        public AnalyticsGateway()
        {
            db = FirebaseService.Initialize();  // Ensure Firebase is initialized
        }

        // Get general analytics for medical records
        public async Task<Dictionary<string, object>> GetMedicalRecordsAnalytics()
        {
            try
            {
                var records = await db.Collection("MedicalRecords").GetSnapshotAsync();

                // Total medical records count
                int totalRecords = records.Documents.Count;

                // Count medical records per doctor
                Dictionary<string, int> recordsPerDoctor = new Dictionary<string, int>();

                // Count medical records per patient
                Dictionary<string, int> recordsPerPatient = new Dictionary<string, int>();

                // Count records created per month
                Dictionary<string, int> recordsPerMonth = new Dictionary<string, int>();

                // Count records with attachments
                int recordsWithAttachments = 0;

                foreach (var doc in records.Documents)
                {
                    string doctorID = doc.GetValue<string>("DoctorID");
                    string patientID = doc.GetValue<string>("PatientID");
                    Timestamp date = doc.GetValue<Timestamp>("Date");
                    bool hasAttachment = doc.ContainsField("Attachment") && doc.GetValue<byte[]>("Attachment") != null;

                    // Count records per doctor
                    if (recordsPerDoctor.ContainsKey(doctorID))
                        recordsPerDoctor[doctorID]++;
                    else
                        recordsPerDoctor[doctorID] = 1;

                    // Count records per patient
                    if (recordsPerPatient.ContainsKey(patientID))
                        recordsPerPatient[patientID]++;
                    else
                        recordsPerPatient[patientID] = 1;

                    // Count records per month
                    string monthYearKey = date.ToDateTime().ToString("yyyy-MM");
                    if (recordsPerMonth.ContainsKey(monthYearKey))
                        recordsPerMonth[monthYearKey]++;
                    else
                        recordsPerMonth[monthYearKey] = 1;

                    // Count records with attachments
                    if (hasAttachment) recordsWithAttachments++;
                }

                return new Dictionary<string, object>
                {
                    { "TotalRecords", totalRecords },
                    { "RecordsPerDoctor", recordsPerDoctor },
                    { "RecordsPerPatient", recordsPerPatient },
                    { "RecordsPerMonth", recordsPerMonth },
                    { "RecordsWithAttachments", recordsWithAttachments }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving medical record analytics: {ex.Message}");
                return null;
            }
        }
    }
}
