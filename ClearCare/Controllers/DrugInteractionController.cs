using ClearCare.Controls;
using ClearCare.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace ClearCare.Controllers
{
    public class DrugInteractionController : Controller
    {
        private readonly DrugInteractionControl _drugInteractionControl;

        public DrugInteractionController(DrugInteractionControl DrugControl, HttpClient httpClient)
        {
            _drugInteractionControl = DrugControl;
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
            var result = await _drugInteractionControl.GetDrugInteractionAsync(drugName1, drugName2);
            return View("Index", result);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(string drugName1, string drugName2, string interaction)
        {
            bool success = await _drugInteractionControl.UploadInteractionAsync(drugName1, drugName2, interaction);
            TempData["Message"] = success ? "Interaction successfully uploaded!" : "Error uploading interaction.";
            return RedirectToAction("Index");
        }
    }
}