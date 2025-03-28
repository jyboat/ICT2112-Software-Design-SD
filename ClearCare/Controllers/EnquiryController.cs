using Microsoft.AspNetCore.Mvc;
using ClearCare.Models;
using ClearCare.Controls;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

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
        public IActionResult Index()
        {
            // Retrieve session values.
            string userRole = HttpContext.Session.GetString("UserRole") ?? "Unknown";
            string userUUID = HttpContext.Session.GetString("UserUUID") ?? "";
            
            // If the user is a Doctor, redirect them to the doctor's list of enquiries.
            if (userRole == "Doctor")
            {
                return RedirectToAction("ListEnquiriesByDoctor", new { userUUID = userUUID });
            }
            
            // Otherwise, render the Enquiry form for patients (or others).
            return View();
        }

        // List all enquiries in memory
        [HttpGet]
        public IActionResult ListEnquiries()
        {
            var enquiries = _enquiryControl.fetchAllEnquiries();
            return View(enquiries);
        }

        // List all enquiries for a particular user (via Firestore)
        [HttpGet]
        public async Task<IActionResult> ListEnquiriesByUser(string userUUID)
        {
            // If userUUID parameter is empty, get it from session.
            if (string.IsNullOrEmpty(userUUID))
            {
                userUUID = HttpContext.Session.GetString("UserUUID") ?? "";
            }
            var userEnquiries = await _enquiryControl.fetchEnquiriesByUserAsync(userUUID);
            return View("ListEnquiries", userEnquiries);
        }

        // List all enquiries for a particular doctor (via Firestore)
        [HttpGet]
        public async Task<IActionResult> ListEnquiriesByDoctor(string userUUID)
        {
            if (string.IsNullOrEmpty(userUUID))
            {
                userUUID = HttpContext.Session.GetString("UserUUID") ?? "";
            }
            var userEnquiries = await _enquiryControl.fetchEnquiriesByDoctorAsync(userUUID);
            return View("ListEnquiries", userEnquiries);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitEnquiry(Enquiry enquiry, string doctorUUID, string topic)
        {
            // Retrieve the user's UUID from session.
            string userUUID = HttpContext.Session.GetString("UserUUID") ?? "";
            
            // Pass both the user UUID and doctor UUID to the control.
            await _enquiryControl.createEnquiryAsync(enquiry, userUUID, doctorUUID, topic);

            ViewData["Name"] = enquiry.Name;
            ViewData["Message"] = enquiry.Message;
            ViewData["UserUUID"] = userUUID;
            ViewData["DoctorUUID"] = doctorUUID;

            return View("EnquiryResult");
        }

        [HttpPost]
        public async Task<IActionResult> SendReply(
            string enquiryId,
            string senderName,         
            string recipientName,
            string recipientEmail,
            string originalMessage,
            string subject,
            string message)
        {
            try
            {
                // Retrieve the user's UUID from session.
                string userUUID = HttpContext.Session.GetString("UserUUID") ?? "";
                
                var reply = new Reply
                {
                    EnquiryId = enquiryId,
                    Subject = subject,
                    Message = message,
                    SenderName = senderName, 
                    RecipientName = recipientName,
                    RecipientEmail = recipientEmail,
                    OriginalMessage = originalMessage,
                    UserUUID = userUUID
                };

                await _enquiryControl.saveReplyAsync(enquiryId, reply);

                return RedirectToAction("reply", new { id = enquiryId, pageNumber = 1 });
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
        public async Task<IActionResult> Reply(string id, int pageNumber = 1)
        {
            try
            {
                var enquiry = await _enquiryControl.fetchEnquiryByFirestoreIdAsync(id);
                if (enquiry == null)
                {
                    return NotFound($"Enquiry with ID {id} not found.");
                }

                // Fetch all replies
                var replies = await _enquiryControl.fetchRepliesForEnquiryAsync(id);

                // Define page size (how many replies to show per page)
                const int pageSize = 10;
                var totalReplies = replies.Count;

                // Calculate how many to skip, then take up to 'pageSize'
                var skip = (pageNumber - 1) * pageSize;
                var pagedReplies = replies
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToList();

                // Populate view model
                var viewModel = new ReplyToEnquiryViewModel
                {
                    Enquiry = enquiry,
                    Replies = pagedReplies
                };

                // Pass pagination info to the view
                ViewBag.CurrentPage = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalReplies = totalReplies;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalReplies / pageSize);

                return View("Reply", viewModel);
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
