using ClearCare.DataSource;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ClearCare.Interfaces;
using System.Text.Json;

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

        public IActionResult ReassignManual(
            int BacklogId,
            string AppointmentId,
            string PatientId,
            string DoctorId,
            string ServiceType,
            int NurseId,
            DateTime DateTime,
            int Slot,
            string Location)
        {
            Console.WriteLine($"Backlog ID: {BacklogId}, Appointment ID: {AppointmentId}, Nurse ID: {NurseId}");
            
            return RedirectToAction("Index");
        }
    }
}
