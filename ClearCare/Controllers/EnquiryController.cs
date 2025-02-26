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
        // This method returns the main view associated with the enquiry.
        return View();
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



    [HttpPost]
    public async Task<IActionResult> SubmitEnquiry(Enquiry enquiry)
    {
        _logger.LogInformation($"Received enquiry from {enquiry.Name} with email {enquiry.Email}: {enquiry.Message}");

        enquiry.Id = Enquiries.Count + 1; // Simple ID assignment
        Enquiries.Add(enquiry);

        ViewData["Name"] = enquiry.Name;
        ViewData["Email"] = enquiry.Email;
        ViewData["Message"] = enquiry.Message;

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
