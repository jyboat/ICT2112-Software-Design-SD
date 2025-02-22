using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Entities;
using ClearCare.Models.Control;
using System.Threading.Tasks;

namespace ClearCare.Controllers
{
    [Route("ViewRecord")]
    public class ViewRecordController : Controller
    {
        [Route("")]
        public IActionResult DisplayViewRecord()
        {
            return View("ViewRecord");
        }

    }
}