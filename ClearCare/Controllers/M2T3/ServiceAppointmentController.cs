using ClearCare.DataSource;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ClearCare.Interfaces;
using ClearCare.Control;
using ClearCare.Models.Interface;
using Google.Api;


// Request Handling
[Route("api/[controller]")]
[ApiController]
public class ServiceAppointmentsController : Controller
{
    private readonly AutomaticAppointmentScheduler AutomaticAppointmentScheduler;
    private readonly CalendarManagement _calendarManagement;
    private readonly NurseAvailabilityManagement _nurseAvailabilityManagement;
    private readonly NotificationManager _notificationManagement;
    private readonly ServiceTypeManager _serviceTypeManagement;
    private readonly ManualAppointmentScheduler _manualAppointmentScheduler;
    private readonly IUserList _userList;

    public ServiceAppointmentsController()
    {




        _nurseAvailabilityManagement = new NurseAvailabilityManagement();

        _notificationManagement = new NotificationManager();

        _serviceTypeManagement = new ServiceTypeManager();

        _calendarManagement = new CalendarManagement();

        AutomaticAppointmentScheduler = new AutomaticAppointmentScheduler();

        _manualAppointmentScheduler = new ManualAppointmentScheduler();

        _userList = new AdminManagement(new UserGateway());

    }


    [HttpGet]
    [Route("GetAppointmentsForCalendar")]
    public async Task<JsonResult> getAppointmentsForCalendar(
        [FromQuery] string? doctorId,
        [FromQuery] string? patientId,
        [FromQuery] string? nurseId,
        [FromQuery] string? location,
        [FromQuery] string? service,
        [FromQuery] string? timeSlot)
    {
        return await _calendarManagement.getAppointmentsForCalendar(doctorId, patientId, nurseId, location, service, timeSlot);
    }

    [HttpGet]
    [Route("Index")]
    public async Task<IActionResult> calendar()
    {
        var users = await _userList.retrieveAllUsers();
        var usersFiltered = users
        .Select(p => new
        {
            UserID = p.getProfileData()["UserID"].ToString(),
            Name = p.getProfileData()["Name"].ToString(),
            Role = p.getProfileData()["Role"].ToString()
        })
        .ToList();

        // Filter for Doctors
        ViewBag.Doctors = usersFiltered
            .Where(u => u.Role == "Doctor")
            .ToList();

        // Filter for Patients
        ViewBag.Patients = usersFiltered
            .Where(u => u.Role == "Patient")
            .ToList();

        // Filter for Nurses
        ViewBag.Nurses = usersFiltered
            .Where(u => u.Role == "Nurse")
            .ToList();

        var services = await _manualAppointmentScheduler.getServices();
        services = services
        .Where(s => s.Status != "discontinued") // Exclude discontinued services
        .ToList();

        var uniqueLocations = services
        .GroupBy(s => s.Modality)  // Group by 'Modality' (location)
        .Select(g => g.First())    // Take the first element from each group (removes duplicates)
        .ToList();

        ViewBag.UniqueLocations = uniqueLocations;
        ViewBag.ServiceNames = services;
        ViewBag.CurrentDoctorId = HttpContext.Session.GetString("UserID");

        return View("~/Views/M2T3/ServiceAppointments/Calendar.cshtml");
    }


    [HttpGet]
    [Route("AutoScheduling")]
    public async Task<IActionResult> addPatients()
    {
        var (appointments, patientNames) = await AutomaticAppointmentScheduler.getUnscheduledPatients();
        ViewBag.Appointment = appointments;
        ViewBag.PatientNames = patientNames;
        return View("~/Views/M2T3/ServiceAppointments/AddPatientsAutoScheduling.cshtml");
    }


    [HttpPut]
    [Route("Update")]
    public async Task<IActionResult> updateAppointment([FromBody] Dictionary<string, JsonElement> requestData)
    {
        try
        {
            Console.WriteLine("Received JSON request body: " + JsonSerializer.Serialize(requestData));

            string appointmentId = requestData["AppointmentId"].GetString();
            string patientId = requestData["PatientId"].GetString() ?? "";
            string nurseId = requestData.ContainsKey("NurseId") ? requestData["NurseId"].GetString() ?? "" : "";
            string doctorId = requestData["DoctorId"].GetString() ?? "";
            string Service = requestData["Service"].GetString() ?? "";
            string status = requestData["Status"].GetString() ?? "";
            DateTime dateTime = requestData["DateTime"].GetDateTime();
            int slot = requestData["Slot"].GetInt32();
            string location = requestData["Location"].GetString() ?? "";

            // Use the ManualAppointmentScheduler to handle validation and updating
            bool result = await _manualAppointmentScheduler.rescheduleAppointment(
                appointmentId, patientId, nurseId, doctorId, Service, status, dateTime, slot, location
            );

            if (result)
            {
                return Ok(new { Success = true, Message = "Appointment updated successfully" });
            }
            else
            {
                return BadRequest(new { Success = false, Message = "Failed to update appointment" });
            }
        }
        catch (InvalidOperationException ex)
        {
            // Log the exception message
            Console.WriteLine($"Error: {ex.Message}");
            return BadRequest(new { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Success = false, Message = "An error occurred", Error = ex.Message });
        }
    }

    // delete appointment
    [HttpDelete]
    [Route("Delete/{appointmentId}")]
    public async Task<IActionResult> deleteAppointment(string appointmentId)
    {
        try
        {
            var result = await _manualAppointmentScheduler.deleteAppointment(appointmentId);
            // TODO - Should we strictly return a view or can we return a JSON response? - dinie
            if (result)
            {
                return Ok(new { Success = true, Message = "Appointment deleted successfully" });
            }
            else
            {
                return BadRequest(new { Success = false, Message = "Failed to delete appointment" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Success = false, Message = "An error occurred", Error = ex.Message });
        }
    }

    // Auto Interface
    [HttpPost]
    [Route("AutoAppointment")]
    public async Task<IActionResult> autoAppointment([FromForm] string appointmentsJson, [FromForm] string algorithm)
    {
        // Deserialize the JSON into a list of dictionaries
        var rawData = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(appointmentsJson);

        if (rawData == null || !rawData.Any())
        {
            return BadRequest(new { Message = "No appointments received." });
        }

        var appointments = new List<ServiceAppointment>();

        foreach (var item in rawData)
        {
            var patient = item["patientId"]?.ToString();
            var service = item["Service"]?.ToString();

            if (!string.IsNullOrEmpty(patient) && !string.IsNullOrEmpty(service))
            {
                appointments.Add(ServiceAppointment.setApptDetails(
                    patientId: patient,
                    nurseId: "",
                    doctorId: "",
                    Service: service,
                    status: "Pending",
                    dateTime: DateTime.UtcNow,
                    slot: 0,
                    location: "Physical"
                ));
            }
        }

        Console.WriteLine("Selected Algorithm: " + algorithm);
        foreach (var appt in appointments)
        {
            Console.WriteLine($"Patient ID: {appt.getAttribute("PatientId")}, Service: {appt.getAttribute("Service")}, DateTime: {appt.getAttribute("Datetime")}");
        }

        if (algorithm == "Preferred")
        {
            AutomaticAppointmentScheduler.setAlgorithm(new PreferredNurseStrategy());
        }
        else if (algorithm == "Earliest")
        {
            AutomaticAppointmentScheduler.setAlgorithm(new EarliestsPossibleTimeSlotStrategy());
        }

        // Pass this full appointment list to your scheduler
        var assignedAppointments = await AutomaticAppointmentScheduler.automaticallyScheduleAppointment(appointments);

        return Ok(new
        {
            Message = "Auto appointment scheduling complete.",
            Assigned = assignedAppointments.Select(a => new
            {
                AppointmentId = a.getAttribute("AppointmentId"),
                PatientId = a.getAttribute("PatientId"),
                NurseId = a.getAttribute("NurseId"),
                Service = a.getAttribute("Service"),
                Slot = a.getAttribute("Slot"),
                Datetime = a.getAttribute("Datetime")
            })
        });
    }

    [HttpPost]
    [Route("AddAppt")]
    public async Task<IActionResult> addAppt([FromBody] Dictionary<string, JsonElement> requestData)
    {
        string jsonRequestBody = JsonSerializer.Serialize(requestData);

        Console.WriteLine("Received JSON request body: " + jsonRequestBody);

        string appointmentId = requestData["AppointmentId"].GetString() ?? "";
        string patientId = requestData["PatientId"].GetString() ?? "";
        string nurseId = requestData.ContainsKey("NurseId") ? requestData["NurseId"].GetString() ?? "" : "";
        string doctorId = requestData["DoctorId"].GetString() ?? "";
        string Service = requestData["Service"].GetString() ?? "";
        string status = "Scheduled"; // hardcoded to always set Scheduled as the default status
        DateTime dateTime = requestData["DateTime"].GetDateTime();
        int slot = requestData["Slot"].GetInt32();
        string location = requestData["Location"].GetString() ?? "";

        try
        {
            string createdAppointmentId = await _manualAppointmentScheduler.scheduleAppointment(
                patientId, nurseId, doctorId, Service, status, dateTime, slot, location
            );

            return Ok(new { Message = "Appointment created successfully", AppointmentId = createdAppointmentId });
        }
        catch (InvalidOperationException ex)
        {
            // Log the exception message
            Console.WriteLine($"Error: {ex.Message}");
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            return StatusCode(500, new { Message = "An unexpected error occurred." });
        }
    }

    [HttpGet]
    [Route("GetSuggestedPatients")]
    public async Task<IActionResult> getSuggestedPatients()
    {
        var result = await _calendarManagement.getSuggestedPatients();
        return Ok(result);
    }
}