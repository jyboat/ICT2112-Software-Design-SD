using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Control;
using ClearCare.Models.Interface;

namespace ClearCare.Controllers
{
    [Route("FileErratum")]
    public class FileErratumController : Controller
    {
        private ErratumManagement ErratumManagement;

        public FileErratumController(IEncryption encryptionService)
        {
            ErratumManagement = new ErratumManagement(encryptionService);
        }

        [Route("{recordID}")]
        public IActionResult displayUpdateRecord(string recordID)
        {
            var userRole = HttpContext.Session.GetString("Role");

            if (userRole != "Doctor") // Restrict access to doctors only
            {
                Console.WriteLine("You do not have permission to access this page.");
                return RedirectToAction("displayViewRecord", "ViewRecord");
            }

            ViewBag.MedicalRecordID = recordID; // Pass record ID to view

            return View("UpdateRecord");
        }


        // Form action to file erratum
        [HttpPost]
        [Route("FileErratumRecord")]
        public async Task<IActionResult> fileErratum(string recordID, string erratumDetails)
        {
            var userRole = HttpContext.Session.GetString("Role");
            var doctorID = HttpContext.Session.GetString("UserID");

            if (userRole != "Doctor") // Only allow doctors to submit erratum
            {
                Console.WriteLine("You do not have permission to access this page.");
                return RedirectToAction("displayViewRecord", "ViewRecord");
            }

            if (string.IsNullOrEmpty(doctorID))
            {
                return BadRequest("Doctor ID is missing.");
            }

            var result = await ErratumManagement.createErratum(recordID, erratumDetails, doctorID);

            if (result == null)
            {
                return NotFound("Failed to create erratum.");
            }

            return RedirectToAction("displayViewRecord", "ViewRecord", new { recordID = recordID });
        }
    }
}