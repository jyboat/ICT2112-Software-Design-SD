using ClearCare.Controls;
// using ClearCare.Models.DTO.M3T2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Json;
using System.Web;
using System.Text.Json.Serialization;
using System.Text;


namespace ClearCare.Controllers.M3T2
{
    public class DrugInteractionController : Controller
    {
        private readonly DrugInteractionControl _drugInteractionControl;
        private readonly HttpClient _httpClient;

        /// <summary>
        ///   Initializes a new instance of the
        ///   <see cref="DrugInteractionController"/> class.
        /// </summary>
        /// <param name="DrugControl">
        ///   The DrugInteractionControl instance for handling drug interaction
        ///   logic.
        /// </param>
        /// <param name="httpClient">
        ///   The HttpClient instance for making HTTP requests (not currently
        ///   used, but kept for potential future use).
        /// </param>
        public DrugInteractionController(
            DrugInteractionControl DrugControl,
            HttpClient httpClient
        )
        {
            _drugInteractionControl = DrugControl;
            _httpClient = httpClient;
        }

        /// <summary>
        ///   Displays the main index view for the drug interaction section.
        /// </summary>
        /// <returns>The Index view.</returns>
        [HttpGet]
        public IActionResult index()
        {
            return View("~/Views/M3T2/DrugInteraction/Index.cshtml");
        }

        /// <summary>
        ///   Displays the view for adding a new drug interaction.
        /// </summary>
        /// <returns>The Add view.</returns>
        [HttpGet]
        public IActionResult add()
        {
            return View("~/Views/M3T2/DrugInteraction/Add.cshtml");
        }

        /// <summary>
        ///   Retrieves drug interaction information based on two drug names.
        /// </summary>
        /// <param name="drugName1">The name of the first drug.</param>
        /// <param name="drugName2">The name of the second drug.</param>
        /// <returns>
        ///   A view displaying the drug interaction information, or the Index
        ///   view if no interaction is found.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Add(string drugName1, string drugName2)
        {
            string ddinterApi = $"https://portfolio-website-lyart-five-75.vercel.app/api/receive?drug1={drugName1}&drug2={drugName2}";
            try
            {
                var resp = await _httpClient.GetStringAsync(ddinterApi);
                var result = JsonSerializer.Deserialize<DrugInteractionResponse>(resp);
                return View("~/Views/M3T2/DrugInteraction/Index.cshtml", result);
            }
            catch (HttpRequestException e)
            {
                return View("~/Views/M3T2/DrugInteraction/Index.cshtml", new DrugInteractionResponse
                {
                    Results = new List<DrugInteraction>
                    {
                        new DrugInteraction { Drug1Name = "Error", Drug2Name = "Error", Interaction = $"No known interaction" }
                    }
                });
            }        
        }

        /// <summary>
        ///   Uploads a new drug interaction to the system.
        /// </summary>
        /// <param name="drugName1">The name of the first drug.</param>
        /// <param name="drugName2">The name of the second drug.</param>
        /// <param name="interaction">The interaction description.</param>
        /// <returns>
        ///   A redirect to the Index action, with a success or error message
        ///   stored in TempData.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Upload(
            string drugName1,
            string drugName2,
            string interaction
        )
        {
            bool success = await _drugInteractionControl.UploadInteractionAsync(
                drugName1,
                drugName2,
                interaction
            );

            if (success)
            {
                TempData["SuccessMessage"] = "Interaction successfully uploaded!";
            }
            else
            {
                TempData["ErrorMessage"] = "Error uploading interaction.";
            }

            return RedirectToAction("Index");
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
}



