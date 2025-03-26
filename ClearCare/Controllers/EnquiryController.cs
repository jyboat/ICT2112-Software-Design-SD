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


        [HttpGet]
        public IActionResult Index()
        {
            // Check the user role from your hardcoded class
            if (HardcodedUUIDs.UserRole == "Doctor")
            {
                // Redirect doctors to ListEnquiriesByDoctor
                return RedirectToAction("ListEnquiriesByDoctor", new { userUuid = HardcodedUUIDs.UserUUID });
            }
            
            // Otherwise, render the Enquiry form for Patients (or others).
            return index();
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


         // List all enquiries for a particular user (via Firestore)
        [HttpGet]
        public async Task<IActionResult> listEnquiriesByDoctor(string userUUID)
        {
            var userEnquiries = await _enquiryControl.fetchEnquiriesByDoctorAsync(userUUID);
            return View("ListEnquiries", userEnquiries);
        }

        [HttpPost]
        public async Task<IActionResult> submitEnquiry(Enquiry enquiry, string userUUID, string doctorUUID, string topic)
        {
            // Pass both userUUID and doctorUUID to the control.
            await _enquiryControl.createEnquiryAsync(enquiry, userUUID, doctorUUID, topic);

            ViewData["Name"] = enquiry.Name;
            ViewData["Message"] = enquiry.Message;
            ViewData["UserUUID"] = userUUID;
            ViewData["DoctorUUID"] = doctorUUID;

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
public async Task<IActionResult> reply(string id, int pageNumber = 1)
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
