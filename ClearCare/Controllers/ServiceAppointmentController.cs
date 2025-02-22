using ClearCare.DataSource;
using ClearCare.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;

// Request Handling
[Route("api/[controller]")]
[ApiController]
public class ServiceAppointmentsController : Controller
{
    private readonly ServiceAppointmentGateway _gateway;

    public ServiceAppointmentsController()
    {
        _gateway = new ServiceAppointmentGateway();
    }

    // GET All appointment
    [HttpGet]
    public async Task<IActionResult> RetrieveAll()
    {
        var appointment = await _gateway.GetAllAppointmentsAsync();

        // No record exists
        if (appointment == null)
        {
            return NotFound(new { Message = "Appointment not found" });
        }

        // Since Model getter and setter is private use the ToFirestoreDictionary to retrieve
        // Convert object to Dictionary for view
        var appointmentList = appointment.Select(a => a.ToFirestoreDictionary()).ToList();

        // Return as json to view in Web (I THINK)
        return View("Index", appointmentList);
    }

    [HttpGet]
    [Route("CreatePage")]
    public IActionResult Create()
    {
        return View("CreateServiceAppt"); // Render the form
    }

    // POST: Create a new appointment
    // Route: localhost:5007/api/ServiceAppointments/Create that retriggers POST
    // Need add form and button to retrieve data and trigger this
    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> CreateAppointment([FromBody] Dictionary<string, JsonElement> requestData)
    {
        // Map JSON data to model
        var appointment = ServiceAppointment.setApptDetails(
            requestData["AppointmentId"].GetString() ?? "",
            requestData["PatientId"].GetString() ?? "",
            requestData.ContainsKey("NurseId") ? requestData["NurseId"].GetString() ?? "" : "",
            requestData["DoctorId"].GetString() ?? "",
            requestData["ServiceTypeId"].GetString() ?? "",
            requestData["Status"].GetString() ?? "",
            requestData["DateTime"].GetDateTime(),
            requestData["Slot"].GetInt32(),
            requestData["Location"].GetString() ?? ""
        );

        string appointmentId = await _gateway.CreateAppointmentAsync(appointment);
        return Ok(new { Message = "Appointment created successfully", AppointmentId = appointmentId });
    }


    // GET: Retrieve an appointment
    // Route localhost:5007/api/ServiceAppointments/Retrieve/{Id} that retriggers GET
    [HttpGet]
    [Route("Retrieve/{appointmentId}")]
    public async Task<IActionResult> GetAppointment(string appointmentId)
    {
        // Pass the ID to the gateway
        // Appointment is data from firestore that is converted into Model instance
        var appointment = await _gateway.GetAppointmentByIdAsync(appointmentId);

        // No record exists
        if (appointment == null)
        {
            return NotFound(new { Message = "Appointment not found" });
        }

        // Since Model getter and setter is private use the ToFirestoreDictionary to retrieve
        // Convert object to Dictionary for view
        var appointmentDetail = appointment.ToFirestoreDictionary();

        // Return as json to view in Web (I THINK)
        return View("AppointmentDetails", appointmentDetail);
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

    [HttpGet]
    [Route("Calendar")]
    public IActionResult Calendar()
    {
        return View("Calendar");
    }

    [HttpGet]
    [Route("GetAppointmentsForCalendar")]
    public async Task<IActionResult> GetAppointmentsForCalendar()
    {
        var appointments = await _gateway.GetAllAppointmentsAsync();

        if (appointments == null || !appointments.Any())
        {
            return Json(new List<object>());  // Return an empty list if no appointments exist
        }

        var eventList = appointments.Select(a => new
        {
            id = a.ToFirestoreDictionary()["AppointmentId"],
            title = "Appointment with " + a.ToFirestoreDictionary()["DoctorId"],
            start = ((DateTime)a.ToFirestoreDictionary()["DateTime"]).ToString("yyyy-MM-ddTHH:mm:ss"),
            extendedProps = new
            {
                patientId = a.ToFirestoreDictionary()["PatientId"],
                doctorId = a.ToFirestoreDictionary()["DoctorId"],
                status = a.ToFirestoreDictionary()["Status"],
                location = a.ToFirestoreDictionary()["Location"]
            }
        });

        return Json(eventList);
    }



}
