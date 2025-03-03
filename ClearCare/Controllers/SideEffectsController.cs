using Microsoft.AspNetCore.Mvc;
using ClearCare.Models;
using ClearCare.Controls;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ClearCare.Controllers
{
    public class SideEffectsController : Controller
    {
        private readonly ILogger<SideEffectsController> _logger;
        private readonly SideEffectControl _sideEffectControl;

        public SideEffectsController(ILogger<SideEffectsController> logger, SideEffectControl sideEffectControl)
        {
            _logger = logger;
            _sideEffectControl = sideEffectControl;
        }

        // GET: /SideEffects
        public async Task<IActionResult> Index()
        {
            // Display all side effects
            var allSideEffects = await _sideEffectControl.FetchAllSideEffectsAsync();
            return View(allSideEffects);  // e.g., an Index.cshtml listing them
        }

        // GET: /SideEffects/Create
        public IActionResult Create()
        {
            // Return a form to create a new side effect
            return View();
        }

        // POST: /SideEffects/Create
        [HttpPost]
        public async Task<IActionResult> Create(SideEffect sideEffect)
        {
            if (!ModelState.IsValid)
            {
                return View(sideEffect);
            }

            await _sideEffectControl.CreateSideEffectAsync(sideEffect);

            // Redirect back to the index or a success page
            return RedirectToAction(nameof(Index));
        }

        // // GET: /SideEffects/Edit?id=xxx
        // public IActionResult Edit(string id)
        // {
        //     var sideEffect = _sideEffectControl.FetchSideEffectById(id);
        //     if (sideEffect == null)
        //     {
        //         return NotFound($"SideEffect with ID {id} not found.");
        //     }
        //     return View(sideEffect);
        // }

        // // POST: /SideEffects/Edit
        // [HttpPost]
        // public async Task<IActionResult> Edit(SideEffect sideEffect)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return View(sideEffect);
        //     }

        //     await _sideEffectControl.UpdateSideEffectAsync(sideEffect);
        //     return RedirectToAction(nameof(Index));
        // }

        // // GET: /SideEffects/Delete?id=xxx
        // public IActionResult Delete(string id)
        // {
        //     var sideEffect = _sideEffectControl.FetchSideEffectById(id);
        //     if (sideEffect == null)
        //     {
        //         return NotFound($"SideEffect with ID {id} not found.");
        //     }
        //     return View(sideEffect);
        // }

        // // POST: /SideEffects/Delete
        // [HttpPost]
        // public async Task<IActionResult> DeleteConfirmed(string id)
        // {
        //     await _sideEffectControl.DeleteSideEffectAsync(id);
        //     return RedirectToAction(nameof(Index));
        // }
    }
}
