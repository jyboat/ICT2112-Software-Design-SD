using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ClearCare.DataSource;

namespace ClearCare.Controllers.M2T5
{
    [Route("Analytics")]
    public class AnalyticsController : Controller
    {
        private readonly AnalyticsGateway _analyticsGateway;

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
    }
}
