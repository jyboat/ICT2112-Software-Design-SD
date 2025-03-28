using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.DataSource;
using ClearCare.Interfaces;
using ClearCare.Models.Interface;
using ClearCare.Models.Entities;

namespace ClearCare.Models.Control
{
    public class AnalyticsDashboardManager
    {
        private readonly AnalyticsGateway _gateway;
        private readonly IAppointmentStatus _appointmentStatusManager;

        public AnalyticsDashboardManager(IAppointmentStatus appointmentStatusManager)
        {
            _gateway = new AnalyticsGateway();
            _appointmentStatusManager = appointmentStatusManager;
        }

        public async Task<Dictionary<string, object>> GetAppointmentAnalytics()
        {
            // return await _gateway.GetAppointmentAnalytics();

            // Retrieve appointments as ServiceAppointment objects via IAppointmentStatus
            List<ServiceAppointment> appointments = await _appointmentStatusManager.getAllAppointmentDetails();
            
            if (appointments == null || appointments.Count == 0)
            {
                Console.WriteLine("No appointment records found.");
                return new Dictionary<string, object>();
            }

            int totalAppointments = appointments.Count;
            var appointmentsPerType = new Dictionary<string, int>();
            var appointmentsPerDoctor = new Dictionary<string, int>();
            var appointmentsPerMonth = new Dictionary<string, int>();
            int completedAppointments = 0;
            int pendingAppointments = 0;
            int cancelledAppointments = 0;

            foreach (ServiceAppointment appointment in appointments)
            {
                // Retrieve attributes from the ServiceAppointment
                string serviceType = appointment.GetAttribute("Service") ?? "Unknown";
                string doctorId = appointment.GetAttribute("DoctorId") ?? "Unknown";
                string status = appointment.GetAttribute("Status") ?? "Unknown";
                DateTime appointmentDateTime = appointment.GetAppointmentDateTime(appointment);

                // Count by service type
                if (appointmentsPerType.ContainsKey(serviceType))
                    appointmentsPerType[serviceType]++;
                else
                    appointmentsPerType[serviceType] = 1;

                // Count by doctor
                if (appointmentsPerDoctor.ContainsKey(doctorId))
                    appointmentsPerDoctor[doctorId]++;
                else
                    appointmentsPerDoctor[doctorId] = 1;

                // Count by month (formatted as "yyyy-MM")
                string monthYearKey = appointmentDateTime.ToString("yyyy-MM");
                if (appointmentsPerMonth.ContainsKey(monthYearKey))
                    appointmentsPerMonth[monthYearKey]++;
                else
                    appointmentsPerMonth[monthYearKey] = 1;

                // Count status types
                if (status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                {
                    completedAppointments++;
                }
                else if (status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                {
                    cancelledAppointments++;
                }
                else
                {
                    // For any other status (e.g., "Missed", "Pending", etc.)
                    pendingAppointments++;
                }
            }

            // Build and return the analytics dictionary
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

        public async Task<List<Dictionary<string, object>>> FetchFilteredAppointments(string status, string doctor, string type)
        {
            return await _gateway.FetchAppointmentsRaw(status, doctor, type); // Return raw list for filtering display
        }

        public async Task<Dictionary<string, object>> GenerateFilteredAppointmentAnalytics(string status, string doctor, string type)
        {
            return await _gateway.FetchAppointmentsByFilter(status, doctor, type); // Return analytics object
        }
    }
}