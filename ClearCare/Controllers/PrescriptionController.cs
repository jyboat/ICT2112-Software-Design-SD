using ClearCare.Controls;
using ClearCare.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClearCare.Controllers
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
            return View(model);
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
            return View(model);
        }

        // GET: /Prescription
        [HttpGet]
        public async Task<IActionResult> index()
        {
            string userRole = HttpContext.Session.GetString("Role") ?? "Unknown";
            string userUUID = HttpContext.Session.GetString("UserID") ?? "Unknown";

            var prescriptions = await _prescriptionControl.getAllPrescriptionsAsync(userRole, userUUID);
            return View(prescriptions);
        }
    }
}
