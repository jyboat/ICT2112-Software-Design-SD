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
    public class NotificationPreferenceInputController : Controller
    {
        private readonly NotificationPreferenceManager _manager;

        public NotificationPreferenceInputController(NotificationPreferenceManager manager)
        {
            _manager = manager;
        }

        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View("~/Views/M2T5/NotificationPreferences/Index.cshtml");
        }

        [HttpPost("save")]
        public async Task<IActionResult> CustomizePreferences([FromBody] Dictionary<string, string> preferenceData)
        {
            string userID = HttpContext.Session.GetString("UserID");

            Console.WriteLine($"UserID from session: {userID}");

            if (string.IsNullOrEmpty(userID))
            {
                return BadRequest(new { message = "UserID is missing or invalid." });
            }

            try
            {
                if (preferenceData == null || !preferenceData.ContainsKey("methods") || !preferenceData.ContainsKey("dndDays") || !preferenceData.ContainsKey("dndTimeRange"))
                {
                    return BadRequest(new { message = "Invalid request body. Missing preference, methods, DND days, or DND time range." });
                }

                var methods = preferenceData["methods"]; // Comma-separated methods
                var dndDays = preferenceData["dndDays"]; // Comma-separated days
                var dndTimeRange = preferenceData["dndTimeRange"];

                if (string.IsNullOrEmpty(methods))
                {
                    return BadRequest(new { message = "Please select at least one notification method." });
                }

                // Convert methods string to a list (comma-separated values)
                var methodsList = methods.Split(',').ToList();
                var methodsString = string.Join(",", methodsList); // Convert back to string

                // Declare dndTimeRangeObj as a nullable TimeRange
                TimeRange? dndTimeRangeObj = null;

                // Check if dndTimeRange is empty or null
                if (!string.IsNullOrWhiteSpace(dndTimeRange))
                {
                    // Trim any spaces around the input values
                    var timeRangeParts = dndTimeRange.Split('-').Select(part => part.Trim()).ToArray();

                    // Validate we have exactly two time components
                    if (timeRangeParts.Length != 2)
                    {
                        return BadRequest(new { message = "Invalid time range format. Use HH:mm-HH:mm (e.g., 08:00-22:00)." });
                    }

                    // Parse start and end times using TimeSpan
                    if (!TimeSpan.TryParse(timeRangeParts[0], out TimeSpan startTime) || !TimeSpan.TryParse(timeRangeParts[1], out TimeSpan endTime))
                    {
                        return BadRequest(new { message = "Invalid time format. Use HH:mm (e.g., 08:00-22:00)." });
                    }

                    // Ensure the start time is before the end time
                    if (startTime == endTime)
                    {
                        return BadRequest(new { message = "Invalid time range. Start time cannot be same as end time." });
                    }

                    // Assign the parsed time range to the variable
                    dndTimeRangeObj = new TimeRange(startTime, endTime);
                }
                else
                {
                    // If both times are empty, don't send a dash ("-"), send null or empty string instead
                    dndTimeRangeObj = null;  // or you can use string.Empty or another placeholder if needed
                }

                // Save preferences
                await _manager.UpdateNotificationPreferences(userID, methodsString, dndDays, dndTimeRangeObj);

                return Ok(new { message = "Preference saved successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error saving preference.", error = ex.Message });
            }
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> DisplayPreferences()
        {
            string userId = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "UserID is missing or invalid." });
            }

            try
            {
                var preferences = await _manager.GetNotificationPreferences(userId);
                var response = preferences.Select(p => new 
                {
                    UserID = p.GetUserID(),
                    Methods = p.GetMethods(),
                    DndDays = p.GetDndDays(),
                    DndTimeRange = $"{p.GetDndTimeRange().GetStartTime()}-{p.GetDndTimeRange().GetEndTime()}"
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching preferences.", error = ex.Message });
            }
        }
    }
}