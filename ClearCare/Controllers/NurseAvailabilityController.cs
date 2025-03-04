using ClearCare.DataSource;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClearCare.Controllers
{
    [Route("NurseAvailability")]
    [ApiController]
    public class NurseAvailabilityController : Controller
    {
        private readonly NurseAvailabilityManager _manager;

        public NurseAvailabilityController()
        {
            _manager = new NurseAvailabilityManager(new NurseAvailabilityGateway());
        }

        // Displays Nurse Availability View
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var availabilityList = _manager.getAvailabilityByStaff("USR003"); // Dummy ID for testing
            return View(availabilityList);
        }

        // Add Availability (Handles Form Submission)
        [HttpPost]
        [Route("AddAvailability")]
        public async Task<IActionResult> AddAvailability([FromForm] string date)
        {
            _manager.addAvailability("USR003", date);
            return RedirectToAction("Index");
        }

        // Handles Updating of Availability
        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> UpdateAvailability([FromForm] int availabilityId, [FromForm] string date)
        {
            _manager.updateAvailability(availabilityId, "USR003", date);
            return RedirectToAction("Index");
        }

        // Handles Deletion of Availability
        [HttpPost]
        [Route("Delete/{availabilityId}")]
        public async Task<IActionResult> DeleteAvailability(int availabilityId)
        {
            // Console.WriteLine($"Attempting to delete availability with ID: {availabilityId}");
            _manager.deleteAvailability(availabilityId);
            return RedirectToAction("Index");
        }
    }
}
