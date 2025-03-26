using ClearCare.DataSource;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; 
using ClearCare.Interfaces;

namespace ClearCare.Controllers
{
    [Route("NurseAvailability")]
    [ApiController]
    public class NurseAvailabilityController : Controller
    {
        private readonly NurseAvailabilityManagement _manager;

        private readonly CalendarManagement _calendarManagement;
        private readonly ServiceAppointmentManagement _serviceAppointmentManagement;



        public NurseAvailabilityController()
        {
            _manager = new NurseAvailabilityManagement();
            _serviceAppointmentManagement = new ServiceAppointmentManagement();
         

            _calendarManagement = new CalendarManagement((IRetrieveAllAppointments)_serviceAppointmentManagement, (INurseAvailability) _manager);
        }

        // methods to get Id from current session 
        // private string GetCurrentNurseId()
        //  {
        //      return HttpContext.Session.GetString("UserID") ?? string.Empty; 
        //  }

        // Displays Nurse Availability for Calendar
        [HttpGet]
        [Route("GetAvailabilityByNurseIdForCalendar")]
        public async Task<JsonResult> GetAvailabilityByNurseIdForCalendar([FromQuery] string? nurseId)
        //  public async Task<JsonResult> GetAvailabilityByNurseIdForCalendar([FromQuery] string? inputNurseId)
        {
            return await _calendarManagement.getAvailabilityByNurseIdForCalendar("USR003"); // Dummy ID for testing

            // string currentNurseId = GetCurrentNurseId();
            //  if (string.IsNullOrEmpty(currentNurseId))
            //      return new JsonResult(new { error = "User not logged in." });
 
            //  return await _calendarManagement.GetAvailabilityByNurseIdForCalendar(currentNurseId);
        }

        // Displays Nurse Availability View
        [HttpGet]
        [Route("Index")]
        public IActionResult Index()
        {
            // var availabilityList = await _manager.getAvailabilityByStaff("USR003"); // Dummy ID for testing
            // return View(availabilityList);
            return View("~/Views/M2T3/NurseAvailability/Index.cshtml");
        }

        // Add Availability (Handles Form Submission)
        [HttpPost]
        [Route("AddAvailability")]
        public async Task<IActionResult> AddAvailability([FromForm] string date)
        {
            // string nurseId = GetCurrentNurseId();
            //  if (string.IsNullOrEmpty(nurseId))
            //      return BadRequest("User is not logged in.");
 
            //  await _manager.addAvailability(nurseId, date);
            
            await _manager.addAvailability("USR003", date);
            return Ok(new { message = "Availability added successfully!" });
            return RedirectToAction("~/Views/M2T3/NurseAvailability/Index.cshtml");
        }

        // Handles Updating of Availability
        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> UpdateAvailability([FromForm] int availabilityId, [FromForm] string date)
        {
            // string nurseId = GetCurrentNurseId();
            //  if (string.IsNullOrEmpty(nurseId))
            //      return BadRequest("User is not logged in.");
 
            //  await _manager.updateAvailability(availabilityId, nurseId, date);
            
            await _manager.updateAvailability(availabilityId, "USR003", date);
            return Ok(new { message = "Availability updated successfully!" });
            return RedirectToAction("~/Views/M2T3/NurseAvailability/Index.cshtml");
        }

        // Handles Deletion of Availability
        [HttpPost]
        [Route("Delete/{availabilityId}")]
        public async Task<IActionResult> DeleteAvailability(int availabilityId)
        {
            // Console.WriteLine($"Attempting to delete availability with ID: {availabilityId}");
            await _manager.deleteAvailability(availabilityId);
            return Ok(new { message = "Availability deleted successfully!" });
            return RedirectToAction("~/Views/M2T3/NurseAvailability/Index.cshtml");
        }

        
    }
}