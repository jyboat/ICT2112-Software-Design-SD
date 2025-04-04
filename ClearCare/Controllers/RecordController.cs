using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Entities;
using ClearCare.Models.Control;
using ClearCare.Models.Interface;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ClearCare.Models.Hubs;

namespace ClearCare.Controllers
{
    [Route("Record")]
    public class RecordController : Controller
    {
        private readonly ViewPersonalMedicalRecord viewPersonalMedicalRecord;

        public RecordController()
        {
            viewPersonalMedicalRecord = new ViewPersonalMedicalRecord();
        }

        [Route("PatientMedicalRecords")]
        public async Task<IActionResult> viewPatientMedRecord()
        {
            string userID;

            if (HttpContext.Session.GetString("Role") == "Caregiver")
            {
                string caregiverID = HttpContext.Session.GetString("UserID");
                User user = await viewPersonalMedicalRecord.getAssignedPatient(caregiverID);

                if (user != null && user.getProfileData().ContainsKey("AssignedPatientID"))
                {
                    userID = (string)user.getProfileData()["AssignedPatientID"];
                }
                else
                {
                    // Handle error if user is null or UserID is missing
                    userID = "Unknown";
                }
            }
            else
            {
                userID = HttpContext.Session.GetString("UserID");
            }

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