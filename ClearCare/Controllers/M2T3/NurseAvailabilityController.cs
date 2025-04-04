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
            _calendarManagement = new CalendarManagement();
        }

        // Displays Nurse Availability View
        [HttpGet]
        [Route("Index")]
        public IActionResult index()
        {
            return View("~/Views/M2T3/NurseAvailability/Index.cshtml");
        }

        // Method to get Id from current session 
        private string getCurrentNurseId()
        {
            return HttpContext.Session.GetString("UserID") ?? string.Empty;
        }

        // Displays Nurse Availability for Calendar
        [HttpGet]
        [Route("getAvailabilityByNurseIdForCalendar")]
        public async Task<JsonResult> getAvailabilityByNurseIdForCalendar([FromQuery] string? nurseId)
        {
            string currentNurseId = getCurrentNurseId();
            if (string.IsNullOrEmpty(currentNurseId))
                return new JsonResult(new { error = "User not logged in." });

            return await _calendarManagement.getAvailabilityByNurseIdForCalendar(currentNurseId);
        }


        // Add Availability (Handles Form Submission)
        [HttpPost]
        [Route("AddAvailability")]
        public async Task<IActionResult> addAvailability([FromForm] string date)
        {
            string nurseId = getCurrentNurseId();
            if (string.IsNullOrEmpty(nurseId))
                return BadRequest("User is not logged in.");

            await _manager.addAvailability(nurseId, date);

            return Ok(new { message = "Availability added successfully!" });
            return RedirectToAction("~/Views/M2T3/NurseAvailability/Index.cshtml");
        }

        // Handles Updating of Availability
        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> updateAvailability([FromForm] int availabilityId, [FromForm] string date)
        {
            string nurseId = getCurrentNurseId();
            if (string.IsNullOrEmpty(nurseId))
                return BadRequest("User is not logged in.");

            await _manager.updateAvailability(availabilityId, nurseId, date);

            return Ok(new { message = "Availability updated successfully!" });
            return RedirectToAction("~/Views/M2T3/NurseAvailability/Index.cshtml");
        }

        // Handles Deletion of Availability
        [HttpPost]
        [Route("Delete/{availabilityId}")]
        public async Task<IActionResult> deleteAvailability(int availabilityId)
        {
            await _manager.deleteAvailability(availabilityId);
            return Ok(new { message = "Availability deleted successfully!" });
            return RedirectToAction("~/Views/M2T3/NurseAvailability/Index.cshtml");
        }
    }
}