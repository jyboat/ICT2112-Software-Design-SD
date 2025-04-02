using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using ClearCare.Models.Control.M3T2;
using ClearCare.Models.Entities.M3T2;
using ClearCare.Models.DTO.M3T2;

namespace ClearCare.Controllers.M3T2
{
    public class EnquiryController : Controller
    {
        private readonly ILogger<EnquiryController> _logger;
        private readonly EnquiryControl _enquiryControl;

        // Inject both logger and EnquiryControl
        public EnquiryController(ILogger<EnquiryController> logger, EnquiryControl enquiryControl)
        {
            _logger = logger;
            _enquiryControl = enquiryControl;
        }

        // Display the Enquiry Form
        public IActionResult Index()
        {
            // Possibly show an empty form or a list of all enquiries
            return View("~/Views/M3T2/Enquiry/Index.cshtml");
        }

        // List all enquiries in memory
        [HttpGet]
        public IActionResult ListEnquiries()
        {
            // The controller calls the control to fetch data
            var enquiries = _enquiryControl.FetchAllEnquiries();
            return View(enquiries);
        }

        // List all enquiries for a particular user (via Firestore)
        [HttpGet]
        public async Task<IActionResult> ListEnquiriesByUser(string userUUID)
        {
            var userEnquiries = await _enquiryControl.FetchEnquiriesByUserAsync(userUUID);
            return View("~/Views/M3T2/Enquiry/ListEnquiries.cshtml", userEnquiries);
        }

        // Submit an enquiry (POST)
        [HttpPost]
        public async Task<IActionResult> SubmitEnquiry(Enquiry enquiry)
        {
            // Let the control handle the logic
            await _enquiryControl.CreateEnquiryAsync(enquiry);

            // Pass data to the View using ViewData (or a strongly-typed model)
            ViewData["Name"] = enquiry.Name;
            ViewData["Email"] = enquiry.Email;
            ViewData["Message"] = enquiry.Message;

            return View("~/Views/M3T2/Enquiry/EnquiryResult.cshtml"); // e.g., a "Thank you" page
        }

        [HttpPost]
        public async Task<IActionResult> SendReply(
    string enquiryId,
    string recipientName,
    string recipientEmail,
    string originalMessage,
    string userUUID,
    string subject,
    string message)
        {
            try
            {
                // 1. Build the Reply object
                var reply = new Reply
                {
                    EnquiryId = enquiryId,
                    Subject = subject,
                    Message = message,
                    RecipientName = recipientName,
                    RecipientEmail = recipientEmail,
                    OriginalMessage = originalMessage,
                    UserUUID = userUUID
                    // CreatedAt can be set here or in the Gateway (e.g., SaveReplyAsync)
                };

                // 2. Save the reply (through your Control or Gateway)
                await _enquiryControl.SaveReplyAsync(enquiryId, reply);

                // 3. Fetch the updated Enquiry and Replies
                var updatedEnquiry = await _enquiryControl.FetchEnquiryByFirestoreIdAsync(enquiryId);
                var updatedReplies = await _enquiryControl.FetchRepliesForEnquiryAsync(enquiryId);

                // 4. Create a new ViewModel with updated data
                var viewModel = new ReplyToEnquiryDTO
                {
                    Enquiry = updatedEnquiry,
                    Replies = updatedReplies
                };

                // 5. Return the same "Reply" view with the updated data
                return View("~/Views/M3T2/Enquiry/Reply.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending reply");
                return View("Error", new ErrorDTO
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
        }



        // EnquiryController.cs
        [HttpGet]
        public async Task<IActionResult> Reply(string id)
        {
            try
            {
                var enquiry = await _enquiryControl.FetchEnquiryByFirestoreIdAsync(id);
                if (enquiry == null)
                {
                    return NotFound($"Enquiry with ID {id} not found.");
                }

                // Use EnquiryControl for replies too
                var replies = await _enquiryControl.FetchRepliesForEnquiryAsync(id);

                var viewModel = new ReplyToEnquiryDTO
                {
                    Enquiry = enquiry,
                    Replies = replies
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving enquiry with ID {id}");
                return View("Error", new ErrorDTO
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
        }


    }
}
