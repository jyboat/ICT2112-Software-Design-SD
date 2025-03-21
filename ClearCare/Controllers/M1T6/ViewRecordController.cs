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
        private readonly ViewPersonalMedicalRecord viewPersonalMedicalRecord;

        public ViewRecordController(IEncryption encryptionService, IAuditLog auditService)
        {
            viewMedicalRecord = new ViewMedicalRecord(encryptionService);
            erratumManagement = new ErratumManagement(encryptionService, auditService);
            viewPersonalMedicalRecord = new ViewPersonalMedicalRecord();
        }

        // View all medical record on 1 page
        [Route("")]
        public async Task<IActionResult> displayViewRecord()
        {
            var userRole = HttpContext.Session.GetString("Role");

            // Restrict access to only Doctor or Nurse
            if (userRole != "Doctor" && userRole != "Nurse")
            {
                Console.WriteLine("You do not have permission to access this page.");
                return RedirectToAction("displayLogin", "Login");
            }

            // Fetch medical records
            var medicalRecords = await viewMedicalRecord.getAllMedicalRecords();

            // Sort records numerically based on MedicalRecordID
            var sortedRecords = medicalRecords.OrderBy(record => int.Parse(Regex.Replace(record.MedicalRecordID, @"\D", ""))).ToList();
            ViewData["MedicalRecords"] = sortedRecords;

            return View("ViewRecord");
        }

        // View all medical record individually
        [Route("Details/{recordID}")]
        public async Task<IActionResult> viewRecord(string recordID)
        {
            var recordDetails = await viewMedicalRecord.getAdjustedRecordByID(recordID);
            if (recordDetails == null)
            {
                return NotFound("Medical Record Not Found.");
            }

            // Fetch erratums for the specific medical record
            var erratums = await erratumManagement.getAllErratum();
            var filteredErratums = erratums.Where(e => e.MedicalRecordID == recordID).ToList();

            ViewData["RecordDetails"] = recordDetails;
            ViewData["Erratums"] = filteredErratums;

            return View("ViewMedicalRecordDetails");
        }

        [Route("ViewAttachment/{recordID}")]
        public async Task<IActionResult> viewAttachment(string recordID)
        {
            var medicalRecord = await viewMedicalRecord.getOriginalRecordByID(recordID);
            if (medicalRecord == null || !medicalRecord.hasAttachment())
            {
                return NotFound("File not found.");
            }

            var (fileBytes, fileName) = medicalRecord.retrieveAttachment();

            return File(fileBytes, "application/octet-stream", fileName);
        }

        [Route("ViewErratumAttachment/{erratumID}")]
        public async Task<IActionResult> viewErratumAttachment(string erratumID)
        {
            var erratum = await erratumManagement.getErratumByID(erratumID);
            if (erratum == null || !erratum.hasErratumAttachment())
            {
                return NotFound("File not found.");
            }

            var (fileBytes, fileName) = erratum.retrieveErratumAttachment();

            if (fileBytes == null)
            {
                return NotFound("File content is empty.");
            }

            return File(fileBytes, "application/octet-stream", fileName);
        }

        //exportRecord(): void
        [Route("Export/{recordID}")]
        public async Task<IActionResult> exportRecord(string recordID)
        {
            // Call the ExportMedicalRecord method from ViewMedicalRecord to export the medical record
            string exportResult = await viewMedicalRecord.exportMedicalRecord(recordID);

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

        // M1T4
        [Route("PatientMedicalRecords")]
        public async Task<IActionResult> viewPatientMedRecord()
        {
            var userID = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("DisplayLogin", "Login");
            }

            if (viewPersonalMedicalRecord == null)
            {
                throw new Exception("viewPersonalMedicalRecord is not initialized.");
            }

            var medicalRecords = await viewPersonalMedicalRecord.getMedicalRecord(userID);

            if (medicalRecords == null || medicalRecords.Count == 0)
            {
                ViewData["MedicalRecords"] = new List<dynamic>();
                return View("ViewPatientMedRecord");
            }
            ViewData["PersonalMedicalRecords"] = medicalRecords;

            return View("ViewPatientMedRecord");
        }

    }

}
