using ClearCare.DataSource;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ClearCare.Interfaces;
using System.Text.Json;
using System.Linq.Expressions;
using ClearCare.Models.Interface;

namespace ClearCare.Controllers
{
    [Route("ServiceBacklog")]
    [ApiController]
    public class ServiceBacklogController : Controller
    {
        private readonly ServiceBacklogManagement _manager;
        private readonly IUserList _userList;
        private readonly ICreateAppointment _appointmentManager;

        public ServiceBacklogController()
        {
            _manager = new ServiceBacklogManagement();
            _userList = (IUserList) new AdminManagement(new UserGateway());
            _appointmentManager = (ICreateAppointment) new ServiceAppointmentManagement();
        }

        // Displays All Backlogs
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> index()
        {
            var backlogs = await _manager.getAllBacklogDetails();
            return View("~/Views/M2T3/ServiceBacklog/Index.cshtml", backlogs);
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> delete(string id)
        {
            var backlog = await _manager.getBacklog(id);
            if (backlog == null)
            {
                return NotFound(new { message = "Backlog not found." });
            }
            else {
                await _manager.deleteBacklog(id);
                await _appointmentManager.deleteAppointment(backlog.getBacklogInformation()["appointmentId"]);
                return await index();
            }
        }
        
        [HttpPost]
        [Route("Reassign")] 
        public async Task<IActionResult> reassign([FromForm] string BacklogId,
                [FromForm] string AppointmentId,
                [FromForm] string PatientId,
                [FromForm] string DoctorId,
                [FromForm] string ServiceType,
                [FromForm] string NurseId,
                [FromForm] string _DateTime,
                [FromForm] string Slot,
                [FromForm] string Location
        ) {   
            DateTime parsedDateTime = DateTime.Parse(_DateTime);

            var (success, errorMessage) = await _manager.reassignBacklog(
                BacklogId: BacklogId,
                AppointmentId: AppointmentId,
                PatientId: PatientId,
                DoctorId: DoctorId,
                ServiceType: ServiceType,
                NurseId: NurseId,
                _DateTime: parsedDateTime,
                Slot: int.Parse(Slot),
                Location: Location
            );
            
            if (success)
            {
                return Ok(new { message = "Backlog successfully rescheduled!" });
            }
            else
            {
                return BadRequest(new { message = errorMessage });
            }
        }


        [HttpGet]
        [Route("GetReassignDetails")]
        public async Task<IActionResult> getReassignDetails()
        {
            var users = await _userList.retrieveAllUsers();
            var usersFiltered = users
            .Select(p => new 
            { 
                UserID = p.getProfileData()["UserID"].ToString(),
                Name = p.getProfileData()["Name"].ToString(),
                Role = p.getProfileData()["Role"].ToString()
            })
            .ToList();

            if (usersFiltered == null || !usersFiltered.Any())
            {
                return NotFound(new { message = "No nurses found." });
            }

            return Ok(usersFiltered);
        }

        // [HttpPost]
        // [Route("GenerateDummy")] 
        // public async Task<IActionResult> GenerateDummyBacklogs()
        // {
        //     ServiceAppointmentManagement svcMgr = new ServiceAppointmentManagement();
        //     var allAppointments = (await svcMgr.retrieveAllAppointments()).Take(5);
        //     foreach (var appointment in allAppointments)
        //     {
        //         await _manager.addBacklog(appointment.getAttribute("AppointmentId"));
        //     }

        //     return Ok(new { message = "dummy dummy done!" });
        // }
    }
}