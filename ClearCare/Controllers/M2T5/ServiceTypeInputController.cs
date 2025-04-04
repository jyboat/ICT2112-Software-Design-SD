using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using ClearCare.Models;


namespace ClearCare.Controllers
{
    public class ServiceTypeInputController : Controller
    {
        private ServiceTypeManager _serviceTypeManager = new ServiceTypeManager();

        public async Task<IActionResult> Index(string searchTerm)
        {
            await _serviceTypeManager.fetchServiceTypes(); // 🔄 Asynchronous trigger
            var serviceTypes = _serviceTypeManager.getCachedServiceTypes(); // ✅ Access processed data


            if (!string.IsNullOrEmpty(searchTerm))
            {
                serviceTypes = serviceTypes
                    .Where(s =>
                        s.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        s.Requirements.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        s.Modality.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList();
            }

            ViewBag.SearchTerm = searchTerm;
            return View("~/Views/M2T5/ServiceType/ServiceType.cshtml", serviceTypes);
        }

        [HttpGet]
        public async Task<JsonResult> searchServices(string term)
        {
            await _serviceTypeManager.fetchServiceTypes(); // 🔄 Asynchronous trigger
            var serviceTypes = _serviceTypeManager.getCachedServiceTypes(); // ✅ Access processed data

            var results = serviceTypes
                .Where(s =>
                    s.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    s.Modality.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    s.Requirements.Contains(term, StringComparison.OrdinalIgnoreCase)
                )
                .Select(s => new
                {
                    id = s.ServiceTypeId,
                    name = s.Name,
                    modality = s.Modality,
                    requirements = s.Requirements,
                    status = s.Status
                })
                .ToList();

            return Json(results);
        }

        [HttpPost]
        public async Task<IActionResult> create(string name, int duration, string requirements, string modality)
        {
            await _serviceTypeManager.fetchServiceTypes(); // 🔄 Asynchronous trigger
            var serviceTypes = _serviceTypeManager.getCachedServiceTypes(); // ✅ Access processed data

            bool nameExists = serviceTypes.Any(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (nameExists)
            {
                TempData["ErrorMessage"] = "A service with the same name already exists.";
                return RedirectToAction("Index");
            }

            await _serviceTypeManager.createServiceType(name, duration, requirements, modality);
            TempData["SuccessMessage"] = "Service added successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> edit(int id, string name, int duration, string requirements, string modality)
        {
            await _serviceTypeManager.fetchServiceTypes(); // 🔄 Asynchronous trigger
            var serviceTypes = _serviceTypeManager.getCachedServiceTypes(); // ✅ Access processed data

            bool nameExists = serviceTypes.Any(s =>
                s.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                s.ServiceTypeId != id);

            if (nameExists)
            {
                TempData["ErrorMessage"] = "Another service with the same name already exists.";
                return RedirectToAction("Index");
            }

            await _serviceTypeManager.updateServiceType(id, name, duration, requirements, modality);
            TempData["SuccessMessage"] = "Service updated successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> confirmDiscontinue(int id)
        {
            await _serviceTypeManager.fetchServiceTypes();
            var serviceTypes = _serviceTypeManager.getCachedServiceTypes();
            var service = serviceTypes.Find(s => s.ServiceTypeId == id);
            
            // Call the ServiceTypeManager to get upcoming appointment IDs
            var upcomingApptIds = await _serviceTypeManager.getUpcomingAppointmentIdsAsync(service.Name);

            ViewBag.UpcomingAppointmentIds = upcomingApptIds;
            return PartialView("~/Views/M2T5/ServiceType/_ConfirmDiscontinue.cshtml", service);
        }



        [HttpPost]
        public async Task<IActionResult> discontinueConfirmed(int id)
        {
            await _serviceTypeManager.discontinueServiceType(id);
            TempData["SuccessMessage"] = "Service discontinued.";
            return RedirectToAction("Index");
        }


    }
}
