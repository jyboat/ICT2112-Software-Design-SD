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

    [Route("Add")]
    [HttpGet]
    public async Task<IActionResult> addConsultation()
    {
        ViewBag.UserRole = "Doctor"; // Hardcoded for testing

        var appointments = await manager.getAppointments();

        return View("~/Views/M3T1/Consultation/Add.cshtml", appointments);
    }

    [Route("Add")]
    [HttpPost]
    public async Task<IActionResult> postConsultation(
        string notes,
        string appointmentId,
        bool isCompleted,
        string zoomLink
    )
    {
        // Get the selected appointment
        var appointment = await manager.getAppointmentById(appointmentId);

        if (appointment == null)
        {
            TempData["FlashMsg"] = "Appointment does not exist";
            return RedirectToAction("listConsultations");
        }

        try
        {
            await manager.insertConsultation(appointment, notes, zoomLink, isCompleted);
            await manager.receiveAddStatus(true);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Could not add consultation: {e}");
            await manager.receiveAddStatus(false);

            TempData["FlashMsg"] = "Could not add consultation";
            return RedirectToAction("listConsultations");
        }

        return RedirectToAction("listConsultations");
    }

    [Route("Delete/{consultationId}")]
    [HttpGet]
    public async Task<IActionResult> deleteConsultation(
        string consultationId
    )
    {
        if (string.IsNullOrWhiteSpace(consultationId))
        {
            return RedirectToAction("listConsultations");
        }

        try
        {
            await manager.deleteConsultationById(consultationId);
            return RedirectToAction("listConsultations");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Could not delete consultation: {e}");
            TempData["FlashMsg"] = "Could not delete consultation";
            return RedirectToAction("listConsultations");
        }
    }

    [Route("View/{consultationId}")]
    [HttpGet]
    public async Task<IActionResult> viewConsultation(
        string consultationId
    )
    {
        ViewBag.UserRole = "Doctor"; // Hardcoded for testing

        if (string.IsNullOrWhiteSpace(consultationId)) return RedirectToAction("listConsultations");

        var consultation = await manager.getConsultationById(consultationId);

        if (consultation == null) return RedirectToAction("listConsultations");

        return View("~/Views/M3T1/Consultation/View.cshtml", consultation);
    }
}