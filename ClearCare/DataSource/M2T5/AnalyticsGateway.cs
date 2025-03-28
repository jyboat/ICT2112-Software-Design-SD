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

        public async Task<List<Dictionary<string, object>>> FetchAllAppointments()
        {
            var snapshot = await db.Collection("ServiceAppointments").GetSnapshotAsync();
            var results = new List<Dictionary<string, object>>();

            foreach (var doc in snapshot.Documents)
            {
                var record = doc.ToDictionary();
                record["AppointmentId"] = doc.Id;
                results.Add(record);
            }

            return results;
        }

        private Dictionary<string, object> GenerateAppointmentAnalytics(List<Dictionary<string, object>> appointments)
        {
            var totalAppointments = appointments.Count;
            var appointmentsPerType = new Dictionary<string, int>();
            var appointmentsPerDoctor = new Dictionary<string, int>();
            var appointmentsPerMonth = new Dictionary<string, int>();
            int completedAppointments = 0, pendingAppointments = 0, cancelledAppointments = 0;

            foreach (var doc in appointments)
            {
                string serviceType = doc.ContainsKey("ServiceTypeId") ? doc["ServiceTypeId"]?.ToString() ?? "Unknown" : "Unknown";
                string doctorId = doc.ContainsKey("DoctorId") ? doc["DoctorId"]?.ToString() ?? "Unknown" : "Unknown";
                string status = doc.ContainsKey("Status") ? doc["Status"]?.ToString() ?? "Unknown" : "Unknown";
                DateTime dateTime = doc.ContainsKey("DateTime") && doc["DateTime"] is Timestamp ts
                    ? ts.ToDateTime()
                    : DateTime.UtcNow;

                // Service type count
                if (appointmentsPerType.ContainsKey(serviceType))
                    appointmentsPerType[serviceType]++;
                else
                    appointmentsPerType[serviceType] = 1;

                // Doctor count
                if (appointmentsPerDoctor.ContainsKey(doctorId))
                    appointmentsPerDoctor[doctorId]++;
                else
                    appointmentsPerDoctor[doctorId] = 1;

                // Appointments per month
                string monthYearKey = dateTime.ToString("yyyy-MM");
                if (appointmentsPerMonth.ContainsKey(monthYearKey))
                    appointmentsPerMonth[monthYearKey]++;
                else
                    appointmentsPerMonth[monthYearKey] = 1;

                // Status count
                switch (status.ToLower())
                {
                    case "completed": completedAppointments++; break;
                    case "cancelled": cancelledAppointments++; break;
                    default: pendingAppointments++; break;
                }
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


        // Return raw list of appointment records
        public async Task<List<Dictionary<string, object>>> FetchAppointmentsRaw(string status, string doctor, string type)
        {
            var snapshot = await db.Collection("ServiceAppointments").GetSnapshotAsync();
            List<Dictionary<string, object>> records = new List<Dictionary<string, object>>();

            foreach (var doc in snapshot.Documents)
            {
                var record = doc.ToDictionary();
                record["AppointmentId"] = doc.Id;
                records.Add(record);
            }

            return records;
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
                    else if (status == "Missed")
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

        public async Task<Dictionary<string, object>> FetchAppointmentsByFilter(string status, string doctor, string type)
        {
            var allAppointments = await FetchAllAppointments(); // ← You should have this already
            var filteredAppointments = allAppointments;

            if (!string.IsNullOrEmpty(status))
            {
                filteredAppointments = filteredAppointments
                    .Where(a => a.ContainsKey("Status") && a["Status"]?.ToString().ToLower() == status.ToLower())
                    .ToList();
            }

            if (!string.IsNullOrEmpty(doctor))
            {
                filteredAppointments = filteredAppointments
                    .Where(a => a.ContainsKey("DoctorId") && a["DoctorId"]?.ToString() == doctor)
                    .ToList();
            }

            if (!string.IsNullOrEmpty(type))
            {
                filteredAppointments = filteredAppointments
                    .Where(a => a.ContainsKey("ServiceTypeId") && a["ServiceTypeId"]?.ToString() == type)
                    .ToList();
            }

            return GenerateAppointmentAnalytics(filteredAppointments);
        }
    }
}
