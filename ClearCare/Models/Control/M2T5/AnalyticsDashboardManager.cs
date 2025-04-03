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
        private readonly IAppointmentStatus _appointmentStatusManager;

        public AnalyticsDashboardManager(IAppointmentStatus appointmentStatusManager)
        {
            _appointmentStatusManager = appointmentStatusManager;
        }

        public async Task<List<Dictionary<string, object>>> FetchFilteredAppointments(string status, string doctor, string type)
        {
            var appointments = await _appointmentStatusManager.getAllServiceAppointments();
            var filtered = appointments;

            // Apply filtering
            if (!string.IsNullOrEmpty(status))
            {
                filtered = filtered
                    .Where(a => a.getAttribute("Status").Equals(status, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(doctor))
            {
                filtered = filtered
                    .Where(a => a.getAttribute("DoctorId") == doctor)
                    .ToList();
            }

            if (!string.IsNullOrEmpty(type))
            {
                filtered = filtered
                    .Where(a => a.getAttribute("Service") == type)
                    .ToList();
            }

            return filtered.Select(a =>
            {
                var dict = a.toFirestoreDictionary();
                dict["AppointmentId"] = a.getAttribute("AppointmentId");
                return dict;
            }).ToList();
        }


        public async Task<Dictionary<string, object>> GenerateFilteredAppointmentAnalytics(string status, string doctor, string type)
        {
            // Retrieve all appointments using the appointment status manager.
            List<ServiceAppointment> appointments = await _appointmentStatusManager.getAllServiceAppointments();

            if (appointments == null || appointments.Count == 0)
            {
                Console.WriteLine("No appointment records found.");
                return new Dictionary<string, object>();
            }

            // Apply filtering based on provided parameters.
            if (!string.IsNullOrEmpty(status))
            {
                appointments = appointments
                    .Where(a => a.getAttribute("Status").Equals(status, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(doctor))
            {
                appointments = appointments
                    .Where(a => a.getAttribute("DoctorId") == doctor)
                    .ToList();
            }

            if (!string.IsNullOrEmpty(type))
            {
                appointments = appointments
                    .Where(a => a.getAttribute("Service") == type)
                    .ToList();
            }

            // Generate analytics based on the filtered list.
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
                string serviceType = appointment.getAttribute("Service") ?? "Unknown";
                string doctorId = appointment.getAttribute("DoctorId") ?? "Unknown";
                string appointmentStatus = appointment.getAttribute("Status") ?? "Unknown";
                DateTime appointmentDateTime = appointment.getAppointmentDateTime(appointment);

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
                if (appointmentStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                {
                    completedAppointments++;
                }
                else if (appointmentStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                {
                    cancelledAppointments++;
                }
                else
                {
                    pendingAppointments++;
                }
            }

            // Build and return the analytics dictionary.
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
    }
}