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
        var mapper = new AssessmentMapper();
        _manager = new AssessmentManager(mapper);
        _environment = environment;
            
    }
    
    
    [Route("")]
    [HttpGet]
    public async Task<IActionResult> List()
    {
        try
        {
            List<Assessment> assessments = await _manager.getAssessments();
            return View("~/Views/M3T1/Assessment/List.cshtml", assessments);
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
            return RedirectToAction("List");
        }
        return View("~/Views/M3T1/Assessment/Index.cshtml", assessment);
    }

    [Route("Add")]
    [HttpGet]
    public IActionResult viewAdd()
    {
        // Initialize with default hazard type or empty
        ViewBag.HazardTypes = new List<string> 
        { 
            "Fire Safety", 
            "Fall Risk", 
            "Wet Condition" 
        };
        return View("~/Views/M3T1/Assessment/Add.cshtml");
    }

    [Route("Edit/{assessmentId}")]
    [HttpGet]
    public async Task<IActionResult> viewEdit(string assessmentId)
    {
        var assessment = await _manager.getAssessment(assessmentId);
        if (assessment == null)
        {
            return RedirectToAction("List");
        }

        // set hazard type from assessment before getting doctors/checklist
        _manager.setHazardType(assessment.getHazardType());
        
        ViewBag.QualifiedDoctors = _manager.getQualifiedDoctors();
        ViewBag.ChecklistItems = _manager.getDefaultChecklist();

        return View("~/Views/M3T1/Assessment/Edit.cshtml", assessment);
    }

    [Route("Delete/{assessmentId}")]
    [HttpPost]
    public async Task<IActionResult> deleteAssessment(string assessmentId)
    {
        // First get the assessment to delete associated files
        var assessment = await _manager.getAssessment(assessmentId);
        if (assessment != null)
        {
            // Delete files from local storage
            foreach (var imagePath in assessment.getImagePath())
            {
                var physicalPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
            }
        }

        await _manager.deleteAssessment(assessmentId);
        TempData["SuccessMessage"] = "Assessment deleted successfully!";
        return RedirectToAction("List");
    }

    [Route("Add")]
    [HttpPost]
    public async Task<IActionResult> addAssessment(string hazardType, string doctorId, IFormFile file)
    {
        try
        {
            _manager.setHazardType(hazardType);

            if (string.IsNullOrEmpty(hazardType) || string.IsNullOrEmpty(doctorId) || file == null)
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return View("~/Views/M3T1/Assessment/Add.cshtml");
            }

            List<string> imagePaths = new List<string>();
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

            imagePaths.Add($"/{UploadsFolder}/{uniqueFileName}");

            string id = await _manager.insertAssessment(
                hazardType: hazardType,
                doctorId: doctorId,
                imagePath: imagePaths
            );

            TempData["SuccessMessage"] = "Assessment added successfully!";
            return RedirectToAction("List");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error: {ex.Message}";
            return View("~/Views/M3T1/Assessment/Add.cshtml");
        }
    }

    [Route("Edit/{assessmentId}")]
    [HttpPost]
    public async Task<IActionResult> updateAssessment(
        string id,
        string doctorId,
        IFormFile file,
        string existingFilePath,
        bool deleteFile)
    {
        try
        {
            var assessment = await _manager.getAssessment(id);
            _manager.setHazardType(assessment.getHazardType());

            if (string.IsNullOrEmpty(doctorId))
            {
                TempData["ErrorMessage"] = "Please select a doctor.";
                return RedirectToAction("Edit", new { id });
            }

            List<string> imagePaths = new List<string>();

            // Handle file operations
            if (deleteFile && !string.IsNullOrEmpty(existingFilePath))
            {
                var physicalPath = Path.Combine(_environment.WebRootPath, existingFilePath.TrimStart('/'));
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
            }
            else if (file != null && file.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingFilePath))
                {
                    var oldPhysicalPath = Path.Combine(_environment.WebRootPath, existingFilePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldPhysicalPath))
                    {
                        System.IO.File.Delete(oldPhysicalPath);
                    }
                }

                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".webm", ".ogg", ".mov" };

                if (!allowedExtensions.Contains(fileExtension))
                {
                    TempData["ErrorMessage"] = "Only image and video files are allowed.";
                    return RedirectToAction("Edit", new { id });
                }

                var uploadsPath = Path.Combine(_environment.WebRootPath, UploadsFolder);
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                imagePaths.Add($"/{UploadsFolder}/{uniqueFileName}");
            }
            else if (!string.IsNullOrEmpty(existingFilePath))
            {
                imagePaths.Add(existingFilePath);
            }

            bool success = await _manager.updateAssessment(
                id: id,
                doctorId: doctorId,
                imagePath: imagePaths
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Failed to update assessment.";
                return RedirectToAction("Edit", new { id });
            }

            TempData["SuccessMessage"] = "Assessment updated successfully!";
            return RedirectToAction("List");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error: {ex.Message}";
            return RedirectToAction("Edit", new { id });
        }
    }

    [Route("DoctorEdit/{assessmentId}")]
    [HttpGet]
    public async Task<IActionResult> viewDoctorEdit(string assessmentId)
    {
        var assessment = await _manager.getAssessment(assessmentId);
        if (assessment == null)
        {
            return RedirectToAction("List");
        }

        _manager.setHazardType(assessment.getHazardType());
        ViewBag.ChecklistItems = _manager.getDefaultChecklist();

        return View("~/Views/M3T1/Assessment/DoctorEdit.cshtml", assessment);
    }

    [Route("DoctorEdit/{assessmentId}")]
    [HttpPost]
    public async Task<IActionResult> updateDoctorAssessment(
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
                return RedirectToAction("List");
            }

            bool success = await _manager.updateDoctorAssessment(
                id: assessmentId,
                riskLevel: riskLevel,
                recommendation: recommendation,
                checklist: checklist
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Failed to update assessment.";
                return RedirectToAction("DoctorEdit", new { assessmentId });
            }

            TempData["SuccessMessage"] = "Assessment updated successfully!";
            return RedirectToAction("List");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error: {ex.Message}";
            return RedirectToAction("DoctorEdit", new { assessmentId });
        }
    }


        
}