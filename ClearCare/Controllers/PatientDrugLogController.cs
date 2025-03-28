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

        public PatientDrugLogController(PatientDrugLogControl patientDrugLogControl, IFetchSideEffects ifetchSideEffects)
        {
            _patientDrugLogControl = patientDrugLogControl;
            _ifetchSideEffects = ifetchSideEffects;
        }

        [HttpGet]
        public async Task<IActionResult> index()
        {
            var drugLog = await _patientDrugLogControl.getDrugLogAsync();
            return View(drugLog);
        }

        [HttpGet]
        public IActionResult add()
        {
            return View();
        }

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
        
        
        [HttpGet]
        public async Task<IActionResult> fetchDrugSideEffect(string drugName)
        {
            if (string.IsNullOrWhiteSpace(drugName))
            {
                return BadRequest("Drug name is required.");
            }

            string sideEffects = await _ifetchSideEffects.fetchDrugSideEffect(drugName);
            return Content(sideEffects, "application/json");
        }
    }
}
