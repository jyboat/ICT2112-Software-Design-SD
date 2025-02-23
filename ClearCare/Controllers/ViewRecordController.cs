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

        [Route("")]
        public async Task<IActionResult> DisplayViewRecord()
        {
            var medicalRecords = await viewMedicalRecord.GetAllMedicalRecords();
            ViewData["MedicalRecords"] = medicalRecords;
            return View("ViewRecord");
        }

        [Route("ViewAttachment/{recordID}")]
        public async Task<IActionResult> ViewAttachment(string recordID)
        {
            var medicalRecord = await viewMedicalRecord.GetMedicalRecordById(recordID);
            if (medicalRecord == null || medicalRecord.Attachment == null)
            {
                return NotFound("File not found.");
            }

            return File(medicalRecord.Attachment, "application/octet-stream", medicalRecord.AttachmentName);
        }
    }
}
