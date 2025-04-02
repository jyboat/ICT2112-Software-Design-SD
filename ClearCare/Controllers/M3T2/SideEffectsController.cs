using ClearCare.Models.Control.M3T2;
using ClearCare.Models.Entities.M3T2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClearCare.Controllers.M3T2
{
    public class SideEffectsController : Controller
    {
        private readonly SideEffectControl _sideEffectControl;

        public SideEffectsController(SideEffectControl sideEffectControl)
        {
            _sideEffectControl = sideEffectControl;
        }

        // GET: Render the form for adding a new side effect
        [HttpGet]
        public async Task<IActionResult> add()
        {
            string userRole = HttpContext.Session.GetString("UserRole") ?? "Unknown";
            string userUUID = HttpContext.Session.GetString("UserUUID") ?? "Unknown";

            if (userRole == "Patient")
            {
                var medications = await _sideEffectControl.getPatientMedications(userRole, userUUID);
                ViewData["Medications"] = medications;
            }

            return View("~/Views/M3T2/SideEffects/Add.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> index()
        {
            string userRole = HttpContext.Session.GetString("UserRole") ?? "Unknown";
            string userUUID = HttpContext.Session.GetString("UserUUID") ?? "Unknown";

            var sideEffects = await _sideEffectControl.getSideEffectsAsync(userRole, userUUID);
            
            return View("~/Views/M3T2/SideEffects/Index.cshtml", sideEffects);
        }

        // Handle the form submission
        [HttpPost]
        public async Task<IActionResult> add(SideEffectModel sideEffect)
        {
            if (ModelState.IsValid)
            {
                await _sideEffectControl.addSideEffectAsync(sideEffect);
                return RedirectToAction("index");
            }

            return View("~/Views/M3T2/SideEffects/Add.cshtml", sideEffect);
        }

        [HttpGet]
        public async Task<IActionResult> chart()
        {
            string userRole = HttpContext.Session.GetString("UserRole") ?? "Unknown";
            string userUUID = HttpContext.Session.GetString("UserUUID") ?? "Unknown";

            var sideEffects = await _sideEffectControl.getSideEffectsAsync(userRole, userUUID);
            
            // Group by DrugName (example)
            var drugGroups = sideEffects
                .GroupBy(se => se.DrugName)
                .Select(g => new { DrugName = g.Key, Count = g.Count() })
                .ToList();

            ViewBag.ChartData = drugGroups;
            return View("~/Views/M3T2/SideEffects/Chart.cshtml", sideEffects);
        }
    }
}
