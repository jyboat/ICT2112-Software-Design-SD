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
            _manager = new NurseAvailabilityManager();
        }

        // Displays Nurse Availability View
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var availabilityList = await _manager.RetrieveAll("USR003"); // Dummy ID for testing
            return View(availabilityList);
        }

        // Show AddAvailability View
        [HttpGet]
        [Route("AddAvailability")]
        public IActionResult AddAvailability()
        {
            return View();
        }

        // Add Availability (Handles Form Submission)
        [HttpPost]
        [Route("AddAvailability")]
        public async Task<IActionResult> AddAvailability([FromForm] string date)
        {
            await _manager.CreateAvailability("USR003", date);
            return RedirectToAction("Index");
        }

        // Handles Updating of Availability
        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> UpdateAvailability([FromForm] int availabilityId, [FromForm] string date)
        {
            await _manager.UpdateAvailability(availabilityId, "USR003", date);
            return RedirectToAction("Index");
        }

        // Handles Deletion of Availability
        [HttpPost]
        [Route("Delete/{availabilityId}")]
        public async Task<IActionResult> DeleteAvailability(int availabilityId)
        {
            // Console.WriteLine($"Attempting to delete availability with ID: {availabilityId}");
            await _manager.DeleteAvailability(availabilityId);
            return RedirectToAction("Index");
        }
    }
}
