using ClearCare.DataSource;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ClearCare.Interfaces;
using System.Text.Json;
using System.Linq.Expressions;

namespace ClearCare.Controllers
{
    [Route("ServiceBacklog")]
    [ApiController]
    public class ServiceBacklogController : Controller
    {
        private readonly ServiceBacklogManagement _manager;

        public ServiceBacklogController()
        {
            _manager = new ServiceBacklogManagement();
            _manager.setController(this);
        }

        // Displays All Backlogs
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var backlogs = await _manager.getAllBacklogDetails();
            return View("~/Views/M2T3/ServiceBacklog/Index.cshtml", backlogs);
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _manager.deleteBacklog(id);
            return await Index();
        }
        

        [HttpGet]
        [Route("Reassign/{id}")]
        public async Task<IActionResult> Reassign(string id)
        {
            ViewBag.Nurses = new ServiceAppointmentManagement().GetAllNurses(); // temporary placeholder for M1
            var backlog = await _manager.getBacklogDetails(id);
            return View("~/Views/M2T3/ServiceBacklog/Reassign.cshtml", backlog);
        }

        [HttpPost]
        public async Task<IActionResult> ReassignManual([FromForm] string BacklogId,
                        [FromForm] string AppointmentId,
                        [FromForm] string PatientId,
                        [FromForm] string DoctorId,
                        [FromForm] string ServiceType,
                        [FromForm] int NurseId,
                        [FromForm] DateTime DateTime,
                        [FromForm] int Slot,
                        [FromForm] string Location)
        {
            Console.WriteLine($"BacklogId: {BacklogId}");
            Console.WriteLine($"AppointmentId: {AppointmentId}");
            Console.WriteLine($"PatientId: {PatientId}");
            Console.WriteLine($"DoctorId: {DoctorId}");
            Console.WriteLine($"ServiceType: {ServiceType}");
            Console.WriteLine($"NurseId: {NurseId}");
            Console.WriteLine($"DateTime: {DateTime}");
            Console.WriteLine($"Slot: {Slot}");
            Console.WriteLine($"Location: {Location}");
            
            bool success = await _manager.reassignBacklog(
                BacklogId:BacklogId,
                AppointmentId:AppointmentId,
                PatientId:PatientId,
                DoctorId:DoctorId,
                ServiceType:ServiceType,
                NurseId:NurseId,
                DateTime:DateTime,
                Slot:Slot,
                Location: Location
            );
            
            if (success)
            {
                return RedirectToAction("Index");
            }
            else {
                // might swap to ajax to handle fail on the same page
                return RedirectToAction("Reassign");
            }
        }
    }
}
