using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Control;
using ClearCare.Models.Interface;

namespace ClearCare.Controllers
{
    [Route("FileErratum")]
    public class FileErratumController : Controller
    {
        private ErratumManagement ErratumManagement;

        public FileErratumController(IEncryption encryptionService, IAuditLog auditService)
        {
            ErratumManagement = new ErratumManagement(encryptionService, auditService);
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
        public async Task<IActionResult> fileErratum(string recordID, string erratumDetails, IFormFile erratumAttachment)
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

            byte[] fileBytes = Array.Empty<byte>();
            string fileName = string.Empty;

            // Handle file upload if provided
            if (erratumAttachment != null && erratumAttachment.Length > 0)
            {
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await erratumAttachment.CopyToAsync(memoryStream);
                        fileBytes = memoryStream.ToArray(); // Convert file to byte array
                    }
                    fileName = erratumAttachment.FileName; // Store original file name
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"File processing failed: {ex.Message}");
                    TempData["Error"] = "Failed to process attachment.";
                    return RedirectToAction("displayCreateRecord");
                }
            }

            var result = await ErratumManagement.createErratum(
                recordID, 
                erratumDetails, 
                doctorID, 
                fileBytes ?? Array.Empty<byte>(), // Provide default empty byte array
                fileName ?? string.Empty // Provide default empty string
            );

            if (result != null)
            {
                TempData["SuccessMessage"] = "Erratum filed successfully!";
                return RedirectToAction("displayViewRecord", "ViewRecord", new { recordID = recordID });
            }
            else{
                TempData["ErrorMessage"] = "Failed to file erratum.";
                 return NotFound("Failed to create erratum.");
            }
        }
    }
}