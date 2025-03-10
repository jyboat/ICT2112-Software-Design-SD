using ClearCare.DataSource;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

// Request Handling
[Route("api/[controller]")]
[ApiController]
public class ServiceAppointmentsController : Controller
{
    private readonly ServiceAppointmentManagement ServiceAppointmentManagement;
    private readonly AutomaticAppointmentScheduler AutomaticAppointmentScheduler;
    private readonly CalendarManagement _calendarManagement;
    private readonly NurseAvailabilityManagement _nurseAvailabilityManagement;

    public ServiceAppointmentsController()
    {
        ServiceAppointmentManagement = new ServiceAppointmentManagement();
        _calendarManagement = new CalendarManagement(ServiceAppointmentManagement, _nurseAvailabilityManagement);
        AutomaticAppointmentScheduler = new AutomaticAppointmentScheduler();
    }

    // GET All appointment
    [HttpGet]
    public async Task<IActionResult> RetrieveAllAppointment()
    {
        // await to wait for task complete or data to retrieve before executing
        var appointment = await ServiceAppointmentManagement.RetrieveAllAppointments();

        // No record exists
        if (appointment != null && appointment.Any())
        {
            return View("Index", appointment);
        }
        else
        {
            return NotFound(new { Message = "Appointment not found" });
        }
    }

    [HttpGet]
    [Route("GetAppointmentsForCalendar")]
    public async Task<JsonResult> GetAppointmentsForCalendar([FromQuery] string? doctorId, [FromQuery] string? patientId, [FromQuery] string? nurseId)
    {
        return await _calendarManagement.GetAppointmentsForCalendar(doctorId, patientId, nurseId);
    }


    [HttpGet]
    [Route("Calendar")]
    public IActionResult Calendar()
    {
        return View("Calendar");
    }

    // Implement IRetrieveAll
    public async Task<List<Dictionary<string, object>>> RetrieveAll()
    {
        return await ServiceAppointmentManagement.RetrieveAllAppointments();
    }

    [HttpGet]
    [Route("CreatePage")]
    public IActionResult Create()
    {
        ViewBag.Patients = ServiceAppointmentManagement.GetAllPatients();
        ViewBag.Nurses = ServiceAppointmentManagement.GetAllNurses();
        AutomaticAppointmentScheduler.TestAutoAssignment();
        return View("CreateServiceAppt"); // Render the form
    }


    // POST: Create a new appointment
    // Route: localhost:5007/api/ServiceAppointments/Create that retriggers POST
    // Need add form and button to retrieve data and trigger this
    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> CreateAppointment([FromBody] Dictionary<string, JsonElement> requestData)
    {
        var appointment = await ServiceAppointmentManagement.CreateAppt(
            requestData["AppointmentId"].GetString() ?? "",
            requestData["PatientId"].GetString() ?? "",
            requestData.ContainsKey("NurseId") ? requestData["NurseId"].GetString() ?? "" : "",
            requestData["DoctorId"].GetString() ?? "",
            requestData["ServiceTypeId"].GetString() ?? "",
            requestData["Status"].GetString() ?? "",
            requestData["DateTime"].GetDateTime(),
            requestData["Slot"].GetInt32(),
            requestData["Location"].GetString() ?? "");

        // No record exists
        if (appointment != "" && appointment.Any())
        {
            return Ok(new { Message = "Appointment created successfully", AppointmentId = appointment });
        }
        else
        {
            return NotFound(new { Message = "Error Creating Appointment" });
        }
    }

    // // GET: Retrieve an appointment
    // Route localhost:5007/api/ServiceAppointments/Retrieve/{Id} that retriggers GET
    [HttpGet]
    [Route("Retrieve/{appointmentId}")]
    public async Task<IActionResult> GetAppointment(string appointmentId)
    {
        var appointmentDetail = await ServiceAppointmentManagement.GetAppt(appointmentId);

        if (appointmentDetail != null && appointmentDetail.Any())
        {
            return View("AppointmentDetails", appointmentDetail);
        }
        else
        {
            return NotFound(new { Message = "Error" });
        }
    }

    // [HttpPost]
    // [Route("Create")]
    // public async Task<IActionResult> AutoAppointment([FromBody] Dictionary<string, JsonElement> requestData)
    // {
    //     // Map JSON data to model
    //     var appointment = ServiceAppointment.setApptDetails(
    //         requestData["AppointmentId"].GetString() ?? "",
    //         requestData["PatientId"].GetString() ?? "",
    //         requestData.ContainsKey("NurseId") ? requestData["NurseId"].GetString() ?? "" : "",
    //         requestData["DoctorId"].GetString() ?? "",
    //         requestData["ServiceTypeId"].GetString() ?? "",
    //         requestData["Status"].GetString() ?? "",
    //         requestData["DateTime"].GetDateTime(),
    //         requestData["Slot"].GetInt32(),
    //         requestData["Location"].GetString() ?? ""
    //     );

    //     string appointmentId = await _gateway.CreateAppointmentAsync(appointment);
    //     return Ok(new { Message = "Appointment created successfully"});
    // }
    
    // update appointment
    [HttpPut]
    [Route("Update")]
    public async Task<IActionResult> UpdateAppointment([FromBody] Dictionary<string, JsonElement> requestData)
    {
        try
        {
            var result = await ServiceAppointmentManagement.UpdateAppt(
                requestData["AppointmentId"].GetString() ?? "",
                requestData["PatientId"].GetString() ?? "",
                requestData.ContainsKey("NurseId") ? requestData["NurseId"].GetString() ?? "" : "",
                requestData["DoctorId"].GetString() ?? "",
                requestData["ServiceTypeId"].GetString() ?? "",
                requestData["Status"].GetString() ?? "",
                requestData["DateTime"].GetDateTime(),
                requestData["Slot"].GetInt32(),
                requestData["Location"].GetString() ?? "");
            
            // TODO - Should we strictly return a view or can we return a JSON response? - dinie
            if (result)
            {
                return Ok(new { Success = true, Message = "Appointment updated successfully" });
            }
            else
            {
                return BadRequest(new { Success = false, Message = "Failed to update appointment" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Success = false, Message = "An error occurred", Error = ex.Message });
        }
    }
}
