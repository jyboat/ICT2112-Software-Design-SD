using ClearCare.DataSource.M3T1;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.Models.Control.M3T1;
using ClearCare.Models.Entities.M3T1;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;

[Route("Assessment")]
public class AssessmentController : Controller
{
    private readonly AssessmentManager _manager;
    private readonly IWebHostEnvironment _environment;
    private const string UploadsFolder = "Uploads";

    public AssessmentController(IWebHostEnvironment environment)
    {
        var mapper = new AssessmentGateway();
        _manager = new AssessmentManager(mapper);
        _environment = environment;
            
    }
    
    
    [Route("")]
    [HttpGet]
    public async Task<IActionResult> list()
    {
        string userRole = HttpContext.Session.GetString("Role") ?? "Unknown";

        if (userRole != "Doctor")
        {
            TempData["ErrorMessage"] = "Unauthorized access";
            return View("~/Views/Home/Index.cshtml");
        }

        try
        {
            List<Assessment> assessments = await _manager.getAssessments();

            var assessmentList = assessments.Select(s => s.getAssessmentDetails()).ToList();

            return View("~/Views/M3T1/Assessment/List.cshtml", assessmentList);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error loading assessments: {ex.Message}";
            return View("~/Views/M3T1/Assessment/List.cshtml", new List<Assessment>());
        }
    }

    [Route("View/{assessmentId}")]
    [HttpGet]
    public async Task<IActionResult> viewAssessment(string assessmentId)
    {
        string userRole = HttpContext.Session.GetString("Role") ?? "Unknown";

        if (userRole != "Doctor")
        {
            TempData["ErrorMessage"] = "Unauthorized access";
            return View("~/Views/Home/Index.cshtml");
        }

        var assessment = await _manager.getAssessment(assessmentId);
        if (assessment == null)
        {
            return RedirectToAction("list");
        }
        return View("~/Views/M3T1/Assessment/Index.cshtml", assessment);
    }

    [Route("Add")]
    [HttpGet]
    public IActionResult viewAdd()
    {
        string userRole = HttpContext.Session.GetString("Role") ?? "Unknown";

        if (userRole != "Patient" && userRole != "Caregiver")
        {
            TempData["ErrorMessage"] = "Unauthorized access";
            return View("~/Views/Home/Index.cshtml");
        }

        return View("~/Views/M3T1/Assessment/Add.cshtml");
    }

    //[Route("Delete/{assessmentId}")]
    //[HttpPost]
    //public async Task<IActionResult> deleteAssessment(string assessmentId)
    //{
    //    // First get the assessment to delete associated files
    //    var assessment = await _manager.getAssessment(assessmentId);
    //    if (assessment != null)
    //    {
    //        // Delete files from local storage
    //        foreach (var imagePath in assessment.getImagePath())
    //        {
    //            var physicalPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
    //            if (System.IO.File.Exists(physicalPath))
    //            {
    //                System.IO.File.Delete(physicalPath);
    //            }
    //        }
    //    }

    //    await _manager.deleteAssessment(assessmentId);
    //    TempData["SuccessMessage"] = "Assessment deleted successfully!";
    //    return RedirectToAction("List");
    //}

    [Route("Add")]
    [HttpPost]
    public async Task<IActionResult> addAssessment(IFormFile file)
    {
        string patientId = HttpContext.Session.GetString("UserID") ?? "";
        if (string.IsNullOrEmpty(patientId))
        {
            TempData["ErrorMessage"] = "Please log in to access";
            return View("~/Views/Home/Index.cshtml");
        }

        try
        {
            if (file == null)
            {
                TempData["ErrorMessage"] = "Please upload a file.";
                return View("~/Views/M3T1/Assessment/Add.cshtml");
            }

            var uploadsPath = Path.Combine(_environment.WebRootPath, UploadsFolder);
            
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Process file
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".webm", ".ogg", ".mov" };

            if (!allowedExtensions.Contains(fileExtension))
            {
                TempData["ErrorMessage"] = "Only image and video files are allowed.";
                return View("~/Views/M3T1/Assessment/Add.cshtml");
            }

            var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            string id = await _manager.insertAssessment(
                imagePath: $"/{UploadsFolder}/{uniqueFileName}",
                patientId: patientId
            );

            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Error inserting new assessment";
                return View("~/Views/M3T1/Assessment/Add.cshtml");
            }

            TempData["SuccessMessage"] = "Living conditions uploaded successfully!";
            return RedirectToAction("list");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error: {ex.Message}";
            return View("~/Views/M3T1/Assessment/Add.cshtml");
        }
    }

    [Route("Edit/{assessmentId}")]
    [HttpGet]
    public async Task<IActionResult> viewEdit(string assessmentId)
    {
        string userRole = HttpContext.Session.GetString("Role") ?? "Unknown";

        if (userRole != "Doctor")
        {
            TempData["ErrorMessage"] = "Unauthorized access";
            return View("~/Views/Home/Index.cshtml");
        }

        var assessment = await _manager.getAssessment(assessmentId);
        if (assessment == null)
        {
            return RedirectToAction("list");
        }

        ViewBag.HazardTypes = new List<string>
        {
            "Fire Safety",
            "Fall Risk",
            "Wet Condition"
        };

        var assessmentDetails = assessment.getAssessmentDetails();

        Dictionary<string, bool> defaultChecklist = new Dictionary<string, bool>();

        if (assessmentDetails.ContainsKey("HazardType") && !string.IsNullOrEmpty(assessmentDetails["HazardType"]?.ToString())){
            _manager.setHazardType(assessmentDetails["HazardType"].ToString());
            defaultChecklist = _manager.getDefaultChecklist();
        }

        if (assessmentDetails.ContainsKey("HomeAssessmentChecklist") && assessmentDetails["HomeAssessmentChecklist"] is Dictionary<string, bool> rawChecklist)
        {
            foreach (var key in rawChecklist.Keys)
            {
                if (defaultChecklist.ContainsKey(key))
                {
                    defaultChecklist[key] = rawChecklist[key];
                }
            }
        }
        
        ViewBag.ChecklistItems = defaultChecklist;

        return View("~/Views/M3T1/Assessment/Edit.cshtml", assessment);
    }

    [Route("Edit/{assessmentId}")]
    [HttpPost]
    public async Task<IActionResult> updateAssessment(
        string assessmentId,
        string riskLevel,
        string recommendation,
        string hazardType,
        Dictionary<string, bool> checklist)
    {
        try
        {
            var assessment = await _manager.getAssessment(assessmentId);
            if (assessment == null)
            {
                return RedirectToAction("list");
            }

            bool success = await _manager.updateAssessment(
                id: assessmentId,
                riskLevel: riskLevel,
                recommendation: recommendation,
                hazardType: hazardType,
                checklist: checklist
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Failed to update assessment.";
                return RedirectToAction("Edit", new { assessmentId });
            }

            TempData["SuccessMessage"] = "Assessment updated successfully!";
            return RedirectToAction("list");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error: {ex.Message}";
            return RedirectToAction("Edit", new { assessmentId });
        }
    }

    [Route("GetChecklist")]
    [HttpGet]
    public IActionResult GetChecklist([FromQuery] string hazardType)
    {
        try
        {
            _manager.setHazardType(hazardType);
            var checklist = _manager.getDefaultChecklist();
            return Json(checklist);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }


}