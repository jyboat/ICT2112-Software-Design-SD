using ClearCare.DataSource;
using ClearCare.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ClearCare.Controls;
using System.Text.Json;

// Request Handling
[Route("api/[controller]")]
[ApiController]
public class SummaryController : Controller
{
    private readonly DischargeSummaryManager _manager;

    public SummaryController()
    {
        _manager = new DischargeSummaryManager();
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        List<DischargeSummary> summaries = await _manager.getSummaries();

        var summaryList = summaries.Select(s => s.getDetails()).ToList();  

        return View("List", summaryList); 
    }

    [HttpGet]
    [Route("Add")]
    public IActionResult Add() { 
        return View("Add");
    }

    [HttpPost]
    [Route("Add")]
    public async Task<IActionResult> AddSummary([FromBody] Dictionary<string, JsonElement> data)
    {
        var summary = DischargeSummary.setDetails(
            data["id"].GetString() ?? "",
            data["details"].GetString() ?? "",
            data["instructions"].GetString() ?? "",
            data["createdAt"].GetDateTime(),
            data["patientId"].GetString() ?? ""
            );

        // Process the summary here
        string id = await _manager.generateSummary(summary);

        return RedirectToAction("List");
    }
   
}
