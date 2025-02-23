using ClearCare.DataSource;
using ClearCare.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClearCare.Controllers
{
    [Route("NurseAvailability")]
    [ApiController]
    public class NurseAvailabilityController : Controller
    {
        private readonly NurseAvailabilityGateway _gateway;

        public NurseAvailabilityController()
        {
            _gateway = new NurseAvailabilityGateway();
        }

        // Displays Nurse Availability View
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var availabilityList = await _gateway.GetAvailabilityByStaffAsync("USR003"); // Dummy Nurse ID for testing
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
        public async Task<IActionResult> AddAvailability([FromForm] NurseAvailability availability)
        {
            if (availability == null)
                return BadRequest(new { Message = "Invalid data" });

            // Get the next availability ID
            int nextAvailabilityId = await _gateway.GetNextAvailabilityIdAsync();

            // Extract values explicitly from Form
            string nurseId = Request.Form["NurseId"];
            string dateStr = Request.Form["Date"];
            string startTimeStr = "08:00:00"; 
            string endTimeStr = "16:00:00";

            if (string.IsNullOrEmpty(dateStr))
            {
                return BadRequest(new { Message = "Date is required" });
            }

            // Create new availability
            NurseAvailability newAvailability = NurseAvailability.SetAvailabilityDetails(
                nextAvailabilityId,
                nurseId,
                dateStr,  
                startTimeStr,
                endTimeStr
            );

            await _gateway.AddAvailabilityAsync(newAvailability);

            // Ensure the latest data is loaded by redirecting to Index
            return RedirectToAction("Index");
        }

        // Handles Updating of Availability
        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> UpdateAvailability([FromForm] int availabilityId, [FromForm] string nurseID, [FromForm] string date, [FromForm] string startTime, [FromForm] string endTime)
        {
            if (availabilityId == 0 || string.IsNullOrEmpty(date))
                return BadRequest(new { Message = "Invalid data" });

            //Create updated availability object
            NurseAvailability updatedAvailability = NurseAvailability.SetAvailabilityDetails(
                availabilityId,
                nurseID,
                date,
                startTime,
                endTime
            );

            // Console.WriteLine($"Attempting to update availability: {availabilityId} → {date} {startTime} - {endTime}");

            await _gateway.UpdateAvailabilityAsync(updatedAvailability);
            return RedirectToAction("Index"); 
        }

        // Handles Deletion of Availability
        [HttpPost]
        [Route("Delete/{availabilityId}")]
        public async Task<IActionResult> DeleteAvailability(int availabilityId)
        {
            // Console.WriteLine($"Attempting to delete availability with ID: {availabilityId}");
            await _gateway.DeleteAvailabilityAsync(availabilityId);
            return RedirectToAction("Index"); 
        }
    }
}
