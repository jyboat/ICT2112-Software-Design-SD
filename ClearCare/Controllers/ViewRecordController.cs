using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Entities;
using ClearCare.Models.Control;
using System.Threading.Tasks;

namespace ClearCare.Controllers
{
    [Route("ViewRecord")]
    public class ViewRecordController : Controller
    {
        private readonly ViewMedicalRecord viewMedicalRecord;

        public ViewRecordController()
        {
            viewMedicalRecord = new ViewMedicalRecord();
        }

        // View all medical record on 1 page
        [Route("")]
        public async Task<IActionResult> DisplayViewRecord()
        {
            var medicalRecords = await viewMedicalRecord.GetAllProcessedMedicalRecords();
            ViewData["MedicalRecords"] = medicalRecords;
            return View("ViewRecord");
        }

        // View all medical record individually
        [Route("{recordID}")]
        public async Task<IActionResult> ViewMedicalRecord(string recordID)
        {
            var recordDetails = await viewMedicalRecord.GetMedicalRecordWithDetails(recordID);
            if (recordDetails == null)
            {
                return NotFound("Medical Record Not Found.");
            }

            ViewData["RecordDetails"] = recordDetails;
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
    }
}
