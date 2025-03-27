using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Control.M2T5;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Controllers.M2T5
{
    [Route("Analytics")]
    public class AnalyticsController : Controller
    {
        private readonly AnalyticsDashboardManager _manager = new AnalyticsDashboardManager();

        [HttpGet("Appointments")]
        public async Task<IActionResult> AppointmentsAnalytics(string status = "", string doctor = "", string type = "")
        {
            var analytics = await _manager.GenerateFilteredAppointmentAnalytics(status, doctor, type);
            ViewData["AppointmentAnalytics"] = analytics;
            return View("~/Views/M2T5/Analytics/AppointmentsAnalytics.cshtml");
        }

        [HttpGet("ListAppointments")]
        public async Task<IActionResult> ListAppointments(string filter = "all", string value = "")
        {
            var appointments = await _manager.FetchFilteredAppointments("", "", "");
            List<Dictionary<string, object>> filtered = appointments;

            switch (filter.ToLower())
            {
                case "servicetype":
                    filtered = appointments.FindAll(a => a["ServiceTypeId"]?.ToString() == value);
                    ViewData["Title"] = $"🏥 Appointments for Service Type: {value}";
                    break;
                case "doctor":
                    filtered = appointments.FindAll(a => a["DoctorId"]?.ToString() == value);
                    ViewData["Title"] = $"👨‍⚕️ Appointments for Doctor: {value}";
                    break;
case "completed":
case "cancelled":
case "missed":
    filtered = appointments.FindAll(a => a["Status"]?.ToString().ToLower() == filter.ToLower());
    ViewData["Title"] = $"📋 {char.ToUpper(filter[0]) + filter[1..]} Appointments";
    break;

case "pending":
    filtered = appointments.FindAll(a => a["Status"]?.ToString().ToLower() == "missed");
    ViewData["Title"] = $"📋 Missed Appointments";
    break;


                default:
                    ViewData["Title"] = "📅 All Appointments";
                    break;
            }

            ViewData["Appointments"] = filtered;
            return View("~/Views/M2T5/Analytics/FilteredAppointmentsList.cshtml");
        }

        [HttpGet("MedicalRecords")]
        public async Task<IActionResult> MedicalRecordsAnalytics()
        {
            var analytics = await _manager.GetMedicalRecordsAnalytics();
            ViewData["MedicalRecordsAnalytics"] = analytics;
            return View("~/Views/M2T5/Analytics/MedicalRecordsAnalytics.cshtml");
        }

        [HttpGet("ListMedicalRecords")]
        public async Task<IActionResult> ListMedicalRecords(string filter = "all", string value = "")
        {
            var records = await _manager.FetchMedicalRecordsRaw();
            var filtered = records;
            string title = "📄 All Medical Records";

            switch (filter.ToLower())
            {
                case "attachments":
                    filtered = records.FindAll(r => r.ContainsKey("Attachment") && r["Attachment"] != null);
                    title = "📎 Records with Attachments";
                    break;
                case "doctor":
                    filtered = records.FindAll(r => r.ContainsKey("DoctorID") && r["DoctorID"]?.ToString() == value);
                    title = $"👨‍⚕️ Records for Doctor: {value}";
                    break;
                case "patient":
                    filtered = records.FindAll(r => r.ContainsKey("PatientID") && r["PatientID"]?.ToString() == value);
                    title = $"🏥 Records for Patient: {value}";
                    break;
            }

            ViewData["Records"] = filtered;
            ViewData["Title"] = title;
            return View("~/Views/M2T5/Analytics/FilteredMedicalRecordsList.cshtml");
        }
    }
}
