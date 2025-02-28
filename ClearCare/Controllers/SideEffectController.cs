using Microsoft.AspNetCore.Mvc;
using ClearCare.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Gateways;  // Make sure to import your Gateway namespace

namespace ClearCare.Controllers
{
    public class SideEffectController : Controller
    {
        private readonly ILogger<SideEffectController> _logger;
    private readonly EnquiryGateway _enquiryGateway;

        public SideEffectController(ILogger<SideEffectController> logger)
        {
            _logger = logger;
        _enquiryGateway = new EnquiryGateway();
        }

        // Fetch and display side effects from Firebase
        public async Task<IActionResult> Index()
        {
            // Fetch side effects from Firebase
            List<SideEffectModel> sideEffects = await _enquiryGateway.GetSideEffectsAsync();

            // Pass the data to the view
            return View(sideEffects);
        }

        // Privacy page (if needed)
        public IActionResult Privacy()
        {
            return View();
        }

        // List enquiries (if needed)
        public IActionResult ListEnquiries()
        {
            return View();
        }
    }
}
