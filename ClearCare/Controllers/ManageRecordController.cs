using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Entities;
using ClearCare.Models.Control;
using System.Threading.Tasks;

namespace ClearCare.Controllers
{
    [Route("ManageRecord")]
    public class ManageRecordController : Controller
    {
        private ManageMedicalRecord ManageMedicalRecord;

        public ManageRecordController()
        {
            ManageMedicalRecord = new ManageMedicalRecord();
        }

        [Route("Create")]
        public IActionResult DisplayCreateRecord()
        {
            return View("CreateRecord");
        }

        [Route("Update")]
        public IActionResult DisplayUpdateRecord()
        {
            return View("UpdateRecord");
        }


        // form action to insert medical record
        [HttpPost]
        [Route("CreateMedicalRecord")]
        public async Task<IActionResult> CreateMedicalRecord(string doctorNote, string patientID)
        {
            var userRole = HttpContext.Session.GetString("Role");

            if (userRole != "Doctor") // Only allow doctors to submit records
            {
                return RedirectToAction("DisplayViewRecord", "ViewRecord");
            }

            var result = await ManageMedicalRecord.AddMedicalRecord(doctorNote, patientID);

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
