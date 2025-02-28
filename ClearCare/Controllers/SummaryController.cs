using ClearCare.DataSource;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.Json;
using ClearCare.Models.Controls;
using ClearCare.Models.Entities;

// Request Handling
[Route("Summary")]
public class SummaryController : Controller
{
    private readonly DischargeSummaryManager _manager;

    public SummaryController()
    {
        _manager = new DischargeSummaryManager();
    }

    [Route("")]
    [HttpGet]
    public async Task<IActionResult> List()
    {
        List<DischargeSummary> summaries = await _manager.getSummaries();

        var summaryList = summaries.Select(s => s.GetSummaryDetails()).ToList();  

        return View("List", summaryList); 
    }

    [Route("Add")]
    [HttpGet]
    public IActionResult Add() { 
        return View("Add");
    }

    [Route("Add")]
    [HttpPost]
    public async Task<IActionResult> AddSummary(string details, string instructions)
    {
        if (string.IsNullOrEmpty(details) || string.IsNullOrEmpty(instructions))
        {
            ViewBag.ErrorMessage = "Please fill in all required fields";
        }

        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        // Process the summary here
        string id = await _manager.generateSummary(details, instructions, currentDate, "1");

        return RedirectToAction("List");
    }
   
}
