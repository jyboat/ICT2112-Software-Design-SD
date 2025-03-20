using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;

namespace ClearCare.Controllers
{
    public class ServiceTypePageController : Controller
    {
        private ServiceTypeManager serviceManager = new ServiceTypeManager();

        public IActionResult Index()
        {
            var serviceTypes = serviceManager.GetServiceTypes();
            return View("~/Views/M2T5/ServiceType/ServiceType.cshtml", serviceTypes);
        }

        [HttpPost]
        public IActionResult Create(string name, int duration, string requirements)
        {
            serviceManager.CreateServiceType(name, duration, requirements);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            serviceManager.DeleteServiceType(id);
            return RedirectToAction("Index");
        }
    }
}
