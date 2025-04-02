using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ClearCare.Models;

namespace ClearCare.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        ///   Initializes a new instance of the <see cref="HomeController"/>
        ///   class.
        /// </summary>
        /// <param name="logger">
        ///   The logger for this controller to record information and errors.
        /// </param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///   Displays the main index page of the application.
        /// </summary>
        /// <returns>The Index view.</returns>
        public IActionResult index()
        {
            return View();
        }

        /// <summary>
        ///   Displays the privacy policy page.
        /// </summary>
        /// <returns>The Privacy view.</returns>
        public IActionResult privacy()
        {
            return View();
        }

        /// <summary>
        ///   Displays an error page with the request ID.
        /// </summary>
        /// <returns>The Error view.</returns>
        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true
        )]
        public IActionResult error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId =
                        Activity.Current?.Id ?? HttpContext.TraceIdentifier
                }
            );
        }
    }
}
