using ClearCare.DataSource.M3T1;
using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Control.M3T1;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T2;
using ClearCare.Gateways;
using ClearCare.Models.ViewModels;

// Request Handling
[Route("Summary")]
public class SummaryController : Controller
{
    private readonly DischargeSummaryManager _manager;

    public SummaryController()
    {
        var gateway = new SummaryGateway();
        IFetchPrescriptions fetcher = new PrescriptionMapper();
        _manager = new DischargeSummaryManager(gateway, fetcher);
        gateway.receiver = _manager;
    }

    [Route("")]
    [HttpGet]
    public async Task<IActionResult> List()
    {
        List<DischargeSummary> summaries = await _manager.getSummaries();

        var summaryList = summaries.Select(s => s.GetSummaryDetails()).ToList();  

        return View("~/Views/M3T1/Summary/List.cshtml", summaryList); 
    }

    [Route("View/{summaryId}")]
    [HttpGet]
    public async Task<IActionResult> ViewSummary(string summaryId)
    {
        var summary = await _manager.getSummary(summaryId);
        string patientId = (string)summary.GetSummaryDetails()["PatientId"];

        var prescription = await _manager.getPrescription(patientId);

        if (summary == null)
        {
            return View("List");
        }
        var viewModel = new SummaryViewModel
        {
            Summary = summary,
            Prescription = prescription
        };

        return View("~/Views/M3T1/Summary/Index.cshtml", viewModel);
    }

    [Route("Add")]
    [HttpGet]
    public IActionResult ViewAdd() { 
        return View("~/Views/M3T1/Summary/Add.cshtml");
    }

    [Route("Add")]
    [HttpPost]
    public async Task<IActionResult> AddSummary(string details, string instructions, string date)
    {
        if (string.IsNullOrEmpty(details) || string.IsNullOrEmpty(instructions) || string.IsNullOrEmpty(date))
        {
            TempData["ErrorMessage"] = "Please fill in all required fields";
            return View("~/Views/M3T1/Summary/Add.cshtml");
        }

        DateTime parsedDate;
        if (!DateTime.TryParse(date, out parsedDate))
        {
            TempData["ErrorMessage"] = "Invalid date format";
            return View("~/Views/M3T1/Summary/Add.cshtml");
        }

        string formattedDate = parsedDate.ToString("yyyy-MM-dd");
        // Process the summary here
        string id = await _manager.generateSummary(details, instructions, formattedDate, "1");

        TempData["SuccessMessage"] = "Summary added successfully!";

        return RedirectToAction("List");
    }

    [Route("Edit/{summaryId}")]
    [HttpGet]
    public async Task<IActionResult> ViewEdit(string summaryId)
    {
        var summary = await _manager.getSummary(summaryId);
        if (summary == null)
        {
            return RedirectToAction("List");
        }
        return View("~/Views/M3T1/Summary/Edit.cshtml", summary);
    }

    [Route("Edit/{summaryId}")]
    [HttpPost]
    public async Task<IActionResult> UpdateSummary(string summaryId, string details, string instructions, string date)
    {
        if (string.IsNullOrEmpty(details) || string.IsNullOrEmpty(instructions))
        {
            TempData["ErrorMessage"] = "Please fill in all required fields";
        }

        DateTime parsedDate;
        if (!DateTime.TryParse(date, out parsedDate))
        {
            TempData["ErrorMessage"] = "Invalid date format";
            return View("~/Views/M3T1/Summary/Add.cshtml");
        }

        string formattedDate = parsedDate.ToString("yyyy-MM-dd");

        await _manager.updateSummary(summaryId, details, instructions, formattedDate, "1");

        TempData["SuccessMessage"] = "Summary updated successfully!";

        return RedirectToAction("List");
    }

    [Route("Delete/{summaryId}")]
    [HttpPost]
   public async Task<IActionResult> DeleteSummary(string summaryId)
    {
        await _manager.deleteSummary(summaryId);

        TempData["SuccessMessage"] = "Summary deleted successfully!";

        return RedirectToAction("List");
    }
}
