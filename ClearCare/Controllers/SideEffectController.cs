using Microsoft.AspNetCore.Mvc;

namespace ClearCare.Controllers
{
    public class SideEffectController : Controller
    {
        // Action to render the Index view
        public IActionResult Index()
        {
            // Hardcoded data for now (replace with dynamic data later)
            var sideEffects = new List<dynamic>
            {
                new { Name = "John Doe", Description = "Headache and nausea", Date = "2023-10-01" },
                new { Name = "Jane Smith", Description = "Dizziness and fatigue", Date = "2023-10-03" },
                new { Name = "Alice Johnson", Description = "Skin rash and itching", Date = "2023-10-05" }
            };

            // Pass the data to the view
            ViewData["SideEffects"] = sideEffects;

            return View();
        }
    }
}
