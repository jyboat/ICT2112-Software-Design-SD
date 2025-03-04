using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Entities;
using ClearCare.Models.Control;
using ClearCare.Models.Interface;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ClearCare.Controllers
{
    [Route("ManageRecord")]
    public class ManageRecordController : Controller
    {
        private ManageMedicalRecord ManageMedicalRecord;

        public ManageRecordController(IEncryption encryptionService, IMedicalRecordSubject medRecordSubject)
        {
            ManageMedicalRecord = new ManageMedicalRecord(encryptionService, medRecordSubject);
        }


        [Route("Create")]
        public IActionResult DisplayCreateRecord()
        {
            var userRole = HttpContext.Session.GetString("Role");

            if (userRole != "Doctor") // Restrict access to doctors only
            {
                Console.WriteLine("You do not have permission to access this page.");
                return RedirectToAction("DisplayViewRecord", "ViewRecord"); 
            }

            return View("CreateRecord");
        }

        // Form action to insert medical record with optional file upload
        [HttpPost]
        [Route("CreateMedicalRecord")]
        public async Task<IActionResult> CreateMedicalRecord(IFormFile attachment, string doctorNote, string patientID)
        {
            var userRole = HttpContext.Session.GetString("Role");
            var doctorID = HttpContext.Session.GetString("UserID");

            if (userRole != "Doctor") // Only allow doctors to submit records
            {
                return RedirectToAction("DisplayViewRecord", "ViewRecord");
            }

            byte[] fileBytes = null;
            string fileName = null;

            // Handle file upload if provided
            if (attachment != null && attachment.Length > 0)
            {
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await attachment.CopyToAsync(memoryStream);
                        fileBytes = memoryStream.ToArray(); // Convert file to byte array
                    }
                    fileName = attachment.FileName; // Store original file name
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"File processing failed: {ex.Message}");
                    TempData["Error"] = "Failed to process attachment.";
                    return RedirectToAction("DisplayCreateRecord");
                }
            }

            // Create the medical record and store the file in Firestore
            var result = await ManageMedicalRecord.AddMedicalRecord(doctorNote, patientID, fileBytes, fileName, doctorID);

            if (result != null)
            {
                TempData["Success"] = "Medical record added successfully!";
                return RedirectToAction("DisplayViewRecord", "ViewRecord"); // Redirect back to the records page
            }
            else
            {
                TempData["Error"] = "Failed to add medical record.";
                return RedirectToAction("DisplayCreateRecord");
            }
        }
    }
}
