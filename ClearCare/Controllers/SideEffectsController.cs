using ClearCare.Domain;
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

        public async Task<IActionResult> Index()
        {
            var sideEffects = await _sideEffectControl.GetSideEffectsAsync();
            return View(sideEffects);
        }
    }
}
