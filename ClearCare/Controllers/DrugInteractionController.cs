using ClearCare.Controls;
using ClearCare.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Json;

namespace ClearCare.Controllers
{
    public class DrugInteractionController : Controller
    {
        private readonly DrugInteractionControl _drugInteractionControl;
        private readonly HttpClient _httpClient;

        public DrugInteractionController(DrugInteractionControl DrugControl, HttpClient httpClient)
        {
            _drugInteractionControl = DrugControl;
            _httpClient = httpClient;
        }

        [HttpGet]
        public IActionResult index() {
            return View();
        }

        // Handle form submission
        [HttpPost]
        public async Task<IActionResult> Add(string drugName)
        {
            string reactionsUrl = $"https://api.fda.gov/drug/event.json?search=patient.drug.openfda.substance_name.exact:'{drugName}'&count=patient.reaction.reactionmeddrapt.exact";
            string labelingUrl = $"https://api.fda.gov/drug/label.json?search=openfda.substance_name:'{drugName}'&limit=1";

            try
            {
                var reactionsTask = _httpClient.GetStringAsync(reactionsUrl);
                var labelingTask = _httpClient.GetStringAsync(labelingUrl);

                await Task.WhenAll(reactionsTask, labelingTask);
                ViewBag.DrugName = drugName;
                var reactionsJson = JsonDocument.Parse(await reactionsTask);
                var labelingJson = JsonDocument.Parse(await labelingTask);

                // Extract reactions safely
                ViewBag.Reactions = reactionsJson.RootElement.GetProperty("results").EnumerateArray().Select(r => new
                {
                    Term = r.GetProperty("term").GetString(),
                    Count = r.GetProperty("count").GetInt32()
                }).ToList();

                // Extract warnings safely
                ViewBag.Labeling = labelingJson.RootElement.TryGetProperty("results", out var labelingArray)
                ? labelingArray.EnumerateArray().Select(r => new
                {
                    Warning = r.TryGetProperty("warnings", out var warningProp) ? warningProp.ToString() : "No warnings available",
                    Precautions = r.TryGetProperty("precautions", out var precautionsProp) ? precautionsProp.ToString() : "No precautions available"
                }).ToList<dynamic>() // Explicitly cast to List<dynamic>
                : new List<dynamic>(); // Ensure both branches return the same type

                    return View("Index"); // Reloads the Index view with data
                }
                catch (HttpRequestException e)
                {
                    return StatusCode(500, $"Request failed: {e.Message}");
            }
        }
    }
}
