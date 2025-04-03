using Microsoft.AspNetCore.Mvc;
using ClearCare.Models;
using ClearCare.Controls;     // <--- Import the namespace for EnquiryControl
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

        /// <summary>
        ///   Initializes a new instance of the <see cref="EnquiryController"/>
        ///   class.
        /// </summary>
        /// <param name="logger">
        ///   The logger for this controller to log information and errors.
        /// </param>
        /// <param name="enquiryControl">
        ///   The control responsible for handling enquiry-related operations.
        /// </param>
        public EnquiryController(
            ILogger<EnquiryController> logger,
            EnquiryControl enquiryControl
        )
        {
            _logger = logger;
            _enquiryControl = enquiryControl;
        }

        /// <summary>
        ///   Displays the Enquiry Form.  Depending on the user's role, either
        ///   redirects the doctor to their list of enquiries or renders the
        ///   Enquiry form for patients.
        /// </summary>
        /// <returns>
        ///   If the user is a Doctor, redirects to the doctor's list of
        ///   enquiries; otherwise, renders the Enquiry form for patients (or
        ///   others).
        /// </returns>
        public IActionResult Index()
        {
            // Retrieve session values to determine user role and ID.
            string userRole = HttpContext.Session.GetString("Role") ?? "Unknown";
            string userUUID = HttpContext.Session.GetString("UserID") ?? "";

            // If the user is a Doctor, redirect them to the doctor's list of
            // enquiries.
            if (userRole == "Doctor")
            {
                return RedirectToAction(
                    "ListEnquiriesByDoctor",
                    new { userUUID = userUUID }
                );
            }

            // Otherwise, render the Enquiry form for patients (or others).
            return View("~/Views/M3T2/Enquiry/Index.cshtml");
        }

        /// <summary>
        ///   Lists all enquiries in memory (primarily for debugging/testing
        ///   purposes).
        /// </summary>
        /// <returns>A view displaying all enquiries.</returns>
        [HttpGet]
        public IActionResult ListEnquiries()
        {
            var enquiries = _enquiryControl.fetchAllEnquiries();
            return View("~/Views/M3T2/Enquiry/ListEnquiries.cshtml", enquiries);
        }

        /// <summary>
        ///   Lists all enquiries for a particular user, fetched from
        ///   Firestore.
        /// </summary>
        /// <param name="userUUID">The UUID of the user.</param>
        /// <returns>A view displaying the user's enquiries.</returns>
        [HttpGet]
        public async Task<IActionResult> ListEnquiriesByUser(string userUUID)
        {
            // If userUUID parameter is empty, get it from session.
            if (string.IsNullOrEmpty(userUUID))
            {
                userUUID = HttpContext.Session.GetString("UserID") ?? "";
            }
            var userEnquiries = await _enquiryControl.fetchEnquiriesByUserAsync(userUUID);
            return View("~/Views/M3T2/Enquiry/ListEnquiries.cshtml", userEnquiries);
        }

        /// <summary>
        ///   Lists all enquiries for a particular doctor, fetched from
        ///   Firestore.
        /// </summary>
        /// <param name="userUUID">The UUID of the doctor.</param>
        /// <returns>A view displaying the doctor's enquiries.</returns>
        [HttpGet]
        public async Task<IActionResult> ListEnquiriesByDoctor(string userUUID)
        {
            if (string.IsNullOrEmpty(userUUID))
            {
                userUUID = HttpContext.Session.GetString("UserID") ?? "";
            }
            var userEnquiries = await _enquiryControl.fetchEnquiriesByDoctorAsync(userUUID);
            return View("~/Views/M3T2/Enquiry/ListEnquiries.cshtml", userEnquiries);
        }

        /// <summary>
        ///   Submits a new enquiry.
        /// </summary>
        /// <param name="enquiry">The enquiry details.</param>
        /// <param name="doctorUUID">The UUID of the doctor to assign to.</param>
        /// <param name="topic">The topic of the enquiry.</param>
        /// <returns>A view confirming the submission of the enquiry.</returns>
        [HttpPost]
        public async Task<IActionResult> SubmitEnquiry(
            Enquiry enquiry,
            string doctorUUID,
            string topic
        )
        {
            // Retrieve the user's UUID from session.
            string userUUID = HttpContext.Session.GetString("UserID") ?? "";

            // Pass both the user UUID and doctor UUID to the control.
            await _enquiryControl.createEnquiryAsync(
                enquiry,
                userUUID,
                doctorUUID,
                topic
            );

            ViewData["Name"] = enquiry.Name;
            ViewData["Message"] = enquiry.Message;
            ViewData["UserUUID"] = userUUID;
            ViewData["DoctorUUID"] = doctorUUID;
            TempData["SuccessMessage"] = "Enquiry added successfully!";

            return View("~/Views/M3T2/Enquiry/EnquiryResult.cshtml"); // e.g., a "Thank you" page
        }

        /// <summary>
        ///   Sends a reply to an existing enquiry.
        /// </summary>
        /// <param name="enquiryId">The ID of the enquiry being replied to.</param>
        /// <param name="senderName">The name of the person sending the reply.</param>
        /// <param name="recipientName">
        ///   The name of the person receiving the reply.
        /// </param>
        /// <param name="recipientEmail">
        ///   The email address of the person receiving the reply.
        /// </param>
        /// <param name="originalMessage">The original message being replied to.</param>
        /// <param name="subject">The subject of the reply.</param>
        /// <param name="message">The content of the reply.</param>
        /// <returns>
        ///   A redirect to the Reply action, or an Error view if something
        ///   goes wrong.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> SendReply(
            string enquiryId,
            string senderName,
            string recipientName,
            string recipientEmail,
            string originalMessage,
            string subject,
            string message
        )
        {
            try
            {
                // Retrieve the user's UUID from session.
                string userUUID = HttpContext.Session.GetString("UserID") ?? "";

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
                TempData["SuccessMessage"] = "Replied to enquiry successfully!";

                return RedirectToAction("reply", new { id = enquiryId, pageNumber = 1 });
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

        /// <summary>
        ///   Displays the replies to a specific enquiry, with pagination.
        /// </summary>
        /// <param name="id">The ID of the enquiry.</param>
        /// <param name="pageNumber">The current page number (default is 1).</param>
        /// <returns>
        ///   A view displaying the replies to the enquiry, with pagination, or
        ///   a NotFound result if the enquiry does not exist, or an Error view
        ///   if an exception occurs.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Reply(string id, int pageNumber = 1)
        {
            try
            {
                var enquiry = await _enquiryControl.fetchEnquiryByFirestoreIdAsync(
                    id
                );
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

                var viewModel = new ReplyToEnquiryDTO
                {
                    Enquiry = enquiry,
                    Replies = pagedReplies
                };

                // Pass pagination info to the view
                ViewBag.CurrentPage = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalReplies = totalReplies;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalReplies / pageSize);

                return View("~/Views/M3T2/Enquiry/Reply.cshtml", viewModel);
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
