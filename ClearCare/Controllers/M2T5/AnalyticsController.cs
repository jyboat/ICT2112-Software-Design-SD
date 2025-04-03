using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Control;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Controllers
{
    [Route("Analytics")]
    public class AnalyticsController : Controller
    {
        private readonly AnalyticsDashboardManager _manager;

        // DI via constructor injection
        public AnalyticsController(AnalyticsDashboardManager manager)
        {
            _manager = manager;
        }

        [HttpGet("Appointments")]
        public async Task<IActionResult> AppointmentsAnalytics(string status = "", string doctor = "", string type = "")
        {
            var analytics = await _manager.GenerateFilteredAppointmentAnalytics(status, doctor, type);
            ViewData["AppointmentAnalytics"] = analytics;

            // // Retrieve dynamic data from the database
            // var serviceTypes = await _manager.GetServiceTypesAsync();  // e.g., returns List<string>
            // var doctorIds = await _manager.GetDoctorIdsAsync();        // e.g., returns List<string>
            // var serviceStatuses = new List<string> { "completed", "cancelled", "missed", "pending" };

            return View("~/Views/M2T5/Analytics/AppointmentsAnalytics.cshtml");
        }

        [HttpGet("ListAppointments")]
        public async Task<IActionResult> ListAppointments(string filter = "", string value = "", string serviceType = "", string doctorId = "", string status = "")
        {
            // If a filter parameter is provided, override the other parameters accordingly.
            if (!string.IsNullOrEmpty(filter))
            {
                switch (filter.ToLower())
                {
                    case "servicetype":
                        serviceType = value;
                        ViewData["Title"] = $"🏥 Appointments for Service Type: {value}";
                        break;
                    case "doctor":
                        doctorId = value;
                        ViewData["Title"] = $"👨‍⚕️ Appointments for Doctor: {value}";
                        break;
                    case "completed":
                    case "cancelled":
                    case "missed":
                        status = filter;
                        ViewData["Title"] = $"📋 {char.ToUpper(filter[0]) + filter[1..]} Appointments";
                        break;
                    case "pending":
                        status = filter;
                        ViewData["Title"] = $"📋 Pending Appointments";
                        break;
                    default:
                        ViewData["Title"] = "📅 All Appointments";
                        break;
                }
            }
            else
            {
                // If no filter is provided, use the query parameters to set the title.
                if (!string.IsNullOrEmpty(serviceType) || !string.IsNullOrEmpty(doctorId) || !string.IsNullOrEmpty(status))
                {
                    ViewData["Title"] = "Filtered Appointments";
                }
                else
                {
                    ViewData["Title"] = "All Appointments";
                }
            }

            // Retrieve the filtered appointments using your existing code.
            var appointments = await _manager.FetchFilteredAppointments(status, doctorId, serviceType);
            ViewData["Appointments"] = appointments;
            return View("~/Views/M2T5/Analytics/FilteredAppointmentsList.cshtml");
        }


        // [HttpGet("ListAppointments")]
        // public async Task<IActionResult> ListAppointments(string filter = "all", string value = "")
        // {
        //     var appointments = await _manager.FetchFilteredAppointments("", "", "");
        //     List<Dictionary<string, object>> filtered = appointments;

        //     switch (filter.ToLower())
        //     {
        //         case "servicetype":
        //             filtered = appointments.FindAll(a => a["Service"]?.ToString() == value);
        //             ViewData["Title"] = $"🏥 Appointments for Service Type: {value}";
        //             break;
        //         case "doctor":
        //             filtered = appointments.FindAll(a => a["DoctorId"]?.ToString() == value);
        //             ViewData["Title"] = $"👨‍⚕️ Appointments for Doctor: {value}";
        //             break;
        //         case "completed":
        //         case "cancelled":
        //         case "missed":
        //             filtered = appointments.FindAll(a => a["Status"]?.ToString().ToLower() == filter.ToLower());
        //             ViewData["Title"] = $"📋 {char.ToUpper(filter[0]) + filter[1..]} Appointments";
        //             break;

        //         case "pending":
        //             filtered = appointments.FindAll(a => a["Status"]?.ToString().ToLower() == "missed");
        //             ViewData["Title"] = $"📋 Missed Appointments";
        //             break;


        //         default:
        //             ViewData["Title"] = "📅 All Appointments";
        //             break;
        //     }

        //     ViewData["Appointments"] = filtered;
        //     return View("~/Views/M2T5/Analytics/FilteredAppointmentsList.cshtml");
        // }
    }
}
