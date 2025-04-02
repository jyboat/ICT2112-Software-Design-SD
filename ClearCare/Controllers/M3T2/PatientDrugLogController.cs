using ClearCare.Controls;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ClearCare.Models.Interfaces.M3T2;
using ClearCare.Models.DTO.M3T2;

namespace ClearCare.Controllers.M3T2
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
            return View("~/Views/M3T2/PatientDrugLog/Index.cshtml", drugLog);
        }

        [HttpGet]
        public IActionResult add()
        {
            return View("~/Views/M3T2/PatientDrugLog/Add.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> DoctorIndex()
        {
            var drugLog = await _patientDrugLogControl.getAllDrugLogAsync();
            return View("~/Views/M3T2/PatientDrugLog/DoctorIndex.cshtml", drugLog);
        }

        //Method Name Matches CD ✅
        [HttpPost]
        public async Task<IActionResult> add(PatientDrugLogDTO drugInfo)
        {
            if (ModelState.IsValid)
            {
                await _patientDrugLogControl.uploadDrugInfo(drugInfo);
                return RedirectToAction("index");
            }

            return View("~/Views/M3T2/PatientDrugLog/Add.cshtml");
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
