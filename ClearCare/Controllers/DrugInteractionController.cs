using ClearCare.Controls;
using ClearCare.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Json;
using System.Web;
using System.Text.Json.Serialization;
using System.Text;

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

        [HttpGet]
        public IActionResult add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(string drugName1, string drugName2)
        {
            string ddinterApi = $"https://portfolio-website-lyart-five-75.vercel.app/api/receive?drug1={drugName1}&drug2={drugName2}";
            try
            {
                var resp = await _httpClient.GetStringAsync(ddinterApi);
                var result = JsonSerializer.Deserialize<DrugInteractionResponse>(resp);
                return View("index", result);
            }
            catch (HttpRequestException e)
            {
                return View("index", new DrugInteractionResponse
                {
                    Results = new List<DrugInteraction>
                    {
                        new DrugInteraction { Drug1Name = "Error", Drug2Name = "Error", Interaction = $"No known interaction" }
                    }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upload(string drugName1, string drugName2, string interaction)
        {
            var newInteraction = new {
                Drug1Name = drugName1,
                Drug2Name = drugName2,
                Interaction = interaction
            };

            string json = JsonSerializer.Serialize(newInteraction);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            string ddinterApi = $"https://portfolio-website-lyart-five-75.vercel.app/api/receive";
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(ddinterApi, content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                TempData["SuccessMessage"] = "Interaction successfully uploaded!"; // Store success message

                return RedirectToAction("Index"); // Redirect to Index action
            }
            catch (HttpRequestException e)
            {
                TempData["ErrorMessage"] = $"Error: {e.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}

public class DrugInteractionResponse
{
    [JsonPropertyName("results")]
    public List<DrugInteraction> Results { get; set; }
}

public class DrugInteraction
{
    public string Drug1Name { get; set; }
    public string Drug2Name { get; set; }
    public string Interaction { get; set; }
}
