using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Control;

namespace ClearCare.Controllers
{
    [Route("FileErratum")]
    public class FileErratumController : Controller
    {
        private ErratumManagement ErratumManagement;

        public FileErratumController()
        {
            ErratumManagement = new ErratumManagement();
        }

        [Route("{recordID}")]
        public IActionResult DisplayUpdateRecord()
        {
            var userRole = HttpContext.Session.GetString("Role");

            if (userRole != "Doctor") // Restrict access to doctors only
            {
                Console.WriteLine("You do not have permission to access this page.");
                return RedirectToAction("DisplayViewRecord", "ViewRecord");
            }

            return View("UpdateRecord");
        }
    }
}