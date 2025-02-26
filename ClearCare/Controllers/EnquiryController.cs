using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ClearCare.Models;

namespace ClearCare.Controllers; // Make sure this namespace matches your project's namespace

public class EnquiryController : Controller
{
    private readonly ILogger<EnquiryController> _logger;

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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // Error view handling
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
