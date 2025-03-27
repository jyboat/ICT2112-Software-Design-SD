using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ClearCare.DataSource;

namespace ClearCare.Controllers.M2T5
{
    [Route("Analytics")]
    public class AnalyticsController : Controller
    {
private readonly AnalyticsGateway _analyticsGateway = new AnalyticsGateway();

        public AnalyticsController()
        {
            _analyticsGateway = new AnalyticsGateway();
        }

        [HttpGet]
        [Route("MedicalRecords")]
        public async Task<IActionResult> MedicalRecordsAnalytics()
        {
            var analyticsData = await _analyticsGateway.GetMedicalRecordsAnalytics();
            ViewData["MedicalRecordsAnalytics"] = analyticsData;
            return View("~/Views/M2T5/Analytics/MedicalRecordsAnalytics.cshtml");        }

            [HttpGet]
        [Route("Appointments")] 
        public async Task<IActionResult> AppointmentsAnalytics()
        {
            var analyticsData = await _analyticsGateway.GetAppointmentAnalytics();
            ViewData["AppointmentAnalytics"] = analyticsData;
            return View("~/Views/M2T5/Analytics/AppointmentsAnalytics.cshtml");  
        }

[Route("ListAppointments")]
public async Task<IActionResult> ListAppointments(string filter = "all", string value = "")
{
    // Get all appointments from your AnalyticsGateway
    var records = await _analyticsGateway.FetchAppointmentsByFilter("all"); // You can rename this to just FetchAllAppointments() if clearer
    List<Dictionary<string, object>> filtered = records;

    // Filtering logic
    switch (filter.ToLower())
    {
        case "servicetype":
            filtered = records
                .Where(r => r.ContainsKey("ServiceTypeId") && r["ServiceTypeId"]?.ToString() == value)
                .ToList();
            ViewData["Title"] = $"🏥 Appointments for Service Type: {value}";
            break;

        case "doctor":
            filtered = records
                .Where(a => a.ContainsKey("DoctorId") && a["DoctorId"]?.ToString() == value)
                .ToList();
            ViewData["Title"] = $"👨‍⚕️ Appointments for Doctor: {value}";
            break;

        case "completed":
        case "pending":
        case "cancelled":
        case "missed":
            filtered = records
                .Where(r => r.ContainsKey("Status") && r["Status"]?.ToString().ToLower() == filter.ToLower())
                .ToList();
            ViewData["Title"] = $"📋 {char.ToUpper(filter[0]) + filter.Substring(1)} Appointments";
            break;

        default:
            ViewData["Title"] = "📅 All Appointments";
            break;
    }

    ViewData["Appointments"] = filtered;

    return View("~/Views/M2T5/Analytics/FilteredAppointmentsList.cshtml");
}



[Route("ListMedicalRecords")]
public async Task<IActionResult> ListMedicalRecords(string filter = "all", string value = "")
{
    var records = await _analyticsGateway.FetchMedicalRecordsByFilter(); // already implemented
    var filtered = records;
    string title = "📄 All Medical Records";

    switch (filter.ToLower())
    {
        case "attachments":
            filtered = records
                .Where(r => r.ContainsKey("Attachment") && r["Attachment"] != null)
                .ToList();
            title = "📎 Medical Records with Attachments";
            break;

        case "doctor":
            if (!string.IsNullOrEmpty(value))
            {
                filtered = records
                    .Where(r => r.ContainsKey("DoctorID") && r["DoctorID"]?.ToString() == value)
                    .ToList();
                title = $"👨‍⚕️ Records for Doctor: {value}";
            }
            break;

        case "patient":
            if (!string.IsNullOrEmpty(value))
            {
                filtered = records
                    .Where(r => r.ContainsKey("PatientID") && r["PatientID"]?.ToString() == value)
                    .ToList();
                title = $"🏥 Records for Patient: {value}";
            }
            break;
    }

    ViewData["Title"] = title;
    ViewData["Records"] = filtered;

    return View("~/Views/M2T5/Analytics/FilteredMedicalRecordsList.cshtml");
}



    }
}
