using ClearCare.Controls;
using ClearCare.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClearCare.Controllers
{
    public class PatientDrugLogController : Controller
    {
        private readonly PatientDrugLogControl _patientDrugLogControl;

        public PatientDrugLogController(PatientDrugLogControl patientDrugLogControl)
        {
            _patientDrugLogControl = patientDrugLogControl;
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
    }
}
