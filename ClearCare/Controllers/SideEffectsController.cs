using ClearCare.Domain;
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
        public IActionResult Add()
        {
            return View();
        }


        public async Task<IActionResult> Index()
        {
            var sideEffects = await _sideEffectControl.GetSideEffectsAsync();
            return View(sideEffects);
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