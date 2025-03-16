using Microsoft.AspNetCore.Mvc;

namespace ClearCare.Controllers
{
    public class ApplicationController : Controller
    {
        /// <summary>
        /// Main entry point of the application.
        /// </summary>
        public IActionResult Index()
        {
            // Typically returns a "home" or "landing" page
            return View();
        }

        /// <summary>
        /// A simple action to redirect users to the Enquiry page.
        /// </summary>
        public IActionResult GoToEnquiry()
        {
            // Redirects to the EnquiryController's Index action
            return RedirectToAction("Index", "Enquiry");
        }


          public IActionResult GoToSideEffects()
        {
            // Redirects to the EnquiryController's Index action
            return RedirectToAction("Index", "SideEffects");
        }

          public IActionResult GoToPrescription()
        {
            // Redirects to the EnquiryController's Index action
            return RedirectToAction("Index", "Prescription");
        }
    }
}
