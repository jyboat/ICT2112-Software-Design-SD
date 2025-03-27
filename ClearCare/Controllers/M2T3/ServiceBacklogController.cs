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
            // var appointmentIDs = await _manager.getAllBacklogAppointmentID();
            // foreach (var id in appointmentIDs)
            // {
            //     Console.WriteLine("hello world:");
            //     Console.WriteLine(id);
            // }
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
        [Route("ReassignManual")] 
        public async Task<IActionResult> ReassignManual([FromForm] string BacklogId,
                [FromForm] string AppointmentId,
                [FromForm] string PatientId,
                [FromForm] string DoctorId,
                [FromForm] string ServiceType,
                [FromForm] string NurseId,
                [FromForm] string _DateTime,
                [FromForm] string Slot,
                [FromForm] string Location)
        {   
            DateTime parsedDateTime = DateTime.Parse(_DateTime);
            Console.WriteLine($"DEBUG1: {parsedDateTime}");

            bool success = await _manager.reassignBacklog(
                    BacklogId:BacklogId,
                    AppointmentId:AppointmentId,
                    PatientId:PatientId,
                    DoctorId:DoctorId,
                    ServiceType:ServiceType,
                    NurseId:NurseId,
                    _DateTime:parsedDateTime,
                    Slot:int.Parse(Slot),
                    Location: Location
            );
            
            if (success)
            {
                return Ok(new { message = "Backlog successfully rescheduled!" });
            }
            else
            {
                return BadRequest(new { message = "Failed to reschedule backlog." });
            }
        }

        [HttpPost]
        [Route("GenerateDummy")] 
        public async Task<IActionResult> GenerateDummyBacklogs()
        {
            ServiceAppointmentManagement svcMgr = new ServiceAppointmentManagement();
            var allAppointments = (await svcMgr.RetrieveAllAppointments()).Take(5);
            foreach (var appointment in allAppointments)
            {
                await _manager.addBacklog(appointment.GetAttribute("AppointmentId"));
            }

            return Ok(new { message = "dummy dummy done!" });
        }
    }
}
