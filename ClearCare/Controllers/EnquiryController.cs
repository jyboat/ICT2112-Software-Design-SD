using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ClearCare.Models;

namespace ClearCare.Controllers; // Make sure this namespace matches your project's namespace

public class EnquiryController : Controller
{
    private readonly ILogger<EnquiryController> _logger;
public static List<Enquiry> Enquiries = new List<Enquiry>();

    public EnquiryController(ILogger<EnquiryController> logger)
    {
        _logger = logger;
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
public IActionResult SubmitEnquiry(Enquiry enquiry)
{
    _logger.LogInformation($"Received enquiry from {enquiry.Name} with email {enquiry.Email}: {enquiry.Message}");

    enquiry.Id = Enquiries.Count + 1; // Simple ID assignment
    Enquiries.Add(enquiry);

    ViewData["Name"] = enquiry.Name;
    ViewData["Email"] = enquiry.Email;
    ViewData["Message"] = enquiry.Message;

    return View("EnquiryResult");
}


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // Error view handling
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
