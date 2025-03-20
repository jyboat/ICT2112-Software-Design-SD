using ClearCare.DataSource;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ClearCare.Interfaces;

namespace ClearCare.Controllers
{
    [Route("NotificationPreference")] // Custom route for your view
    [ApiController]
    public class NotificationPreferenceController : Controller
    {

        private readonly NotificationPreferenceManager _manager;

        public NotificationPreferenceController(NotificationPreferenceManager manager)
        {
            _manager = manager;
        }
        public IActionResult Index()
        {

            return View("~/Views/M2T5/NotificationPreferences/Index.cshtml");
        }

[HttpPost("save")]
public async Task<IActionResult> SaveNotificationPreference([FromBody] Dictionary<string, List<string>> preferenceData)
{
    string userID = HttpContext.Session.GetString("UserID");

    Console.WriteLine($"UserID from session: {userID}");

    if (string.IsNullOrEmpty(userID))
    {
        return BadRequest(new { message = "UserID is missing or invalid." });
    }

    try
    {
        // Extract preference and methods from the dictionary
        if (preferenceData == null || !preferenceData.ContainsKey("preference") || !preferenceData.ContainsKey("methods"))
        {
            return BadRequest(new { message = "Invalid request body. Missing preference or methods." });
        }

        var preference = preferenceData["preference"]?.FirstOrDefault(); // Gets the first preference value
        var methods = preferenceData["methods"]; // List of methods (sms, email)

        if (string.IsNullOrEmpty(preference) || methods == null || methods.Count == 0)
        {
            return BadRequest(new { message = "Invalid preference or methods." });
        }

        await _manager.SaveNotificationPreferenceAsync(userID, preference, methods);

        return Ok(new { message = "Preference saved successfully!" });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "Error saving preference.", error = ex.Message });
    }
}
    }
}
