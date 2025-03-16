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
        public IActionResult Create()
        {
            // Render a form for creating a Prescription
            return View();
        }

        // POST: /Prescription/Create
        [HttpPost]
        public async Task<IActionResult> Create(PrescriptionModel model)
        {
            if (ModelState.IsValid)
            {
                await _prescriptionControl.AddPrescriptionAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: /Prescription
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var prescriptions = await _prescriptionControl.GetAllPrescriptionsAsync();
            return View(prescriptions);
        }
    }
}
