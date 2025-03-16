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
        public IActionResult Add()
        {
            return View("~/Views/M3T2/SideEffects/Add.cshtml");
        }


        public async Task<IActionResult> Index()
        {
            var sideEffects = await _sideEffectControl.GetSideEffectsAsync();
            return View("~/Views/M3T2/SideEffects/Index.cshtml", sideEffects);
        }

        // Handle the form submission
        [HttpPost]
        public async Task<IActionResult> Add(SideEffectModel sideEffect)
        {
            if (ModelState.IsValid)
            {
                await _sideEffectControl.AddSideEffectAsync(sideEffect);
                return RedirectToAction("Index");
            }

            return View(sideEffect);
        }

    }
}