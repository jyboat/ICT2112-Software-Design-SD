using ClearCare.DataSource.M3T1;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.Json;
using ClearCare.Models.Entities;
using static Google.Rpc.Context.AttributeContext.Types;
using ClearCare.Models.Control.M3T1;
using ClearCare.Models.Entities.M3T1;
using System.Collections.Generic;

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
        feedbackMapper.feedbackReceiver = _feedbackManager;
        _responseManager = new ResponseManager(feedbackMapper);
        feedbackMapper.responseReceiver = _responseManager;

        // Attach observer
        var patientObserver = new PatientNotificationObserver();
        feedbackMapper.attach(patientObserver);
    }

    [Route("")]
    [HttpGet]
    public async Task<IActionResult> displayAllFeedback(
        int page = 1,
        int pageSize = 10,
        string search = "",
        string responseFilter = "All",
        int ratingFilter = 0)

    {
        List<Dictionary<string, object>> feedbackList = new List<Dictionary<string, object>>();
        List<Dictionary<string, object>> responseList = new List<Dictionary<string, object>>();

        string userRole = HttpContext.Session.GetString("Role") ?? "Unknown";

        if (userRole == "Doctor" || userRole == "Nurse") {
            
            feedbackList = (await _feedbackManager.viewFeedback())
                .Select(s => s.getFeedbackDetails())
                .ToList();

            responseList = (await _responseManager.viewResponse())
                .Select(r => r.getResponseDetails())
                .ToList();
        }
        else if (userRole == "Patient" || userRole == "Caregiver")
        {
            string patientId = HttpContext.Session.GetString("UserID") ?? "";
            if (string.IsNullOrEmpty(patientId))
            {
                TempData["ErrorMessage"] = "Please log in to access";
                return View("~/Views/Home/Index.cshtml");
            }

            feedbackList = (await _feedbackManager.viewFeedbackByUserId(patientId))
                .Select(s => s.getFeedbackDetails())
                .ToList();

            // Fetch responses only for feedbacks belonging to this patient/caregiver
            responseList = await _responseManager.getResponsesForFeedbackList(feedbackList);

            // Check if patient has new notifications
            if (_feedbackManager.responseNotification(patientId))
            {
                TempData["SuccessMessage"] = "One or more of your feedbacks received a response.";
            }
        }
        else
        {
            TempData["ErrorMessage"] = "Unauthorized access";
            return View("~/Views/Home/Index.cshtml");
        }

        // Combine feedbackList and responseList
        List<Dictionary<string, object>> combinedList = _feedbackManager.combineFeedbackResponse(feedbackList, responseList);

        combinedList = _feedbackManager.applySearchFilter(combinedList, search);
        combinedList = _feedbackManager.applyResponseFilter(combinedList, responseFilter);
        combinedList = _feedbackManager.applyRatingFilter(combinedList, ratingFilter);

        // Pass state to ViewBag
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = (combinedList.Count + pageSize - 1)/pageSize;
        ViewBag.TotalItems = combinedList.Count;

        combinedList = _feedbackManager.applyPagination(combinedList, page, pageSize);

        return View("~/Views/M3T1/Feedback/List.cshtml", combinedList);
    }

    [Route("Submission")]
    [HttpGet]
    public IActionResult displayAddForm()
    {
        string userRole = HttpContext.Session.GetString("Role") ?? "Unknown";
        if (userRole != "Patient")
        {
            TempData["ErrorMessage"] = "Unauthorized access";
            return View("~/Views/Home/Index.cshtml");
        }

        return View("~/Views/M3T1/Feedback/Submission.cshtml");
    }

    [Route("Submission")]
    [HttpPost]
    public async Task<IActionResult> postAddFeedback(string content, int rating)
    {
        string userId = HttpContext.Session.GetString("UserID") ?? "";

        if (string.IsNullOrEmpty(userId))
        {
            TempData["ErrorMessage"] = "Please log in to access";
            return View("~/Views/Home/Index.cshtml");
        }

        if (string.IsNullOrEmpty(content) || rating < 1 || rating > 5)
        {
            TempData["ErrorMessage"] = "Please fill in all required fields";
        }

        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        // Process the feedback here
        string id = await _feedbackManager.submitFeedback(content, rating, userId, currentDate);

        TempData["SuccessMessage"] = "Feedback added successfully!";

        return RedirectToAction("DisplayAllFeedback");
    }

    [Route("Response/{feedbackId}")]
    [HttpPost]
    public async Task<IActionResult> postRespondFeedback(string feedbackId, string response)
    {
        string userRole = HttpContext.Session.GetString("Role") ?? "Unknown";

        if (userRole != "Doctor" || userRole != "Nurse")
        {
            TempData["ErrorMessage"] = "Unauthorized access";
            return View("~/Views/Home/Index.cshtml");
        }

        string userId = HttpContext.Session.GetString("UserID") ?? "";

        if (string.IsNullOrEmpty(userId))
        {
            TempData["ErrorMessage"] = "Please log in to access";
            return View("~/Views/Home/Index.cshtml");
        }


        if (string.IsNullOrEmpty(response))
        {
            TempData["ErrorMessage"] = "Please fill in all required fields";
        }

        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

        // Process the response here
        string id = await _responseManager.respondToFeedback(feedbackId, response, userId, currentDate);

        TempData["SuccessMessage"] = "Response added successfully!";

        return RedirectToAction("DisplayAllFeedback");
    }

    [Route("ResponseEdit/{responseId}")]
    [HttpPost]
    public async Task<IActionResult> updateResponse(string responseId, string response)
    {
        string userRole = HttpContext.Session.GetString("Role") ?? "Unknown";

        if (userRole != "Doctor" || userRole != "Nurse")
        {
            TempData["ErrorMessage"] = "Unauthorized access";
            return View("~/Views/Home/Index.cshtml");
        }

        if (string.IsNullOrEmpty(response))
        {
            TempData["ErrorMessage"] = "Please fill in all required fields";
        }

        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        // Process the response here
        bool success = await _responseManager.updateResponse(responseId, response, currentDate);

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
    public async Task<IActionResult> deleteResponse(string responseId)
    {
        string userRole = HttpContext.Session.GetString("Role") ?? "Unknown";

        if (userRole != "Doctor" || userRole != "Nurse")
        {
            TempData["ErrorMessage"] = "Unauthorized access";
            return View("~/Views/Home/Index.cshtml");
        }

        await _responseManager.deleteResponse(responseId);

        TempData["SuccessMessage"] = "Response deleted successfully!";

        return RedirectToAction("DisplayAllFeedback");
    }
}
