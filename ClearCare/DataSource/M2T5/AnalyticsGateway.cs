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
            db = FirebaseService.Initialize(); 
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

               public async Task<Dictionary<string, object>> GetAppointmentAnalytics()
        {
            try
            {
                var appointments = await db.Collection("ServiceAppointments").GetSnapshotAsync();

                if (appointments.Documents.Count == 0)
                {
                    Console.WriteLine("No appointment records found in Firestore.");
                    return new Dictionary<string, object>();
                }

                // Total number of appointments
                int totalAppointments = appointments.Documents.Count;

                // Count appointments per service type
                Dictionary<string, int> appointmentsPerType = new Dictionary<string, int>();

                // Count appointments per doctor
                Dictionary<string, int> appointmentsPerDoctor = new Dictionary<string, int>();

                // Count appointments per month
                Dictionary<string, int> appointmentsPerMonth = new Dictionary<string, int>();

                // Count completed, pending, and cancelled appointments
                int completedAppointments = 0;
                int pendingAppointments = 0;
                int cancelledAppointments = 0;

                foreach (var doc in appointments.Documents)
                {
                    string serviceType = doc.ContainsField("ServiceTypeId") ? doc.GetValue<string>("ServiceTypeId") : "Unknown";
                    string doctorId = doc.ContainsField("DoctorId") ? doc.GetValue<string>("DoctorId") : "Unknown";
                    string status = doc.ContainsField("Status") ? doc.GetValue<string>("Status") : "Unknown";
                    Timestamp date = doc.ContainsField("DateTime") ? doc.GetValue<Timestamp>("DateTime") : Timestamp.FromDateTime(DateTime.UtcNow);

                    // Count appointments per service type
                    if (appointmentsPerType.ContainsKey(serviceType))
                        appointmentsPerType[serviceType]++;
                    else
                        appointmentsPerType[serviceType] = 1;

                    // Count appointments per doctor
                    if (appointmentsPerDoctor.ContainsKey(doctorId))
                        appointmentsPerDoctor[doctorId]++;
                    else
                        appointmentsPerDoctor[doctorId] = 1;

                    // Count appointments per month
                    string monthYearKey = date.ToDateTime().ToString("yyyy-MM");
                    if (appointmentsPerMonth.ContainsKey(monthYearKey))
                        appointmentsPerMonth[monthYearKey]++;
                    else
                        appointmentsPerMonth[monthYearKey] = 1;

                    // Count completed, pending, and cancelled appointments
                    if (status == "Completed")
                        completedAppointments++;
                    else if (status == "Cancelled")
                        cancelledAppointments++;
                    else
                        pendingAppointments++;
                }

                return new Dictionary<string, object>
                {
                    { "TotalAppointments", totalAppointments },
                    { "AppointmentsPerType", appointmentsPerType },
                    { "AppointmentsPerDoctor", appointmentsPerDoctor },
                    { "AppointmentsPerMonth", appointmentsPerMonth },
                    { "CompletedAppointments", completedAppointments },
                    { "PendingAppointments", pendingAppointments },
                    { "CancelledAppointments", cancelledAppointments }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving appointment analytics: {ex.Message}");
                return null;
            }
        }

    }
}
