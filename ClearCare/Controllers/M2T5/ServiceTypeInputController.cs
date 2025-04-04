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
        private ServiceTypeManager serviceManager = new ServiceTypeManager();

        public async Task<IActionResult> Index(string searchTerm)
        {
            await serviceManager.fetchServiceTypes(); // 🔄 Asynchronous trigger
            var serviceTypes = serviceManager.getCachedServiceTypes(); // ✅ Access processed data


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
        
        public async Task<IActionResult> TestObserver()
        {
            await serviceManager.fetchServiceTypes();
            var types = serviceManager.getCachedServiceTypes();

            Console.WriteLine("Types fetched: " + types.Count);
            return Json(types); // optional
        }


        [HttpGet]
        public async Task<JsonResult> SearchServices(string term)
        {
            await serviceManager.fetchServiceTypes(); // 🔄 Asynchronous trigger
            var serviceTypes = serviceManager.getCachedServiceTypes(); // ✅ Access processed data


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
        public async Task<IActionResult> Create(string name, int duration, string requirements, string modality)
        {
            await serviceManager.fetchServiceTypes(); // 🔄 Asynchronous trigger
            var serviceTypes = serviceManager.getCachedServiceTypes(); // ✅ Access processed data

            bool nameExists = serviceTypes.Any(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (nameExists)
            {
                TempData["ErrorMessage"] = "A service with the same name already exists.";
                return RedirectToAction("Index");
            }

            await serviceManager.createServiceType(name, duration, requirements, modality);
            TempData["SuccessMessage"] = "Service added successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
            public async Task<IActionResult> Edit(int id, string name, int duration, string requirements, string modality)
            {
                await serviceManager.fetchServiceTypes(); // 🔄 Asynchronous trigger
                var serviceTypes = serviceManager.getCachedServiceTypes(); // ✅ Access processed data

                bool nameExists = serviceTypes.Any(s =>
                    s.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                    s.ServiceTypeId != id);

                if (nameExists)
                {
                    TempData["ErrorMessage"] = "Another service with the same name already exists.";
                    return RedirectToAction("Index");
                }

                await serviceManager.updateServiceType(id, name, duration, requirements, modality);
                TempData["SuccessMessage"] = "Service updated successfully.";
                return RedirectToAction("Index");
            }



        [HttpGet]
        public async Task<IActionResult> ConfirmDiscontinue(int id)
        {
            await serviceManager.fetchServiceTypes(); // 🔄 Asynchronous trigger
            var serviceTypes = serviceManager.getCachedServiceTypes(); // ✅ Access processed data

            var service = serviceTypes.Find(s => s.ServiceTypeId == id);

            var appointmentChecker = new ServiceAppointmentStatusManagement();
            var allAppointments = await appointmentChecker.getAppointmentDetails();

            var upcomingApptIds = allAppointments
                .Where(appt => appt.getAttribute("Service") == service.Name)
                .Select(appt => appt.getAttribute("AppointmentId"))
                .ToList();

            ViewBag.UpcomingAppointmentIds = upcomingApptIds;

            return PartialView("~/Views/M2T5/ServiceType/_ConfirmDiscontinue.cshtml", service);

        }


        [HttpPost]
        public async Task<IActionResult> DiscontinueConfirmed(int id)
        {
            await serviceManager.discontinueServiceType(id);
            TempData["SuccessMessage"] = "Service discontinued.";
            return RedirectToAction("Index");
        }


    }
}
