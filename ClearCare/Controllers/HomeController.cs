using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ClearCare.Models;
using ClearCare.DataSource;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;

namespace ClearCare.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var userRole = HttpContext.Session.GetString("Role");

        // Restrict access to only Doctor or Nurse
        if (userRole == null) 
        {
            Console.WriteLine("You do not have permission to access this page.");
            return RedirectToAction("DisplayLogin", "Login"); 
        }

        return View();
    }

    public IActionResult Privacy()
    {
        var userRole = HttpContext.Session.GetString("Role");
        
        // Restrict access to only Doctor or Nurse
        if (userRole != "Doctor" && userRole != "Nurse") 
        {
            Console.WriteLine("You do not have permission to access this page.");
            return RedirectToAction("DisplayLogin", "Login"); 
        }

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
