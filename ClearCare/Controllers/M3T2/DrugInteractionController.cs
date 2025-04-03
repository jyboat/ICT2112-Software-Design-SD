using ClearCare.Controls;
using ClearCare.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace ClearCare.Controllers.M3T2
{
    public class DrugInteractionController : Controller
    {
        private readonly DrugInteractionControl _drugInteractionControl;

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
            var result = await _drugInteractionControl.GetDrugInteractionAsync(drugName1, drugName2);
            TempData["SuccessMessage"] = "Drug interaction added successfully!";
            return View("~/Views/M3T2/DrugInteraction/Add.cshtml", result);
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
}
