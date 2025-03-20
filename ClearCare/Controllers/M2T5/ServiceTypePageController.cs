using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;

namespace ClearCare.Controllers
{
    public class ServiceTypePageController : Controller
    {
        private ServiceTypeManager serviceManager = new ServiceTypeManager();

        public async Task<IActionResult> Index()
        {
            var serviceTypes = await serviceManager.GetServiceTypes();
            return View("~/Views/M2T5/ServiceType/ServiceType.cshtml", serviceTypes);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, int duration, string requirements)
        {
            await serviceManager.CreateServiceType(name, duration, requirements);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string name, int duration, string requirements)
        {
            await serviceManager.UpdateServiceType(id, name, duration, requirements);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await serviceManager.DeleteServiceType(id);
            return RedirectToAction("Index");
        }
    }
}
