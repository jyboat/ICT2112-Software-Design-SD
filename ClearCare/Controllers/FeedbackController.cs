using ClearCare.DataSource;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.Json;
using ClearCare.Models.Controls;
using ClearCare.Models.Entities;

// Request Handling
[Route("Feedback")]
public class FeedbackController : Controller
{
    private readonly FeedbackManager _manager;

    public FeedbackController()
    {
        _manager = new FeedbackManager();
    }

    [Route("")]
    [HttpGet]
    public async Task<IActionResult> List()
    {
        List<Dictionary<string, object>> feedbackList = new List<Dictionary<string, object>>();
        ViewBag.UserRole = "Doctor"; // Hardcoded for testing

        if (ViewBag.UserRole == "Doctor") {
            
            feedbackList = (await _manager.viewFeedback())
                .Select(s => s.GetDetails())
                .ToList();
        }
        else if (ViewBag.UserRole == "Patient")
        {
            String patientId = "1"; // Hardcoded for testing
            feedbackList = (await _manager.viewFeedbackByPatientId(patientId))
                .Select(s => s.GetDetails())
                .ToList();
        }
        else
        {
            return RedirectToAction("List");
        }
        return View("List", feedbackList);
    }

    [Route("Submission")]
    [HttpGet]
    public IActionResult Submit()
    {
        return View("Submission");
    }

    [Route("Submission")]
    [HttpPost]
    public async Task<IActionResult> Submit(string content, int rating)
    {
        if (string.IsNullOrEmpty(content) || rating < 1 || rating > 5)
        {
            ViewBag.ErrorMessage = "Please fill in all required fields";
        }

        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        // Process the feedback here
        string id = await _manager.submitFeedback(content, rating, "1", currentDate);

        return RedirectToAction("List");
    }

    [Route("Response/{feedbackId}")]
    [HttpGet]
    public async Task<IActionResult> Respond(string feedbackId)
    {
        Feedback feedback = await _manager.getFeedback(feedbackId);

        if (feedback == null)
        {
            ViewBag.ErrorMessage = "Feedback not found.";
            return View("Response");
        }

        ViewBag.FeedbackId = feedbackId;
        ViewBag.FeedbackContent = feedback.GetDetails()["Content"];
        ViewBag.FeedbackRating = feedback.GetDetails()["Rating"];

        return View("Response");
    }

    [Route("Response/{feedbackId}")]
    [HttpPost]
    public async Task<IActionResult> Respond(string feedbackId, string response)
    {
        if (string.IsNullOrEmpty(response))
        {
            ViewBag.ErrorMessage = "Please fill in all required fields";
        }

        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        // Process the response here
        bool success = await _manager.respondToFeedback(feedbackId, response, "1", currentDate);

        if (!success)
        {
            ViewBag.ErrorMessage = "Failed to respond to feedback. Please try again.";
        }

        return RedirectToAction("List");
    }
}
