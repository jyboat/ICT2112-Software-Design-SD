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
public async Task<IActionResult> ListAppointments(string filter = "all")
{
    var gateway = new AnalyticsGateway();
    var appointments = await gateway.FetchAppointmentsByFilter(filter); // this returns List<Dictionary<string, object>>
    var appointmentList = await _analyticsGateway.FetchAppointmentsByFilter(filter);

        ViewData["Appointments"] = appointments;
        ViewData["Title"] = filter switch
        {
            "completed" => "✅ Completed Appointments",
            "pending" => "⏳ Pending Appointments",
            "cancelled" => "❌ Cancelled Appointments",
            _ => "📅 All Appointments"
        };
return View("~/Views/M2T5/Analytics/FilteredAppointmentsList.cshtml", appointmentList);
}

[Route("ListMedicalRecords")]
public async Task<IActionResult> ListMedicalRecords(string filter = "all", string doctorId = null)
{
    var records = await _analyticsGateway.FetchMedicalRecordsByFilter();
    var filtered = records;

    if (filter == "attachments")
    {
        filtered = records
            .Where(r => r.ContainsKey("Attachment") && r["Attachment"] != null)
            .ToList();

        ViewData["Title"] = "📎 Medical Records with Attachments";
    }
    else if (filter == "doctor" && !string.IsNullOrEmpty(doctorId))
    {
        filtered = records
            .Where(r => r.ContainsKey("DoctorID") && r["DoctorID"]?.ToString() == doctorId)
            .ToList();

        ViewData["Title"] = $"👨‍⚕️ Records by Doctor {doctorId}";
    }
    else
    {
        ViewData["Title"] = "📄 All Medical Records";
    }

    ViewData["Records"] = filtered;
    return View("~/Views/M2T5/Analytics/FilteredMedicalRecordsList.cshtml");
}



    }
}
