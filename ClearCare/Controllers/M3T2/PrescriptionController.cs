using ClearCare.Models.Control.M3T2;
using ClearCare.Models.Entities.M3T2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClearCare.Controllers.M3T2
{
    public class PrescriptionController : Controller
    {
        private readonly PrescriptionControl _prescriptionControl;

        /// <summary>
        ///   Initializes a new instance of the
        ///   <see cref="PrescriptionController"/> class.
        /// </summary>
        /// <param name="prescriptionControl">
        ///   The control responsible for handling prescription-related
        ///   operations.
        /// </param>
        public PrescriptionController(PrescriptionControl prescriptionControl)
        {
            _prescriptionControl = prescriptionControl;
        }

        /// <summary>
        ///   Displays the form for creating a new prescription.
        /// </summary>
        /// <returns>The Create view with an initialized PrescriptionModel.</returns>
        // GET: /Prescription/Create
        [HttpGet]
        public IActionResult create()
        {
            // You can initialize the model with one empty medication row if
            // you want:
            var model = new PrescriptionModel();
            model.Medications.Add(new DrugDosage()); 
            return View("~/Views/M3T2/Prescription/Create.cshtml", model);
        }

        /// <summary>
        ///   Handles the submission of the new prescription form.
        /// </summary>
        /// <param name="model">The PrescriptionModel containing the
        ///   prescription data.</param>
        /// <returns>
        ///   If the model is valid, adds the prescription and redirects to the
        ///   Index action; otherwise, returns the Create view with validation
        ///   errors.
        /// </returns>
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

        /// <summary>
        ///   Displays a list of prescriptions.
        /// </summary>
        /// <returns>
        ///   The Index view with a list of prescriptions based on the user's
        ///   role.
        /// </returns>
        // GET: /Prescription
        [HttpGet]
        public async Task<IActionResult> index()
        {
            string userRole = HttpContext.Session.GetString("Role") ?? "Unknown";
            string userUUID = HttpContext.Session.GetString("UserID") ?? "Unknown";

            var prescriptions = await _prescriptionControl.getAllPrescriptionsAsync(userRole, userUUID);
            return View("~/Views/M3T2/Prescription/Index.cshtml", prescriptions);
        }
    }
}
