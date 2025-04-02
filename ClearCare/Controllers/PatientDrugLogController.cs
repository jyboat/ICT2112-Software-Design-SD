using ClearCare.Controls;
using ClearCare.Models;
using ClearCare.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClearCare.Controllers
{
    public class PatientDrugLogController : Controller
    {
        private readonly PatientDrugLogControl _patientDrugLogControl;
        //IFetchSideEffects interface
        private readonly IFetchSideEffects _ifetchSideEffects;

        /// <summary>
        ///   Initializes a new instance of the
        ///   <see cref="PatientDrugLogController"/> class.
        /// </summary>
        /// <param name="patientDrugLogControl">
        ///   The control for managing patient drug logs.
        /// </param>
        /// <param name="ifetchSideEffects">
        ///   The interface for fetching drug side effects.
        /// </param>
        public PatientDrugLogController(
            PatientDrugLogControl patientDrugLogControl,
            IFetchSideEffects ifetchSideEffects
        )
        {
            _patientDrugLogControl = patientDrugLogControl;
            _ifetchSideEffects = ifetchSideEffects;
        }

        /// <summary>
        ///   Displays the patient's drug log.
        /// </summary>
        /// <returns>The Index view with the patient's drug log data.</returns>
        [HttpGet]
        public async Task<IActionResult> index()
        {
            var drugLog = await _patientDrugLogControl.getDrugLogAsync();
            return View(drugLog);
        }

        /// <summary>
        ///   Displays the form for adding a new drug entry to the patient's log.
        /// </summary>
        /// <returns>The Add view.</returns>
        [HttpGet]
        public IActionResult add()
        {
            return View();
        }

        /// <summary>
        ///   Displays the drug log for all patients (Doctor's view).
        /// </summary>
        /// <returns>The DoctorIndex view with all drug log data.</returns>
        [HttpGet]
        public async Task<IActionResult> DoctorIndex()
        {
            var drugLog = await _patientDrugLogControl.getAllDrugLogAsync();
            return View("DoctorIndex", drugLog);
        }

        /// <summary>
        ///   Adds a new drug entry to the patient's log.
        /// </summary>
        /// <param name="drugInfo">The drug information to add.</param>
        /// <returns>
        ///   If the model state is valid, redirects to the Index view;
        ///   otherwise, returns the Add view with validation errors.
        /// </returns>
        //Method Name Matches CD ✅
        [HttpPost]
        public async Task<IActionResult> add(PatientDrugLogModel drugInfo)
        {
            if (ModelState.IsValid)
            {
                await _patientDrugLogControl.uploadDrugInfo(drugInfo);
                return RedirectToAction("index");
            }

            return View();
        }

        /// <summary>
        ///   Fetches the side effects for a given drug name.
        /// </summary>
        /// <param name="drugName">The name of the drug to fetch side effects
        ///   for.</param>
        /// <returns>
        ///   A JSON content result containing the side effects, or a BadRequest
        ///   result if the drug name is invalid.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> fetchDrugSideEffect(string drugName)
        {
            if (string.IsNullOrWhiteSpace(drugName))
            {
                return BadRequest("Drug name is required.");
            }

            string sideEffects = await _ifetchSideEffects.fetchDrugSideEffect(
                drugName
            );
            return Content(sideEffects, "application/json");
        }
    }
}
