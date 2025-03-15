using ClearCare.DataSource;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.Json;
using ClearCare.Models.Controls;
using ClearCare.Models.Entities;
using static Google.Rpc.Context.AttributeContext.Types;

// Request Handling
[Route("Feedback")]
public class FeedbackController : Controller
{
    private readonly FeedbackManager _manager;

    public FeedbackController()
    {
        var gateway = new FeedbackGateway();
        _manager = new FeedbackManager(gateway);
        gateway.receiver = _manager;
    }

    [Route("")]
    [HttpGet]
    public async Task<IActionResult> DisplayAllFeedback()
    {
        List<Dictionary<string, object>> feedbackList = new List<Dictionary<string, object>>();
        ViewBag.UserRole = "Doctor"; // Hardcoded for testing

        if (ViewBag.UserRole == "Doctor") {
            
            feedbackList = (await _manager.viewFeedback())
                .Select(s => s.GetFeedbackDetails())
                .ToList();
        }
        else if (ViewBag.UserRole == "Patient")
        {
            String patientId = "1"; // Hardcoded for testing
            feedbackList = (await _manager.viewFeedbackByPatientId(patientId))
                .Select(s => s.GetFeedbackDetails())
                .ToList();
        }
        else
        {
            return RedirectToAction("DisplayAllFeedback");
        }
        return View("List", feedbackList);
    }

    [Route("Submission")]
    [HttpGet]
    public IActionResult DisplayAddForm()
    {
        return View("Submission");
    }

    [Route("Submission")]
    [HttpPost]
    public async Task<IActionResult> PostAddFeedback(string content, int rating)
    {
        if (string.IsNullOrEmpty(content) || rating < 1 || rating > 5)
        {
            TempData["ErrorMessage"] = "Please fill in all required fields";
        }

        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        // Process the feedback here
        string id = await _manager.submitFeedback(content, rating, "1", currentDate);

        TempData["SuccessMessage"] = "Feedback added successfully!";

        return RedirectToAction("DisplayAllFeedback");
    }

    [Route("Response/{feedbackId}")]
    [HttpGet]
    public async Task<IActionResult> DisplayResponseForm(string feedbackId)
    {
        var feedback = await _manager.getFeedback(feedbackId);

        if (feedback == null)
        {
            TempData["ErrorMessage"] = "Feedback not found.";
            return RedirectToAction("DisplayAllFeedback");
        }

        return View("Response", feedback);
    }

    [Route("Response/{feedbackId}")]
    [HttpPost]
    public async Task<IActionResult> PostRespondFeedback(string feedbackId, string response)
    {
        if (string.IsNullOrEmpty(response))
        {
            TempData["ErrorMessage"] = "Please fill in all required fields";
        }

        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        // Process the response here
        bool success = await _manager.respondToFeedback(feedbackId, response, "1", currentDate);

        if (!success)
        {
            TempData["ErrorMessage"] = "Failed to respond to feedback. Please try again.";
        }
        else
        {
            TempData["SuccessMessage"] = "Response added successfully!";
        }

        return RedirectToAction("DisplayAllFeedback");
    }

    [Route("View/{feedbackId}")]
    [HttpGet]
    public async Task<IActionResult> ViewFeedback(string feedbackId)
    {
        var feedback = await _manager.getFeedback(feedbackId);
        if (feedback == null)
        {
            return RedirectToAction("DisplayAllFeedback");
        }
        return View("Index", feedback);
    }

    [Route("Edit/{feedbackId}")]
    [HttpGet]
    public async Task<IActionResult> ViewEdit(string feedbackId)
    {
        var feedback = await _manager.getFeedback(feedbackId);
        if (feedback == null)
        {
            return RedirectToAction("DisplayAllFeedback");
        }
        return View("Edit", feedback);
    }

    [Route("Edit/{feedbackId}")]
    [HttpPost]
    public async Task<IActionResult> UpdateFeedback(string feedbackId, string content, int rating)
    {
        if (string.IsNullOrEmpty(content) || rating < 1 || rating > 5)
        {
            TempData["ErrorMessage"] = "Please fill in all required fields";
        }

        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        // Process the feedback here
        bool success = await _manager.updateFeedback(feedbackId, content, rating, currentDate);

        if (!success)
        {
            TempData["ErrorMessage"] = "Failed to respond to feedback. Please try again.";
        }
        else
        {
            TempData["SuccessMessage"] = "Feedback updated successfully!";
        }

        return RedirectToAction("DisplayAllFeedback");
    }

    [Route("Delete/{feedbackId}")]
    [HttpPost]
    public async Task<IActionResult> DeleteFeedback(string feedbackId)
    {
        await _manager.deleteFeedback(feedbackId);

        TempData["SuccessMessage"] = "Feedback deleted successfully!";

        return RedirectToAction("DisplayAllFeedback");
    }
}
