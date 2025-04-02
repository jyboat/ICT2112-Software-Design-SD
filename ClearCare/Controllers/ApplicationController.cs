using Microsoft.AspNetCore.Mvc;

namespace ClearCare.Controllers
{
    public class ApplicationController : Controller
    {
        /// <summary>
        ///   Main entry point of the application. This action is responsible
        ///   for rendering the initial view when the application starts.
        /// </summary>
        public IActionResult index()
        {
            // Typically returns a "home" or "landing" page to the user.
            return View();
        }

        /// <summary>
        ///   Action to redirect users to the Enquiry page.  It navigates the
        ///   user interface to the section where enquiries can be made or
        ///   managed.
        /// </summary>
        public IActionResult goToEnquiry()
        {
            // Redirects to the EnquiryController's index action, effectively
            // taking the user to the enquiry section.
            return RedirectToAction("index", "Enquiry");
        }

        /// <summary>
        ///   Action to redirect users to the Side Effects page.  This is
        ///   intended to guide users to information or management related to
        ///   side effects.
        /// </summary>
        public IActionResult goToSideEffects()
        {
            // Redirects to the SideEffectsController's index action, taking
            // the user to the side effects section.
            return RedirectToAction("index", "SideEffects");
        }

        /// <summary>
        ///   Action to redirect users to the Prescription page.  This action
        ///   navigates the user to the area where prescriptions are handled.
        /// </summary>
        public IActionResult goToPrescription()
        {
            // Redirects to the PrescriptionController's index action,
            // navigating the user to the prescription management section.
            return RedirectToAction("index", "Prescription");
        }

        /// <summary>
        ///   Action to redirect users to the Patient Drug Log page.  This
        ///   allows users to view or manage logs related to patient drug
        ///   information.
        /// </summary>
        public IActionResult goToDrugLog()
        {
            // Redirects to the PatientDrugLogController's index action,
            // navigating to the patient drug log section.
            return RedirectToAction("index", "PatientDrugLog");
        }

        /// <summary>
        ///   Action to redirect doctors to their specific view of the Drug Log
        ///   page.  This likely provides a different interface or set of
        ///   functionalities compared to the standard patient drug log view.
        /// </summary>
        public IActionResult goToDoctorDrugLog()
        {
            // Redirects to the PatientDrugLogController's DoctorIndex action,
            // providing doctors with a specialized view of the drug log.
            return RedirectToAction("DoctorIndex", "PatientDrugLog");
        }

        /// <summary>
        ///   Action to redirect users to the Drug Interaction page.  This is
        ///   where users can find information or tools related to potential
        ///   drug interactions.
        /// </summary>
        public IActionResult goToDrugInteraction()
        {
            // Redirects to the DrugInteractionController's index action,
            // navigating to the drug interaction section.
            return RedirectToAction("index", "DrugInteraction");
        }
    }
}
