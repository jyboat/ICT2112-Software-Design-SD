using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ClearCare.Models.Control;

namespace ClearCare.Controllers
{
    public class NotificationTestController : Controller
    {
        // You can instantiate NotificationManager directly here.
        private readonly NotificationManager _notificationManager = new NotificationManager();

        // GET: /NotificationTest/
        public IActionResult Index()
        {
            return View("~/Views/M2T5/NotificationTest/Index.cshtml");
        }

        // POST: /NotificationTest/Create
        [HttpPost]
        public async Task<IActionResult> CreateNotification()
        {
            // Hardcoded parameters for testing:
            int testUserId = 1; // Example userId
            string testContent = "This is a test notification hehehehe.";

            // Call the NotificationManager's createNotification method.
            await _notificationManager.createNotification(testUserId, testContent);

            // Optionally, set a TempData message to show success on the page.
            TempData["Message"] = "Test notification created and sent!";
            return RedirectToAction("Index");
        }
    }
}
