using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ClearCare.Controllers
{
    [Route("NotificationPreference")]
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
public async Task<IActionResult> SaveNotificationPreference([FromBody] Dictionary<string, string> preferenceData)
{
    string userID = HttpContext.Session.GetString("UserID");

    Console.WriteLine($"UserID from session: {userID}");

    if (string.IsNullOrEmpty(userID))
    {
        return BadRequest(new { message = "UserID is missing or invalid." });
    }

    try
    {
        if (preferenceData == null || !preferenceData.ContainsKey("preference") || !preferenceData.ContainsKey("methods") || !preferenceData.ContainsKey("dndDays") || !preferenceData.ContainsKey("dndTimeRange"))
        {
            return BadRequest(new { message = "Invalid request body. Missing preference, methods, DND days or DND time range." });
        }

        var preference = preferenceData["preference"];
        var methods = preferenceData["methods"]; // This is a comma-separated string
        var dndDays = preferenceData["dndDays"]; // This is also a comma-separated string
        var dndTimeRange = preferenceData["dndTimeRange"];

        if (string.IsNullOrEmpty(preference) || string.IsNullOrEmpty(methods) || string.IsNullOrEmpty(dndDays))
        {
            return BadRequest(new { message = "Invalid preference, methods, or DND days." });
        }

        // Convert methods string to a list (comma-separated values)
        var methodsList = methods.Split(',').ToList();

        // DND Days - already a comma-separated string, so just use it as it is
        var dndDaysString = dndDays;

        // Parse DND Time Range
        var timeRangeParts = dndTimeRange?.Split('-');
        var startTime = TimeSpan.Parse(timeRangeParts[0]);
        var endTime = TimeSpan.Parse(timeRangeParts[1]);
        var dndTimeRangeObj = new TimeRange(startTime, endTime);

        // Convert methodsList to a comma-separated string for passing to SaveNotificationPreferenceAsync
        var methodsString = string.Join(",", methodsList);

        // Save preferences using the manager (methodsString is string, dndDaysString is string)
        await _manager.SaveNotificationPreferenceAsync(userID, preference, methodsString, dndDaysString, dndTimeRangeObj);

        return Ok(new { message = "Preference saved successfully!" });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "Error saving preference.", error = ex.Message });
    }
}





 [HttpGet("get-all")]
public async Task<IActionResult> GetUserNotificationPreference()
{
    string userId = HttpContext.Session.GetString("UserID");

    if (string.IsNullOrEmpty(userId))
    {
        return BadRequest(new { message = "UserID is missing or invalid." });
    }

    try
    {
        // Call manager to get notification preferences for the user
        var preferences = await _manager.GetNotificationPreferenceAsync(userId);

        // Return the preferences as JSON
        return Ok(preferences);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "Error fetching preferences.", error = ex.Message });
    }
}
    }
}
