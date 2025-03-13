using ClearCare.DataSource;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
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
            // did this so to resolve the circular dependency and fix the error by ensuring that the gateway has a receiver set before any callbacks are invoked otherwise this shit doesnt load lmao

            // Create the gateway first using the parameterless constructor
            var gateway = new NurseAvailabilityGateway();
            // Create the manager and pass the gateway
            _manager = new NurseAvailabilityManagement(gateway);
            // Set the gateway's receiver to the manager (which implements IAvailabilityDB_Receive)
            gateway.Receiver = _manager;

            _serviceAppointmentManagement =  new ServiceAppointmentManagement();

            _calendarManagement = new CalendarManagement((IRetrieveAllAppointments)_serviceAppointmentManagement, (INurseAvailability) _manager);
        }

        // Displays Nurse Availability for Calendar
        [HttpGet]
        [Route("GetAvailabilityByNurseIdForCalendar")]
        public async Task<JsonResult> GetAvailabilityByNurseIdForCalendar([FromQuery] string? nurseId)
        {
            return await _calendarManagement.GetAvailabilityByNurseIdForCalendar("USR003"); // Dummy ID for testing
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
            await _manager.addAvailability("USR003", date);
            return RedirectToAction("~/Views/M2T3/NurseAvailability/Index.cshtml");
        }

        // Handles Updating of Availability
        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> UpdateAvailability([FromForm] int availabilityId, [FromForm] string date)
        {
            await _manager.updateAvailability(availabilityId, "USR003", date);
            return RedirectToAction("~/Views/M2T3/NurseAvailability/Index.cshtml");
        }

        // Handles Deletion of Availability
        [HttpPost]
        [Route("Delete/{availabilityId}")]
        public async Task<IActionResult> DeleteAvailability(int availabilityId)
        {
            // Console.WriteLine($"Attempting to delete availability with ID: {availabilityId}");
            await _manager.deleteAvailability(availabilityId);
            return RedirectToAction("~/Views/M2T3/NurseAvailability/Index.cshtml");
        }

        
    }
}
