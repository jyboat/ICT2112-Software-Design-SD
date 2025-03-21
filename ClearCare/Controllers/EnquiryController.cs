using Microsoft.AspNetCore.Mvc;
using ClearCare.Models;
using ClearCare.Controls;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ClearCare.Controllers
{
    public class EnquiryController : Controller
    {
        private readonly ILogger<EnquiryController> _logger;
        private readonly EnquiryControl _enquiryControl;

        public EnquiryController(ILogger<EnquiryController> logger, EnquiryControl enquiryControl)
        {
            _logger = logger;
            _enquiryControl = enquiryControl;
        }

        // Display the Enquiry Form
        public IActionResult index()
        {
            return View();
        }

        // List all enquiries in memory
        [HttpGet]
        public IActionResult listEnquiries()
        {
            var enquiries = _enquiryControl.fetchAllEnquiries();
            return View(enquiries);
        }

        // List all enquiries for a particular user (via Firestore)
        [HttpGet]
        public async Task<IActionResult> listEnquiriesByUser(string userUUID)
        {
            var userEnquiries = await _enquiryControl.fetchEnquiriesByUserAsync(userUUID);
            return View("ListEnquiries", userEnquiries);
        }

        // Submit an enquiry (POST)
        [HttpPost]
        public async Task<IActionResult> submitEnquiry(Enquiry enquiry)
        {
            await _enquiryControl.createEnquiryAsync(enquiry);

            ViewData["Name"] = enquiry.Name;
            ViewData["Email"] = enquiry.Email;
            ViewData["Message"] = enquiry.Message;

            return View("EnquiryResult");
        }

        [HttpPost]
        public async Task<IActionResult> sendReply(
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
                var reply = new Reply
                {
                    EnquiryId = enquiryId,
                    Subject = subject,
                    Message = message,
                    RecipientName = recipientName,
                    RecipientEmail = recipientEmail,
                    OriginalMessage = originalMessage,
                    UserUUID = userUUID
                };

                await _enquiryControl.saveReplyAsync(enquiryId, reply);

                var updatedEnquiry = await _enquiryControl.fetchEnquiryByFirestoreIdAsync(enquiryId);
                var updatedReplies = await _enquiryControl.fetchRepliesForEnquiryAsync(enquiryId);

                var viewModel = new ReplyToEnquiryViewModel
                {
                    Enquiry = updatedEnquiry,
                    Replies = updatedReplies
                };

                return View("Reply", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending reply");
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> reply(string id)
        {
            try
            {
                var enquiry = await _enquiryControl.fetchEnquiryByFirestoreIdAsync(id);
                if (enquiry == null)
                {
                    return NotFound($"Enquiry with ID {id} not found.");
                }

                var replies = await _enquiryControl.fetchRepliesForEnquiryAsync(id);

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
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
        }
    }
}
