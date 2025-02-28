using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ClearCare.Models;
using ClearCare.Gateways;  // Make sure to import your Gateway namespace

namespace ClearCare.Controllers; // Make sure this namespace matches your project's namespace

public class EnquiryController : Controller
{
    private readonly ILogger<EnquiryController> _logger;
    public static List<Enquiry> Enquiries = new List<Enquiry>();
    private readonly EnquiryGateway _enquiryGateway;



    public EnquiryController(ILogger<EnquiryController> logger)
    {
        _logger = logger;
        _enquiryGateway = new EnquiryGateway();
    }

    public IActionResult Index()
    {
        // If you're using the static in-memory list:
        var allEnquiries = Enquiries; // or wherever you get your list
        return View(allEnquiries);
    }



    public IActionResult Privacy()
    {
        // Assuming you have a Privacy view for enquiries as well.
        return View();
    }

    public IActionResult ListEnquiries()
    {
        return View(Enquiries);
    }


    [HttpGet]
    public async Task<IActionResult> ListEnquiriesByUser(string userUUID)
    {
        // If using the Firestore query approach:
        var userEnquiries = await _enquiryGateway.GetEnquiriesForUserAsync(userUUID);

        // Then pass them to a view
        return View("ListEnquiries", userEnquiries);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitEnquiry(Enquiry enquiry)
    {
        _logger.LogInformation($"Received enquiry from {enquiry.Name} with email {enquiry.Email}: {enquiry.Message}");

        enquiry.Id = Guid.NewGuid().ToString();
        Enquiries.Add(enquiry);

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
        // Fetch the enquiry from Firestore using your gateway
        var enquiry = await _enquiryGateway.GetEnquiryByIdAsync(id);

        if (enquiry == null)
        {
            return NotFound($"Enquiry with ID {id} not found.");
        }

        // Set the FirestoreId property
        enquiry.FirestoreId = id;

        // Fetch all replies for this enquiry
        var replies = await _enquiryGateway.GetRepliesForEnquiryAsync(id);

        // Create a view model to pass both enquiry and replies to the view
        var viewModel = new ReplyToEnquiryViewModel
        {
            Enquiry = enquiry,
            Replies = replies
        };

        // Return the view with the view model
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
        // Create a new Reply object
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

        // Save the reply to Firestore
        string replyId = await _enquiryGateway.SaveReplyAsync(enquiryId, reply);

        // Fetch the updated enquiry and replies
        var enquiry = await _enquiryGateway.GetEnquiryByIdAsync(enquiryId);
        var replies = await _enquiryGateway.GetRepliesForEnquiryAsync(enquiryId);

        // Create a view model with the updated data
        var viewModel = new ReplyToEnquiryViewModel
        {
            Enquiry = enquiry,
            Replies = replies
        };

        // Add a success message (optional)
        TempData["SuccessMessage"] = "Your reply has been sent successfully!";

        // Return the same view with the updated data
        return View("Reply", viewModel);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error sending reply");

        // Add an error message
        TempData["ErrorMessage"] = "There was an error sending your reply. Please try again.";

        // Fetch the enquiry and replies again to reload the page
        var enquiry = await _enquiryGateway.GetEnquiryByIdAsync(enquiryId);
        var replies = await _enquiryGateway.GetRepliesForEnquiryAsync(enquiryId);

        var viewModel = new ReplyToEnquiryViewModel
        {
            Enquiry = enquiry,
            Replies = replies
        };

        // Return the same view with the error message
        return View("Reply", viewModel);
    }
}




}
