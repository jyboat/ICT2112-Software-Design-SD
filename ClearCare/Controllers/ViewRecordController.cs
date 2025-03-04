using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Entities;
using ClearCare.Models.Control;
using ClearCare.Models.Interface;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;  // Add this namespace
using ClearCare.Models.Hubs;

namespace ClearCare.Controllers
{
    [Route("ViewRecord")]
    public class ViewRecordController : Controller
    {
        private readonly ViewMedicalRecord viewMedicalRecord;
        private readonly ErratumManagement erratumManagement;

        public ViewRecordController(IEncryption encryptionService)
        {
            viewMedicalRecord = new ViewMedicalRecord(encryptionService);
            erratumManagement = new ErratumManagement(encryptionService);
        }

        // View all medical record on 1 page
        [Route("")]
        public async Task<IActionResult> DisplayViewRecord()
        {
            var userRole = HttpContext.Session.GetString("Role");

            // Restrict access to only Doctor or Nurse
            if (userRole != "Doctor" && userRole != "Nurse")
            {
                Console.WriteLine("You do not have permission to access this page.");
                return RedirectToAction("DisplayLogin", "Login");
            }

            // Fetch medical records
            var medicalRecords = await viewMedicalRecord.GetAllMedicalRecords();

            // Sort records numerically based on MedicalRecordID
            var sortedRecords = medicalRecords.OrderBy(record => int.Parse(Regex.Replace(record.MedicalRecordID, @"\D", ""))).ToList();
            ViewData["MedicalRecords"] = sortedRecords;

            return View("ViewRecord");
        }

        // View all medical record individually
        [Route("Details/{recordID}")]
        public async Task<IActionResult> ViewMedicalRecord(string recordID)
        {
            var recordDetails = await viewMedicalRecord.GetMedicalRecordByID(recordID);
            if (recordDetails == null)
            {
                return NotFound("Medical Record Not Found.");
            }

            // Fetch erratums for the specific medical record
            var erratums = await erratumManagement.GetAllErratum();
            var filteredErratums = erratums.Where(e => e.MedicalRecordID == recordID).ToList();

            ViewData["RecordDetails"] = recordDetails;
            ViewData["Erratums"] = filteredErratums;

            return View("ViewMedicalRecordDetails");
        }

        [Route("ViewAttachment/{recordID}")]
        public async Task<IActionResult> ViewAttachment(string recordID)
        {
            var medicalRecord = await viewMedicalRecord.GetMedicalRecordById(recordID);
            if (medicalRecord == null || !medicalRecord.HasAttachment())
            {
                return NotFound("File not found.");
            }

            var (fileBytes, fileName) = medicalRecord.RetrieveAttachment();
            return File(fileBytes, "application/octet-stream", fileName);
        }

        //exportRecord(): void
        [Route("Export/{recordID}")]
        public async Task<IActionResult> ExportRecord(string recordID)
        {
            // Call the ExportMedicalRecord method from ViewMedicalRecord to export the medical record
            string exportResult = await viewMedicalRecord.ExportMedicalRecord(recordID);

            // Check if the export was successful or if there was an error
            if (exportResult.Contains("exported"))
            {
                // Assuming that the file path returned is accessible, we can return the file as a download
                string filePath = exportResult.Replace("Medical record exported to ", "");

                // Read the file from the path
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var fileName = $"{recordID}_MedicalRecord.csv"; // Use the recordID in the file name

                // Delete the file after sending it to the user (optional, to keep the server clean)
                System.IO.File.Delete(filePath);

                // Return the file as a downloadable response
                return File(fileBytes, "text/csv", fileName);
            }

            // If the export failed, return an error message
            return Content(exportResult);
        }
    }
}
