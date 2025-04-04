using ClearCare.DataSource.M3T1;
using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Control.M3T1;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T1;
using ClearCare.Models.DTO.M3T1;
using ClearCare.Models.Interfaces.M3T2;
using ClearCare.DataSource.M3T2;

// Request Handling
[Route("Summary")]
public class SummaryController : Controller
{
    private readonly DischargeSummaryManager _manager;

    public SummaryController()
    {
        var gateway = new SummaryGateway();
        IAssessment assessmentFetcher = new AssessmentGateway();
        IFetchPrescriptions prescriptionFetcher = new PrescriptionMapper();
        _manager = new DischargeSummaryManager(gateway, assessmentFetcher, prescriptionFetcher);
        gateway.receiver = _manager;
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


        List<DischargeSummary> summaries = await _manager.getSummaries();

        var summaryList = summaries.Select(s => s.GetSummaryDetails()).ToList();  

        return View("~/Views/M3T1/Summary/List.cshtml", summaryList); 
    }

    [Route("View/{summaryId}")]
    [HttpGet]
    public async Task<IActionResult> viewSummary(string summaryId)
    {
        string userRole = HttpContext.Session.GetString("Role") ?? "Unknown";

        if (userRole != "Doctor")
        {
            TempData["ErrorMessage"] = "Unauthorized access";
            return View("~/Views/Home/Index.cshtml");
        }

        var summary = await _manager.getSummary(summaryId);

        if (summary == null)
        {
            return View("list");
        }
        string patientId = (string)summary.GetSummaryDetails()["PatientId"];

        var prescription = await _manager.getPrescription(patientId);
        var assessment = await _manager.getAssessment(patientId);

        var dto = new SummaryDTO
        {
            Summary = summary,
            Assessment = assessment,
            Prescription = prescription
        };

        return View("~/Views/M3T1/Summary/Index.cshtml", dto);
    }

    [Route("Add")]
    [HttpGet]
    public IActionResult viewAdd() {
        string userRole = HttpContext.Session.GetString("Role") ?? "Unknown";

        if (userRole != "Doctor")
        {
            TempData["ErrorMessage"] = "Unauthorized access";
            return View("~/Views/Home/Index.cshtml");
        }
        return View("~/Views/M3T1/Summary/Add.cshtml");
    }

    [Route("Add")]
    [HttpPost]
    public async Task<IActionResult> addSummary(string details, string instructions, string patientId)
    {

        if (string.IsNullOrEmpty(details) || string.IsNullOrEmpty(instructions))
        {
            TempData["ErrorMessage"] = "Please fill in all required fields";
            return View("~/Views/M3T1/Summary/Add.cshtml");
        }

        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        // Process the summary here
        string id = await _manager.generateSummary(details, instructions, currentDate, patientId);

        if (!string.IsNullOrEmpty(id))
        {
            TempData["SuccessMessage"] = "Summary added successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Error adding summary";
        }

        return RedirectToAction("list");
    }

    [Route("Edit/{summaryId}")]
    [HttpGet]
    public async Task<IActionResult> viewEdit(string summaryId)
    {
        string userRole = HttpContext.Session.GetString("Role") ?? "Unknown";

        if (userRole != "Doctor")
        {
            TempData["ErrorMessage"] = "Unauthorized access";
            return View("~/Views/Home/Index.cshtml");
        }

        var summary = await _manager.getSummary(summaryId);
        if (summary == null)
        {
            return RedirectToAction("list");
        }
        return View("~/Views/M3T1/Summary/Edit.cshtml", summary);
    }

    [Route("Edit/{summaryId}")]
    [HttpPost]
    public async Task<IActionResult> updateSummary(string summaryId, string details, string instructions)
    {
        if (string.IsNullOrEmpty(details) || string.IsNullOrEmpty(instructions))
        {
            TempData["ErrorMessage"] = "Please fill in all required fields";
        }

        bool success = await _manager.updateSummary(summaryId, details, instructions);

        if (success)
        {
            TempData["SuccessMessage"] = "Summary updated successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Error updating summary";
        }

        return RedirectToAction("list");
    }

    [Route("Delete/{summaryId}")]
    [HttpPost]
   public async Task<IActionResult> updateSummaryStatus(string summaryId)
    {
        bool success = await _manager.updateSummaryStatus(summaryId);
        if (success)
        {
            TempData["SuccessMessage"] = "Summary deleted successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Error deleting summary";
        }

        return RedirectToAction("list");
    }
}
