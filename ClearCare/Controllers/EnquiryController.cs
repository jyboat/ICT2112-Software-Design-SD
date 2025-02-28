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
}
