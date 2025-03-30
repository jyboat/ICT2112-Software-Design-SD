using ClearCare.DataSource.M3T1;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.Models.Control.M3T1;
using ClearCare.Models.Entities.M3T1;
using Microsoft.AspNetCore.Hosting;
using System.IO;

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
        return View("~/Views/M3T1/Assessment/Add.cshtml");
    }

    //[Route("Edit/{assessmentId}")]
    //[HttpGet]
    //public async Task<IActionResult> viewEdit(string assessmentId)
    //{
    //    var assessment = await _manager.getAssessment(assessmentId);
    //    if (assessment == null)
    //    {
    //        return RedirectToAction("List");
    //    }

    //    // set hazard type from assessment before getting doctors/checklist
    //    _manager.setHazardType(assessment.getHazardType());
        
    //    ViewBag.QualifiedDoctors = _manager.getQualifiedDoctors();
    //    ViewBag.ChecklistItems = _manager.getDefaultChecklist();

    //    return View("~/Views/M3T1/Assessment/Edit.cshtml", assessment);
    //}

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
                imagePath: $"/{UploadsFolder}/{uniqueFileName}"
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

        // _manager.setHazardType(assessment.getHazardType());
        ViewBag.ChecklistItems = _manager.getDefaultChecklist();

        return View("~/Views/M3T1/Assessment/Edit.cshtml", assessment);
    }

    [Route("Edit/{assessmentId}")]
    [HttpPost]
    public async Task<IActionResult> updateAssessment(
        string assessmentId,
        string riskLevel,
        string recommendation,
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
        
}