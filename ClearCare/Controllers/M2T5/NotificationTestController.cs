using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ClearCare.Models.Control;

namespace ClearCare.Controllers
{
    public class NotificationTestController : Controller
    {
        private readonly NotificationManager _notificationManager;

        // Constructor injection
        public NotificationTestController(NotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }

        // GET: /NotificationTest/
        public IActionResult Index()
        {
            return View("~/Views/M2T5/NotificationTest/Index.cshtml");
        }

        // POST: /NotificationTest/Create
        [HttpPost]
        public async Task<IActionResult> CreateNotification()
        {
            string userId = HttpContext.Session.GetString("UserID");
            // Hardcoded parameters for testing:
            string testUserId = "USR003"; // Example userId
            string testContent = "Test";

            // Call the NotificationManager's createNotification method.
            await _notificationManager.createNotification(testUserId, testContent);

            // Optionally, set a TempData message to show success on the page.
            TempData["Message"] = "Test notification created and sent!";
            return RedirectToAction("Index");
        }
    }
}
