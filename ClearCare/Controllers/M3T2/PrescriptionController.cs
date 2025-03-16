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
        public IActionResult Create()
        {
            // Render a form for creating a Prescription
            return View("~/Views/M3T2/Prescription/Create.cshtml");
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
            return View("~/Views/M3T2/Prescription/Index.cshtml", prescriptions);
        }
    }
}
