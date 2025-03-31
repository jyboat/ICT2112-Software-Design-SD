using ClearCare.DataSource;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ClearCare.Interfaces;
using System.Text.Json;

namespace ClearCare.Controllers
{
    [Route("ServiceStatus")]
    [ApiController]
    public class ServiceStatusController : Controller
    {
        private readonly ServiceAppointmentStatusManagement _manager;

        public ServiceStatusController()
        {
            _manager = new ServiceAppointmentStatusManagement();
        }

        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> display(String? patientId)
        {
            if (patientId != null) {
                Console.WriteLine($"PATIENTID: {patientId}");
                 ViewBag.PatientIdFilter = patientId;
            }
            //  await to wait for task complete or data to retrieve before executing
            var appointment =  await _manager.getAppointmentDetails();
            
            
            // No record exists
            if (appointment != null && appointment.Any())
            {
                return View("~/Views/M2T3/ServiceStatus/ServiceStatus.cshtml", appointment);
            }
            else
            {
                return View("~/Views/M2T3/ServiceStatus/ServiceStatus.cshtml", null);
            }

               
            }



    }
}