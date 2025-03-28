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


// Request Handling
[Route("api/[controller]")]
[ApiController]
public class ServiceAppointmentsController : Controller
{
    private readonly ServiceAppointmentManagement ServiceAppointmentManagement;
    private readonly AutomaticAppointmentScheduler AutomaticAppointmentScheduler;
    private readonly CalendarManagement _calendarManagement;
    private readonly NurseAvailabilityManagement _nurseAvailabilityManagement;
    private readonly NotificationManager _notificationManagement;
    private readonly ServiceTypeManager _serviceTypeManagement;
    private readonly ManualAppointmentScheduler _manualAppointmentScheduler;

    public ServiceAppointmentsController()
    {

        ServiceAppointmentManagement = new ServiceAppointmentManagement();


        _nurseAvailabilityManagement = new NurseAvailabilityManagement();
        
        _notificationManagement = new NotificationManager();
        
        _serviceTypeManagement = new ServiceTypeManager();

        _calendarManagement = new CalendarManagement();

        AutomaticAppointmentScheduler = new AutomaticAppointmentScheduler();
        _manualAppointmentScheduler = new ManualAppointmentScheduler();

    }

    // GET All appointment
    [HttpGet]
    public async Task<IActionResult> RetrieveAllAppointment()
    {
        // retrieve appointments
        var appointments = await ServiceAppointmentManagement.RetrieveAllAppointments();
    
        // check if appointments exist
        if (appointments != null && appointments.Any())
        {
            // convert to dictionary format for the view
            // var appointmentDicts = appointments.Select(a => a.ToFirestoreDictionary()).ToList();
            return View("~/Views/M2T3/ServiceAppointments/Index.cshtml", appointments);
        }
        else
        {
            return NotFound(new { Message = "Appointment not found" });
        }
    }

    [HttpGet]
    [Route("GetAppointmentsForCalendar")]
    public async Task<JsonResult> getAppointmentsForCalendar(
        [FromQuery] string? doctorId,
        [FromQuery] string? patientId,
        [FromQuery] string? nurseId,
        [FromQuery] string? location,
        [FromQuery] string? service)
    {
        return await _calendarManagement.getAppointmentsForCalendar(doctorId, patientId, nurseId, location, service);
    }

    [HttpGet]
    [Route("Calendar")]
    public async Task<IActionResult> Calendar()
    {
        ViewBag.Patients = ServiceAppointmentManagement.GetAllPatients();
        ViewBag.Nurses = ServiceAppointmentManagement.GetAllNurses();
        ViewBag.ServiceNames = await ServiceAppointmentManagement.GetServiceTypeNames();
        ViewBag.DoctorId = "DOC001"; // TODO - hardcoded for now, will retrieve from session later

        return View("~/Views/M2T3/ServiceAppointments/Calendar.cshtml");
    }

    [HttpGet]
    [Route("CreatePage")]
    public IActionResult Create()
    {
        ViewBag.Patients = ServiceAppointmentManagement.GetAllPatients();
        ViewBag.Nurses = ServiceAppointmentManagement.GetAllNurses();
        return View("~/Views/M2T3/ServiceAppointments/CreateServiceAppt.cshtml"); // Render the form
    }

    [HttpGet]
    [Route("AutoScheduling")]
    public async Task<IActionResult> AddPatients()
    {
        ViewBag.Appointment = await ServiceAppointmentManagement.getUnscheduledPatients();
        // ViewBag.Services = await ServiceAppointmentManagement.getAllServices();
        return View("~/Views/M2T3/ServiceAppointments/AddPatientsAutoScheduling.cshtml");
    }

    // POST: Create a new appointment
    // Route: localhost:5007/api/ServiceAppointments/Create that retriggers POST
    // Need add form and button to retrieve data and trigger this
    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> CreateAppointment([FromBody] Dictionary<string, JsonElement> requestData)
    {
        var appointment = await ServiceAppointmentManagement.CreateAppointment(
            requestData["PatientId"].GetString() ?? "",
            requestData.ContainsKey("NurseId") ? requestData["NurseId"].GetString() ?? "" : "",
            requestData["DoctorId"].GetString() ?? "",
            requestData["Service"].GetString() ?? "",
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
    [Route("Retrieve/{documentId}")]
    public async Task<IActionResult> GetAppointment(string documentId)
    {
        ServiceAppointment appointment = await ServiceAppointmentManagement.getAppointmentByID(documentId);

        if (appointment != null)
        {
            return View("~/Views/M2T3/ServiceAppointments/AppointmentDetails.cshtml", appointment);
        }
        else
        {
            return NotFound(new { Message = "Error" });
        }
    }

    // update appointment
    // [HttpPut]
    // [Route("Update")]
    // public async Task<IActionResult> UpdateAppointment([FromBody] Dictionary<string, JsonElement> requestData)
    // {
    //     try
    //     {
    //         Console.WriteLine("Received JSON request body: " + JsonSerializer.Serialize(requestData));
    //
    //         ServiceAppointment appointment = await ServiceAppointmentManagement.getAppointmentByID(requestData["AppointmentId"].GetString());
    //         // Entity Method 
    //         ServiceAppointment appt = appointment.updateServiceAppointementById(
    //                 appointment,
    //                 requestData["PatientId"].GetString() ?? "",
    //                 requestData.ContainsKey("NurseId") ? requestData["NurseId"].GetString() ?? "" : "",
    //                 requestData["DoctorId"].GetString() ?? "",
    //                 requestData["Service"].GetString() ?? "",
    //                 requestData["Status"].GetString() ?? "",
    //                 requestData["DateTime"].GetDateTime(),
    //                 requestData["Slot"].GetInt32(),
    //                 requestData["Location"].GetString() ?? ""
    //                 );
    //         var result = await ServiceAppointmentManagement.UpdateAppointment(appt);
    //
    //
    //         // TODO - Should we strictly return a view or can we return a JSON response? - dinie
    //         if (result)
    //         {
    //             return Ok(new { Success = true, Message = "Appointment updated successfully" });
    //         }
    //         else
    //         {
    //             return BadRequest(new { Success = false, Message = "Failed to update appointment" });
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, new { Success = false, Message = "An error occurred", Error = ex.Message });
    //     }
    // }
    
    [HttpPut]
    [Route("Update")]
    public async Task<IActionResult> UpdateAppointment([FromBody] Dictionary<string, JsonElement> requestData)
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
            bool result = await _manualAppointmentScheduler.RescheduleAppointment(
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
    public async Task<IActionResult> DeleteAppointment(string appointmentId)
    {
        try
        {
            var result = await ServiceAppointmentManagement.DeleteAppointment(appointmentId);
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

    // Test Auto Interface
    [HttpPost]
    [Route("TestAutoAppointment")]
    public IActionResult TestAutoAppointment([FromForm] string appointmentsJson, [FromForm] string algorithm)
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
            Console.WriteLine($"Patient ID: {appt.GetAttribute("PatientId")}, Service: {appt.GetAttribute("Service")}, DateTime: {appt.GetAttribute("Datetime")}");
        }

        if (algorithm == "Preferred")
        {
            AutomaticAppointmentScheduler.SetAlgorithm(new PreferredNurseStrategy());
        }
        else if (algorithm == "Earliest")
        {
            AutomaticAppointmentScheduler.SetAlgorithm(new EarliestsPossibleTimeSlotStrategy());
        }

        // Pass this full appointment list to your scheduler
        AutomaticAppointmentScheduler.AutomaticallyScheduleAppointment(appointments);

        return Ok(new { Message = "Auto appointment scheduling initiated." });
    }




    [HttpPost]
    [Route("AddAppt")]
    public async Task<IActionResult> AddAppt([FromBody] Dictionary<string, JsonElement> requestData)
    {
        string jsonRequestBody = JsonSerializer.Serialize(requestData);

        Console.WriteLine("Received JSON request body: " + jsonRequestBody);

        string appointmentId = requestData["AppointmentId"].GetString() ?? "";
        string patientId = requestData["PatientId"].GetString() ?? "";
        string nurseId = requestData.ContainsKey("NurseId") ? requestData["NurseId"].GetString() ?? "" : "";
        string doctorId = requestData["DoctorId"].GetString() ?? "";
        string Service = requestData["Service"].GetString() ?? "";
        string status = "PENDING"; // hardcoded to always set PENDING as the default status
        DateTime dateTime = requestData["DateTime"].GetDateTime();
        int slot = requestData["Slot"].GetInt32();
        string location = requestData["Location"].GetString() ?? "";

        try
        {
            string createdAppointmentId = await _manualAppointmentScheduler.ScheduleAppointment(
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
}