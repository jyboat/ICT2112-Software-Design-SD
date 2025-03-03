using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ClearCare.Models;
using ClearCare.Gateways;

namespace ClearCare.Controllers;

public class EnquiryController : Controller
{
    private readonly ILogger<EnquiryController> _logger;
    // Remove the static list
    // public static List<Enquiry> Enquiries = new List<Enquiry>();
    
    // Change to interface type
    private readonly IEnquiryGateway _enquiryGateway;

    // Update constructor to accept IEnquiryGateway
    public EnquiryController(ILogger<EnquiryController> logger, IEnquiryGateway enquiryGateway)
    {
        _logger = logger;
        _enquiryGateway = enquiryGateway;
    }

    // Update to use the gateway and make it async
    public async Task<IActionResult> Index()
    {
        var allEnquiries = await _enquiryGateway.GetAllEnquiriesAsync();
        return View(allEnquiries);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // Update to use the gateway and make it async
    public async Task<IActionResult> ListEnquiries()
    {
        var enquiries = await _enquiryGateway.GetAllEnquiriesAsync();
        return View(enquiries);
    }

    [HttpGet]
    public async Task<IActionResult> ListEnquiriesByUser(string userUUID)
    {
        var userEnquiries = await _enquiryGateway.GetEnquiriesForUserAsync(userUUID);
        return View("ListEnquiries", userEnquiries);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitEnquiry(Enquiry enquiry)
    {
        _logger.LogInformation($"Received enquiry from {enquiry.Name} with email {enquiry.Email}: {enquiry.Message}");

        enquiry.Id = Guid.NewGuid().ToString();
        // Remove adding to static list
        // Enquiries.Add(enquiry);

        ViewData["Name"] = enquiry.Name;
        ViewData["Email"] = enquiry.Email;
        ViewData["Message"] = enquiry.Message;
        ViewData["UserUUID"] = HardcodedUUIDs.UserUUID;

        await _enquiryGateway.SaveEnquiryAsync(enquiry);

        return View("EnquiryResult");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }

    public async Task<IActionResult> Reply(string id)
    {
        try
        {
            var enquiry = await _enquiryGateway.GetEnquiryByIdAsync(id);

            if (enquiry == null)
            {
                return NotFound($"Enquiry with ID {id} not found.");
            }

            // This line is redundant since GetEnquiryByIdAsync already sets FirestoreId
            // enquiry.FirestoreId = id;

            var replies = await _enquiryGateway.GetRepliesForEnquiryAsync(id);

            var viewModel = new ReplyToEnquiryViewModel
            {
                Enquiry = enquiry,
                Replies = replies
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving enquiry with ID {id}");
            return View("Error", new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendReply(string enquiryId, string recipientName,
        string recipientEmail, string originalMessage, string userUUID,
        string subject, string message)
    {
        try
        {
            var reply = new Reply
            {
                EnquiryId = enquiryId,
                Subject = subject,
                Message = message,
                RecipientName = recipientName,
                RecipientEmail = recipientEmail,
                OriginalMessage = originalMessage,
                UserUUID = userUUID,
                CreatedAt = DateTime.UtcNow
            };

            string replyId = await _enquiryGateway.SaveReplyAsync(enquiryId, reply);

            var enquiry = await _enquiryGateway.GetEnquiryByIdAsync(enquiryId);
            var replies = await _enquiryGateway.GetRepliesForEnquiryAsync(enquiryId);

            var viewModel = new ReplyToEnquiryViewModel
            {
                Enquiry = enquiry,
                Replies = replies
            };

            TempData["SuccessMessage"] = "Your reply has been sent successfully!";

            return View("Reply", viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending reply");

            TempData["ErrorMessage"] = "There was an error sending your reply. Please try again.";

            var enquiry = await _enquiryGateway.GetEnquiryByIdAsync(enquiryId);
            var replies = await _enquiryGateway.GetRepliesForEnquiryAsync(enquiryId);

            var viewModel = new ReplyToEnquiryViewModel
            {
                Enquiry = enquiry,
                Replies = replies
            };

            return View("Reply", viewModel);
        }
    }
}
