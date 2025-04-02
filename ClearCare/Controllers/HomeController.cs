using System.Diagnostics;
using ClearCare.Models.DTO.M3T2;
using Microsoft.AspNetCore.Mvc;

namespace ClearCare.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult index()
        {
            return View();
        }

        public IActionResult privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult error()
        {
            return View(new ErrorDTO { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
