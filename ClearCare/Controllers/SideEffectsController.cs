using ClearCare.Controls;
using ClearCare.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClearCare.Controllers
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
        public IActionResult add()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> index()
        {
            var sideEffects = await _sideEffectControl.getSideEffectsAsync();
            return View(sideEffects);
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

            return View(sideEffect);
        }
    }
}
