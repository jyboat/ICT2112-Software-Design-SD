using ClearCare.API;
using ClearCare.DataSource.M3T1;
using ClearCare.Models.Control.M3T1;
using ClearCare.Models.DTO.M3T1;
using ClearCare.Models.Interfaces.M3T1;
using Microsoft.AspNetCore.Mvc;

namespace ClearCare.Controllers.M3T1;

[Route("Consultation")]
public class ConsultationController : Controller
{
    private readonly ConsultationManagement manager;

    private const string ZOOM_ACCESS_TOKEN_KEY = "cc-zoomAccessToken";
    private const string ZOOM_REFRESH_TOKEN_KEY = "cc-zoomRefreshToken";
    private const string ZOOM_ACCESS_TOKEN_SERVER_KEY = "cc-zoomAccessToken-Server";

    public ConsultationController()
    {
        var clientId = Environment.GetEnvironmentVariable("ZOOM_CLIENT_ID");
        var clientSecret = Environment.GetEnvironmentVariable("ZOOM_CLIENT_SECRET");

        if (clientId == null || clientSecret == null)
        {
            Console.WriteLine(
                "Client ID/secret for Zoom API was not specified, setting to empty string");
            clientId = "";
            clientSecret = "";
        }

        manager = new ConsultationManagement(new ConsultationGateway(), new ZoomApi(clientId, clientSecret));
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
        IZoomApi.MeetingResponse? response = null;

        string? token = Request.Cookies[ZOOM_ACCESS_TOKEN_SERVER_KEY];

        if (token == null)
        {
            // Generate a new server-to-server token
            var newToken = await manager.generateAccessToken("", "");

            if (newToken != null)
            {
                Response.Cookies.Append(ZOOM_ACCESS_TOKEN_SERVER_KEY, newToken.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    // SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(1)
                });
                token = newToken.AccessToken;
            }
        }

        response = await manager.generateZoomLink(token);
        Console.WriteLine($"Response: {response.JoinUrl}");

        // if (Request.Cookies.ContainsKey(ZOOM_ACCESS_TOKEN_KEY))
        // {
        //     Console.WriteLine($"Access token: {Request.Cookies[ZOOM_ACCESS_TOKEN_KEY]}");
        //     response = await manager.generateZoomLink(Request.Cookies[ZOOM_ACCESS_TOKEN_KEY]);
        //     Console.WriteLine($"Response: {response.JoinUrl}");
        // }

        return View("~/Views/M3T1/Consultation/Add.cshtml", new AddConsultationDTO(
            appointments, response
        ));
    }

    [Route("Add")]
    [HttpPost]
    public async Task<IActionResult> postConsultation(
        string notes,
        string appointmentId,
        bool isCompleted,
        string zoomLink,
        string zoomPwd
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
            await manager.insertConsultation(appointment, notes, zoomLink, zoomPwd, isCompleted);
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

    [Route("Edit/{consultationId}")]
    [HttpGet]
    public async Task<IActionResult> editConsultation(
        string consultationId
    )
    {
        if (string.IsNullOrWhiteSpace(consultationId)) return RedirectToAction("listConsultations");

        ViewBag.UserRole = "Doctor"; // Hardcoded for testing

        var consultation = await manager.getConsultationById(consultationId);

        if (consultation == null)
        {
            TempData["FlashMsg"] = "No such consultation exists!";
            return RedirectToAction("listConsultations");
        }

        var appointments = await manager.getAppointments();

        return View("~/Views/M3T1/Consultation/Edit.cshtml", new EditConsultationDTO(appointments, consultation));
    }

    [Route("Edit/{consultationId}")]
    [HttpPost]
    public async Task<IActionResult> postEditConsultation(
        string consultationId,
        string appointmentId,
        string notes,
        string zoomLink,
        bool isCompleted
    )
    {
        var appt = await manager.getAppointmentById(appointmentId);
        if (appt == null)
        {
            TempData["FlashMsg"] = "No such appointment exists!";
            return RedirectToAction("listConsultations");
        }

        try
        {
            await manager.updateConsultationById(consultationId, appt, notes, zoomLink, isCompleted);
            await manager.receiveUpdateStatus(true);

            return RedirectToAction("listConsultations");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await manager.receiveUpdateStatus(false);

            TempData["FlashMsg"] = "Could not update consultation.";
            return RedirectToAction("listConsultations");
        }
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

    [HttpGet("Zoom/Auth/Redirect")]
    public IActionResult redirectToOAuth()
    {
        return Redirect(manager.getOAuthZoomRedirectUri(
            Url.Action("oAuthCallback", null, null, Request.Scheme))
        );
    }

    // Zoom OAuth
    [HttpGet("Zoom/Auth/Callback")]
    public async Task<IActionResult> oAuthCallback(
        string code
    )
    {
        // Send POST request
        var token = await manager.generateAccessToken(code,
            Url.Action("addConsultation", null, null, Request.Scheme));

        if (token != null)
        {
            Response.Cookies.Append(ZOOM_REFRESH_TOKEN_KEY, token.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                // SameSite = SameSiteMode.Strict,
                // Refresh tokens expire in 90 days, see
                // https://developers.zoom.us/docs/integrations/oauth/#refresh-an-access-token
                Expires = DateTime.UtcNow.AddDays(90)
            });
            Response.Cookies.Append(ZOOM_ACCESS_TOKEN_KEY, token.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                // SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            });
        }

        return RedirectToAction("addConsultation");
    }
}