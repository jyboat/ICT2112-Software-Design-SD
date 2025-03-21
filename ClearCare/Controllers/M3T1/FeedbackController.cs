using ClearCare.DataSource.M3T1;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.Json;
using ClearCare.Models.Entities;
using static Google.Rpc.Context.AttributeContext.Types;
using ClearCare.Models.Control.M3T1;

// Request Handling
[Route("Feedback")]
public class FeedbackController : Controller
{
    private readonly FeedbackManager _feedbackManager;
    private readonly ResponseManager _responseManager;

    public FeedbackController()
    {
        var feedbackMapper = new FeedbackMapper();
        _feedbackManager = new FeedbackManager(feedbackMapper);
        _responseManager = new ResponseManager(feedbackMapper);
        feedbackMapper.feedbackReceiver = _feedbackManager;
        feedbackMapper.responseReceiver = _responseManager;
    }

    [Route("")]
    [HttpGet]
    public async Task<IActionResult> DisplayAllFeedback()
    {
        List<Dictionary<string, object>> feedbackList = new List<Dictionary<string, object>>();
        List<Dictionary<string, object>> responseList = new List<Dictionary<string, object>>();

        ViewBag.UserRole = "Doctor"; // Hardcoded for testing

        if (ViewBag.UserRole == "Doctor" || ViewBag.UserRole == "Nurse") {
            
            feedbackList = (await _feedbackManager.viewFeedback())
                .Select(s => s.GetFeedbackDetails())
                .ToList();

            responseList = (await _responseManager.viewResponse())
                .Select(r => r.GetResponseDetails())
                .ToList();
        }
        else if (ViewBag.UserRole == "Patient")
        {
            String patientId = "1"; // Hardcoded for testing
            feedbackList = (await _feedbackManager.viewFeedbackByUserId(patientId))
                .Select(s => s.GetFeedbackDetails())
                .ToList();

            // Fetch responses only for feedbacks belonging to this patient
            foreach (var feedback in feedbackList)
            {
                string? feedbackId = feedback["Id"]?.ToString();
                if (!string.IsNullOrEmpty(feedbackId))
                {
                    var response = await _responseManager.viewResponseByFeedbackId(feedbackId);
                    if (response != null)
                    {
                        responseList.Add(response.GetResponseDetails());
                    }
                }
            }
        }
        else
        {
            return RedirectToAction("DisplayAllFeedback");
        }

        // Create lookup dictionary for responses by FeedbackId
        var responseMap = responseList
            .Where(r => r.ContainsKey("FeedbackId"))
            .ToDictionary(r => r["FeedbackId"]?.ToString() ?? "", r => r);

        // Combine feedbackList and responseList
        List<Dictionary<string, object>> combinedList = feedbackList.Select(fb =>
        {
            string feedbackId = fb["Id"]?.ToString() ?? "";
            responseMap.TryGetValue(feedbackId, out var response);

            fb["Response"] = response?["Response"] ?? "";
            fb["DateResponded"] = response?["DateResponded"] ?? "";
            fb["ResponseUserId"] = response?["UserId"] ?? "";
            return fb;
        }).ToList();

        return View("~/Views/M3T1/Feedback/List.cshtml", combinedList);
    }

    [Route("Submission")]
    [HttpGet]
    public IActionResult DisplayAddForm()
    {
        return View("~/Views/M3T1/Feedback/Submission.cshtml");
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
        string id = await _feedbackManager.submitFeedback(content, rating, "1", currentDate);

        TempData["SuccessMessage"] = "Feedback added successfully!";

        return RedirectToAction("DisplayAllFeedback");
    }

    [Route("Delete/{feedbackId}")]
    [HttpPost]
    public async Task<IActionResult> DeleteFeedback(string feedbackId)
    {
        await _feedbackManager.deleteFeedback(feedbackId);

        TempData["SuccessMessage"] = "Feedback deleted successfully!";

        return RedirectToAction("DisplayAllFeedback");
    }

    [Route("Response/{feedbackId}")]
    [HttpGet]
    public async Task<IActionResult> DisplayResponseForm(string feedbackId)
    {
        var feedback = await _feedbackManager.getFeedback(feedbackId);

        if (feedback == null)
        {
            TempData["ErrorMessage"] = "Feedback not found.";
            return RedirectToAction("DisplayAllFeedback");
        }

        return View("~/Views/M3T1/Feedback/Response.cshtml", feedback);
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
        string id = await _responseManager.respondToFeedback(feedbackId, response, "1", currentDate);

        TempData["SuccessMessage"] = "Response added successfully!";

        return RedirectToAction("DisplayAllFeedback");
    }

    [Route("ResponseEdit/{responseId}")]
    [HttpGet]
    public async Task<IActionResult> ViewResponseEdit(string responseId)
    {
        var response = await _responseManager.getResponse(responseId);
        if (response == null)
        {
            return RedirectToAction("DisplayAllFeedback");
        }
        return View("~/Views/M3T1/Feedback/ResponseEdit.cshtml", response);
    }

    [Route("ResponseEdit/{responseId}")]
    [HttpPost]
    public async Task<IActionResult> UpdateResponse(string responseId, string response)
    {
        if (string.IsNullOrEmpty(response))
        {
            TempData["ErrorMessage"] = "Please fill in all required fields";
        }

        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        // Process the response here
        bool success = await _responseManager.updateResponse(responseId, response, "1", currentDate);

        if (!success)
        {
            TempData["ErrorMessage"] = "Failed to update response. Please try again.";
        }
        else
        {
            TempData["SuccessMessage"] = "Response updated successfully!";
        }

        return RedirectToAction("DisplayAllFeedback");
    }

    [Route("ResponseDelete/{responseId}")]
    [HttpPost]
    public async Task<IActionResult> DeleteResponse(string responseId)
    {
        await _responseManager.deleteResponse(responseId);

        TempData["SuccessMessage"] = "Response deleted successfully!";

        return RedirectToAction("DisplayAllFeedback");
    }
}
