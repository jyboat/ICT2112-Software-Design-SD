using ClearCare.Controls;
using ClearCare.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClearCare.Controllers
{
    public class SideEffectsController : Controller
    {
        private readonly SideEffectControl _sideEffectControl;

        /// <summary>
        ///   Initializes a new instance of the
        ///   <see cref="SideEffectsController"/> class.
        /// </summary>
        /// <param name="sideEffectControl">
        ///   The control responsible for handling side effect-related
        ///   operations.
        /// </param>
        public SideEffectsController(SideEffectControl sideEffectControl)
        {
            _sideEffectControl = sideEffectControl;
        }

        /// <summary>
        ///   Renders the form for adding a new side effect.
        /// </summary>
        /// <returns>The Add view, populated with medication data for
        ///   patients.</returns>
        // GET: Render the form for adding a new side effect
        [HttpGet]
        public async Task<IActionResult> add()
        {
            string userRole = HttpContext.Session.GetString("Role") ?? "Unknown";
            string userUUID = HttpContext.Session.GetString("UserID") ?? "Unknown";

            if (userRole == "Patient")
            {
                var medications = await _sideEffectControl.getPatientMedications(
                    userRole,
                    userUUID
                );
                ViewData["Medications"] = medications;
            }

            return View();
        }

        /// <summary>
        ///   Displays a list of side effects.
        /// </summary>
        /// <returns>
        ///   The Index view with a list of side effects based on the user's
        ///   role.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> index()
        {
            string userRole = HttpContext.Session.GetString("Role") ?? "Unknown";
            string userUUID = HttpContext.Session.GetString("UserID") ?? "Unknown";

            var sideEffects = await _sideEffectControl.getSideEffectsAsync(
                userRole,
                userUUID
            );

            return View(sideEffects);
        }

        /// <summary>
        ///   Handles the submission of the new side effect form.
        /// </summary>
        /// <param name="sideEffect">The SideEffectModel containing the side
        ///   effect data.</param>
        /// <returns>
        ///   If the model is valid, adds the side effect and redirects to the
        ///   Index action; otherwise, returns the Add view with validation
        ///   errors.
        /// </returns>
        // Handle the form submission
        [HttpPost]
        public async Task<IActionResult> add(SideEffectModel sideEffect)
        {
            if (ModelState.IsValid)
            {
                await _sideEffectControl.addSideEffectAsync(sideEffect);
                return RedirectToAction("index");
            }

            return View(sideEffect);
        }

        /// <summary>
        ///   Displays a chart of side effects, grouped by drug name.
        /// </summary>
        /// <returns>The Chart view with side effect data grouped for
        ///   charting.</returns>
        [HttpGet]
        public async Task<IActionResult> chart()
        {
            string userRole = HttpContext.Session.GetString("Role") ?? "Unknown";
            string userUUID = HttpContext.Session.GetString("UserID") ?? "Unknown";

            var sideEffects = await _sideEffectControl.getSideEffectsAsync(
                userRole,
                userUUID
            );

            // Group by DrugName (example)
            var drugGroups = sideEffects
                .GroupBy(se => se.DrugName)
                .Select(g => new { DrugName = g.Key, Count = g.Count() })
                .ToList();

            ViewBag.ChartData = drugGroups;
            return View(sideEffects);
        }
    }
}
