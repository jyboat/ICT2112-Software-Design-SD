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

// Request Handling
[Route("api/[controller]")]
[ApiController]
public class ServiceAppointmentsController : Controller
{
    private readonly ServiceAppointmentManagement ServiceAppointmentManagement;
    private readonly AutomaticAppointmentScheduler AutomaticAppointmentScheduler;
    private readonly CalendarManagement _calendarManagement;
    private readonly NurseAvailabilityManagement _nurseAvailabilityManagement;
    private readonly ManualAppointmentScheduler _manualAppointmentScheduler;


    public ServiceAppointmentsController()
    {

        ServiceAppointmentManagement = new ServiceAppointmentManagement();


        _nurseAvailabilityManagement = new NurseAvailabilityManagement();
        // var nurse_availability_gateway = new NurseAvailabilityGateway();
        // // Create the manager and pass the gateway
        // _nurseAvailabilityManagement = new NurseAvailabilityManagement(nurse_availability_gateway);
        // // Set the gateway's receiver to the manager (which implements IAvailabilityDB_Receive)
        // nurse_availability_gateway.Receiver = _nurseAvailabilityManagement;




        _calendarManagement = new CalendarManagement(ServiceAppointmentManagement, _nurseAvailabilityManagement);

        AutomaticAppointmentScheduler = new AutomaticAppointmentScheduler((ICreateAppointment)ServiceAppointmentManagement, (INurseAvailability)_nurseAvailabilityManagement);
        _manualAppointmentScheduler = new ManualAppointmentScheduler((ICreateAppointment)ServiceAppointmentManagement, (INurseAvailability)_nurseAvailabilityManagement);

    }

    // GET All appointment
    [HttpGet]
    public async Task<IActionResult> RetrieveAllAppointment()
    {
        // await to wait for task complete or data to retrieve before executing
        var appointment = await ServiceAppointmentManagement.retrieveAllAppointments();
        // No record exists
        if (appointment != null && appointment.Any())
        {
            return View("~/Views/M2T3/ServiceAppointments/Index.cshtml", appointment);
        }
        else
        {
            return NotFound(new { Message = "Appointment not found" });
        }
    }

    [HttpGet]
    [Route("GetAppointmentsForCalendar")]
    public async Task<JsonResult> getAppointmentsForCalendar([FromQuery] string? doctorId,
        [FromQuery] string? patientId, [FromQuery] string? nurseId)
    {
        return await _calendarManagement.getAppointmentsForCalendar(doctorId, patientId, nurseId);
    }


    [HttpGet]
    [Route("Calendar")]
    public IActionResult Calendar()
    {
        ViewBag.Patients = ServiceAppointmentManagement.GetAllPatients();
        ViewBag.Nurses = ServiceAppointmentManagement.GetAllNurses();
        ViewBag.ServiceTypes = ServiceAppointmentManagement.GetAllServiceTypes();
        ViewBag.DoctorId = "DOC001"; // hardcoded for now, will retrieve from session later

        return View("~/Views/M2T3/ServiceAppointments/Calendar.cshtml");
    }

    // Implement IRetrieveAll
    public async Task<List<Dictionary<string, object>>> RetrieveAll()
    {
        return await ServiceAppointmentManagement.retrieveAllAppointments();
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
        ViewBag.Patients = await ServiceAppointmentManagement.getUnscheduledPatients();
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
    [Route("Retrieve/{documentId}")]
    public async Task<IActionResult> GetAppointment(string documentId)
    {
        var appointmentDetail = await ServiceAppointmentManagement.getAppointmentByID(documentId);

        if (appointmentDetail != null && appointmentDetail.Any())
        {
            return View("~/Views/M2T3/ServiceAppointments/AppointmentDetails.cshtml", appointmentDetail);
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
            Console.WriteLine("Received JSON request body: " + JsonSerializer.Serialize(requestData));

            var result = await ServiceAppointmentManagement.UpdateAppointment(
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

    // Test Manual's Interface
    [HttpGet]
    [Route("TestManualAppointment")]
    public async Task<IActionResult> TestManualAppointment()
    {
        await _manualAppointmentScheduler.TestInterface();
        return View("~/Views/M2T3/ServiceAppointments/TestManualAppointment.cshtml"); // Render the View
    }

    [HttpPost]
    [Route("TestManualAppointment")]
    public async Task<IActionResult> RunTestManualAppointment()
    {
        await _manualAppointmentScheduler.TestInterface();
        return RedirectToAction("~/Views/M2T3/ServiceAppointments/TestManualAppointment.cshtml");
    }

    // Test Auto Interface
    [HttpPost]
    [Route("TestAutoAppointment")]
    public IActionResult TestAutoAppointment([FromForm] List<string> PatientIds, [FromForm] string algorithm)
    {
        Console.WriteLine("Selected Patient IDs: " + string.Join(", ", PatientIds));
        Console.WriteLine("Selected Algorithm: " + algorithm);
        
        // Use the selected algorithm to set the scheduling strategy
        if (algorithm == "Preferred")
        {
            AutomaticAppointmentScheduler.SetAlgorithm(new PreferredNurseStrategy());
            // Pass the list of patient IDs to the scheduling algorithm.
            AutomaticAppointmentScheduler.AutomaticallyScheduleAppointment(PatientIds);
            return Ok(new { Message = "Auto appointment scheduling Initiated."});
        }
        else if (algorithm == "Earliest")
        {
            AutomaticAppointmentScheduler.SetAlgorithm(new EarliestsPossibleTimeSlotStrategy());
            // Pass the list of patient IDs to the scheduling algorithm.
            AutomaticAppointmentScheduler.AutomaticallyScheduleAppointment(PatientIds);
            return Ok(new { Message = "Auto appointment scheduling Initiated."});
        }   
        
        return Ok(new { Message = "Auto appointment scheduling Completed."});
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
        string serviceTypeId = requestData["ServiceTypeId"].GetString() ?? "";
        string status = "PENDING"; // hardcoded to always set PENDING as the default status
        DateTime dateTime = requestData["DateTime"].GetDateTime();
        int slot = requestData["Slot"].GetInt32();
        string location = requestData["Location"].GetString() ?? "";

        try
        {
            string createdAppointmentId = await _manualAppointmentScheduler.ScheduleAppointment(
                patientId, nurseId, doctorId, serviceTypeId, status, dateTime, slot, location
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