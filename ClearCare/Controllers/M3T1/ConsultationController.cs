using ClearCare.DataSource.M3T1;
using ClearCare.Models.Control.M3T1;
using Microsoft.AspNetCore.Mvc;

namespace ClearCare.Controllers.M3T1;

[Route("Consultation")]
public class ConsultationController : Controller
{
    private readonly ConsultationManagement manager;

    public ConsultationController()
    {
        manager = new ConsultationManagement(new ConsultationGateway());
    }

    [Route("")]
    [HttpGet]
    public async Task<IActionResult> listConsultations(
        int page = 1,
        int pageSize = 10,
        string search = "",
        string isCompleted = "All"
    )
    {
        ViewBag.UserRole = "Doctor"; // Hardcoded for testing

        var sessions = await manager.getConsultations();

        var filteredSessions = manager.applySearchFilter(sessions, search);
        filteredSessions = manager.applyPagination(filteredSessions, page, pageSize);

        // Pass state to ViewBag
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = (filteredSessions.Count + pageSize - 1) / pageSize;
        ViewBag.TotalItems = filteredSessions.Count;

        return View("~/Views/M3T1/Consultation/List.cshtml", filteredSessions);
    }
}