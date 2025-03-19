using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.DataSource
{
    public class AppointmentAnalyticsGateway
    {
        private FirestoreDb db;

        public AppointmentAnalyticsGateway()
        {
            db = FirebaseService.Initialize();  
        }

        public async Task<Dictionary<string, object>> GetAppointmentAnalytics()
        {
            try
            {
                var appointments = await db.Collection("ServiceAppointments").GetSnapshotAsync();

                // Total number of appointments
                int totalAppointments = appointments.Documents.Count;

                // Count appointments per service type
                Dictionary<string, int> appointmentsPerType = new Dictionary<string, int>();

                // Count appointments per doctor/nurse
                Dictionary<string, int> appointmentsPerProvider = new Dictionary<string, int>();

                // Count appointments per month
                Dictionary<string, int> appointmentsPerMonth = new Dictionary<string, int>();

                // Count completed vs. cancelled appointments
                int completedAppointments = 0;
                int cancelledAppointments = 0;

                foreach (var doc in appointments.Documents)
                {
                    string serviceType = doc.GetValue<string>("ServiceType");
                    string providerID = doc.GetValue<string>("ProviderID"); // Doctor/Nurse
                    string status = doc.GetValue<string>("Status"); // "Completed" or "Cancelled"
                    Timestamp date = doc.GetValue<Timestamp>("AppointmentDate");

                    // Count appointments per service type
                    if (appointmentsPerType.ContainsKey(serviceType))
                        appointmentsPerType[serviceType]++;
                    else
                        appointmentsPerType[serviceType] = 1;

                    // Count appointments per doctor/nurse
                    if (appointmentsPerProvider.ContainsKey(providerID))
                        appointmentsPerProvider[providerID]++;
                    else
                        appointmentsPerProvider[providerID] = 1;

                    // Count appointments per month
                    string monthYearKey = date.ToDateTime().ToString("yyyy-MM");
                    if (appointmentsPerMonth.ContainsKey(monthYearKey))
                        appointmentsPerMonth[monthYearKey]++;
                    else
                        appointmentsPerMonth[monthYearKey] = 1;

                    // Count completed vs. cancelled
                    if (status == "Completed")
                        completedAppointments++;
                    else if (status == "Cancelled")
                        cancelledAppointments++;
                }

                return new Dictionary<string, object>
                {
                    { "TotalAppointments", totalAppointments },
                    { "AppointmentsPerType", appointmentsPerType },
                    { "AppointmentsPerProvider", appointmentsPerProvider },
                    { "AppointmentsPerMonth", appointmentsPerMonth },
                    { "CompletedAppointments", completedAppointments },
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
