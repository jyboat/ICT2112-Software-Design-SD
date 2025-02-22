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

        [Route("Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("CreateMD")]
        public IActionResult CreateMD()
        {
            return View();
        }


        // form action to insert medical record
        [HttpPost]
        public async Task<IActionResult> CreateMedicalRecord(string doctorNote, string patientID)
        {
            var userRole = HttpContext.Session.GetString("Role");

            if (userRole != "Doctor") // Only allow doctors to submit records
            {
                return RedirectToAction("Privacy");
            }

            var result = await ManageMedicalRecord.AddMedicalRecord(doctorNote, patientID);

            if (result != null)
            {
                TempData["Success"] = "Medical record added successfully!";
                return RedirectToAction("Privacy"); // Redirect back to the records page
            }
            else
            {
                TempData["Error"] = "Failed to add medical record.";
                return View("CreateMD");
            }

            
        }
    }
}
