using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using ClearCare.Models;


namespace ClearCare.Controllers
{
    public class ServiceTypePageController : Controller
    {
        private ServiceTypeManager serviceManager = new ServiceTypeManager();

        public async Task<IActionResult> Index(string searchTerm)
        {
            var serviceTypes = await serviceManager.GetServiceTypes();

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
        public async Task<JsonResult> SearchServices(string term)
        {
            var all = await serviceManager.GetServiceTypes();

            var results = all
                .Where(s =>
                    s.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    s.Modality.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    s.Requirements.Contains(term, StringComparison.OrdinalIgnoreCase)
                )
                .Select(s => new
                {
                    id = s.ServiceTypeId,
                    name = s.Name,
                    modality = s.Modality
                })
                .ToList();

            return Json(results);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, int duration, string requirements, string modality)
        {
            await serviceManager.CreateServiceType(name, duration, requirements, modality);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string name, int duration, string requirements, string modality)
        {
            await serviceManager.UpdateServiceType(id, name, duration, requirements, modality);
            TempData["SuccessMessage"] = "Service updated successfully.";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmDiscontinue(int id)
        {
            var serviceTypes = await serviceManager.GetServiceTypes();
            var service = serviceTypes.Find(s => s.ServiceTypeId == id);

            var appointmentChecker = new ServiceAppointmentStatusManagement();
            var allAppointments = await appointmentChecker.getAppointmentDetails();

            var upcomingApptIds = allAppointments
                .Where(appt => appt.GetAttribute("ServiceTypeId") == id.ToString())
                .Select(appt => appt.GetAttribute("AppointmentId"))
                .ToList();

            ViewBag.UpcomingAppointmentIds = upcomingApptIds;

            return PartialView("~/Views/M2T5/ServiceType/_ConfirmDiscontinue.cshtml", service);

        }


        [HttpPost]
        public async Task<IActionResult> DiscontinueConfirmed(int id)
        {
            await serviceManager.DiscontinueServiceType(id);
            return RedirectToAction("Index");
        }


    }
}
