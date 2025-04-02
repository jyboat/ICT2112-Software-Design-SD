using ClearCare.Models.Control.M3T2;
using ClearCare.Models.Entities.M3T2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClearCare.Controllers.M3T2
{
    public class PrescriptionController : Controller
    {
        private readonly PrescriptionControl _prescriptionControl;

        public PrescriptionController(PrescriptionControl prescriptionControl)
        {
            _prescriptionControl = prescriptionControl;
        }

        // GET: /Prescription/Create
        [HttpGet]
        public IActionResult create()
        {
            // You can initialize the model with one empty medication row if you want:
            var model = new PrescriptionModel();
            model.Medications.Add(new DrugDosage()); 
            return View("~/Views/M3T2/Prescription/Create.cshtml", model);
        }

        // POST: /Prescription/Create
        [HttpPost]
        public async Task<IActionResult> create(PrescriptionModel model)
        {
            if (ModelState.IsValid)
            {
                await _prescriptionControl.addPrescriptionAsync(model);
                return RedirectToAction("index");
            }
            return View("~/Views/M3T2/Prescription/Create.cshtml", model);
        }

        // GET: /Prescription
        [HttpGet]
        public async Task<IActionResult> index()
        {
            string userRole = HttpContext.Session.GetString("UserRole") ?? "Unknown";
            string userUUID = HttpContext.Session.GetString("UserUUID") ?? "Unknown";

            var prescriptions = await _prescriptionControl.getAllPrescriptionsAsync(userRole, userUUID);
            return View("~/Views/M3T2/Prescription/Index.cshtml", prescriptions);
        }
    }
}
