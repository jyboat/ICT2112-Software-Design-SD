using Microsoft.AspNetCore.Mvc;
using ClearCare.Controls; // or your actual namespace for PrescriptionControl
using ClearCare.Models;   // or your actual namespace for PrescriptionEntity_RDM
using System.Collections.Generic;

namespace ClearCare.Controllers
{
    public class PrescriptionController : Controller
    {
        // GET: /Prescription/Index
        public IActionResult Index()
        {
            // Get all prescriptions from your singleton
            var prescriptions = PrescriptionControl.Instance.GetAllPrescriptions();

            // Pass them to the Index view
            return View(prescriptions);
        }

        // Additional actions like Create, Edit, etc. can go here
    }
}
