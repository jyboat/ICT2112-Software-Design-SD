using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Control;

namespace ClearCare.Controllers
{
    [Route("FileErratum")]
    public class FileErratumController : Controller
    {
        private ErratumManagement ErratumManagement;

        public FileErratumController()
        {
            ErratumManagement = new ErratumManagement();
        }

        [Route("{recordID}")]
        public IActionResult DisplayUpdateRecord(string recordID)
        {
            var userRole = HttpContext.Session.GetString("Role");

            if (userRole != "Doctor") // Restrict access to doctors only
            {
                Console.WriteLine("You do not have permission to access this page.");
                return RedirectToAction("DisplayViewRecord", "ViewRecord");
            }

            ViewBag.MedicalRecordID = recordID; // Pass record ID to view

            return View("UpdateRecord");
        }


        // Form action to file erratum
        [HttpPost]
        [Route("FileErratumRecord")]
        public async Task<IActionResult> FileErratum(string recordID, string erratumDetails)
        {
            var userRole = HttpContext.Session.GetString("Role");
            var doctorID = HttpContext.Session.GetString("UserID");

            if (userRole != "Doctor") // Only allow doctors to submit erratum
            {
                Console.WriteLine("You do not have permission to access this page.");
                return RedirectToAction("DisplayViewRecord", "ViewRecord");
            }

            if (string.IsNullOrEmpty(doctorID))
            {
                return BadRequest("Doctor ID is missing.");
            }

            var result = await ErratumManagement.CreateErratum(recordID, erratumDetails, doctorID);

            if (result == null)
            {
                return NotFound("Failed to create erratum.");
            }

            return RedirectToAction("DisplayViewRecord", "ViewRecord");
        }
    }
}