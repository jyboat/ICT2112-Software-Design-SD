using Microsoft.AspNetCore.Mvc;

namespace ClearCare.Controllers
{
    public class ApplicationController : Controller
    {
        /// <summary>
        /// Main entry point of the application.
        /// </summary>
        public IActionResult index()
        {
            // Typically returns a "home" or "landing" page
            return View();
        }

        /// <summary>
        /// A simple action to redirect users to the Enquiry page.
        /// </summary>
        public IActionResult goToEnquiry()
        {
            // Redirects to the EnquiryController's index action
            return RedirectToAction("index", "Enquiry");
        }

        public IActionResult goToSideEffects()
        {
            // Redirects to the SideEffectsController's index action
            return RedirectToAction("index", "SideEffects");
        }


         public IActionResult goToPrescription()
        {
            // Redirects to the SideEffectsController's index action
            return RedirectToAction("index", "Prescription");
        }
    }
}
