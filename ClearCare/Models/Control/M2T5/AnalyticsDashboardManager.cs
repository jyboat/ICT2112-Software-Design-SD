using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.DataSource;

namespace ClearCare.Models.Control.M2T5
{
    public class AnalyticsDashboardManager
    {
        private readonly AnalyticsGateway _gateway;

        public AnalyticsDashboardManager()
        {
            _gateway = new AnalyticsGateway();
        }

        public async Task<Dictionary<string, object>> GetAppointmentAnalytics()
        {
            return await _gateway.GetAppointmentAnalytics();
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
