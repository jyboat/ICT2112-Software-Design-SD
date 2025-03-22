using ClearCare.DataSource.M3T1;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.Models.Control.M3T1;
using ClearCare.Models.Entities.M3T1;

    [Route("Assessment")]
    public class AssessmentController : Controller
    {
        private readonly AssessmentManager _manager;

        public AssessmentController()
        {
            var mapper = new AssessmentMapper();
            _manager = new AssessmentManager(mapper);
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> List()
    {
        List<Assessment> assessments = await _manager.getAssessments();
        return View("~/Views/M3T1/Assessment/List.cshtml", assessments); // Pass List<Assessment> directly
    }

        [Route("View/{assessmentId}")]
        [HttpGet]
        public async Task<IActionResult> ViewAssessment(string assessmentId)
        {
            var assessment = await _manager.getAssessment(assessmentId);
            if (assessment == null)
            {
                return RedirectToAction("List");
            }
            return View("~/Views/M3T1/Assessment/Index.cshtml", assessment);
        }

        [Route("Add")]
        [HttpGet]
        public IActionResult ViewAdd()
        {
            return View("~/Views/M3T1/Assessment/Add.cshtml");
        }

       [Route("Add")]
[HttpPost]
public async Task<IActionResult> AddAssessment(string severity, string notes, string dateCreated, string patientId, string doctorId, string imageUrls)
{
    if (string.IsNullOrEmpty(severity) || string.IsNullOrEmpty(notes) || string.IsNullOrEmpty(dateCreated) || string.IsNullOrEmpty(patientId) || string.IsNullOrEmpty(doctorId))
    {
        TempData["ErrorMessage"] = "Please fill in all required fields.";
        return View("~/Views/M3T1/Assessment/Add.cshtml");
    }

    // Validate date format (ISO 8601 recommended)
    if (!DateTime.TryParse(dateCreated, out DateTime parsedDate))
    {
        TempData["ErrorMessage"] = "Invalid date format.";
        return View("~/Views/M3T1/Assessment/Add.cshtml");
    }

    // Convert DateTime to an ISO 8601 string (e.g., "2025-03-17T14:30:00Z")
    string formattedDate = parsedDate.ToString("yyyy-MM-ddTHH:mm:ssZ");

    // Split the imageUrls string into a list (if provided)
    List<string> imageUrlList = string.IsNullOrEmpty(imageUrls)
        ? new List<string>()
        : imageUrls.Split(',').Select(url => url.Trim()).ToList();

    // Insert the assessment into Firestore
    string id = await _manager.insertAssessment(severity, notes, formattedDate, patientId, doctorId, imageUrlList);

    TempData["SuccessMessage"] = "Assessment added successfully!";
    return RedirectToAction("List");
}


        [Route("Edit/{assessmentId}")]
        [HttpGet]
        public async Task<IActionResult> ViewEdit(string assessmentId)
        {
            var assessment = await _manager.getAssessment(assessmentId);
            if (assessment == null)
            {
                return RedirectToAction("List");
            }
            return View("~/Views/M3T1/Assessment/Edit.cshtml", assessment);
        }

            [Route("Edit/{assessmentId}")]
        [HttpPost]
        public async Task<IActionResult> UpdateAssessment(string id, string riskLevel, string recommendation, string dateCreated, string imageUrls)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(riskLevel) || string.IsNullOrEmpty(recommendation) || string.IsNullOrEmpty(dateCreated))
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return RedirectToAction("Edit", new { id = id });
            }

            // Validate the date format
            if (!DateTime.TryParse(dateCreated, out DateTime parsedDate))
            {
                TempData["ErrorMessage"] = "Invalid date format.";
                return RedirectToAction("Edit", new { id = id });
            }

            string formattedDate = parsedDate.ToString("yyyy-MM-dd");

            // Convert comma-separated string to list
            List<string> imageUrlList = string.IsNullOrEmpty(imageUrls)
                ? new List<string>()
                : imageUrls.Split(',').Select(url => url.Trim()).ToList();

            // Call the Firestore update function
            bool success = await _manager.updateAssessment(id, riskLevel, recommendation, formattedDate, imageUrlList);

            if (!success)
            {
                TempData["ErrorMessage"] = "Failed to update assessment.";
                return RedirectToAction("Edit", new { id = id });
            }

            TempData["SuccessMessage"] = "Assessment updated successfully!";
            return RedirectToAction("List"); // Redirect to the List view
        }


        [Route("Delete/{assessmentId}")]
        [HttpPost]
        public async Task<IActionResult> DeleteAssessment(string assessmentId)
        {
            await _manager.deleteAssessment(assessmentId);
            TempData["SuccessMessage"] = "Assessment deleted successfully!";
            return RedirectToAction("List");
        }
    }

