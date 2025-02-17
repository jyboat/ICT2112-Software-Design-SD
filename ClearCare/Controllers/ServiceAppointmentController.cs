using ClearCare.DataSource;
using ClearCare.Models;
using Microsoft.AspNetCore.Mvc;
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

    // POST: Create a new appointment
    // Route: localhost:5007/api/ServiceAppointments/Create that retriggers POST
    // Need add form and button to retrieve data and trigger this
    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> CreateAppointment([FromBody] ServiceAppointment appointment)
    {
        string appointmentId = await _gateway.CreateAppointmentAsync(appointment);
        return Ok(new { Message = "Appointment created successfully", AppointmentId = appointmentId });
    }

    // GET: Retrieve an appointment
    // Route localhost:5007/api/ServiceAppointments/{Id} that retriggers GET
    [HttpGet]
    [Route("{appointmentId}")]
    public async Task<IActionResult> GetAppointment(string appointmentId)
    {
        // Pass the ID to the gateway
        // Appointment is data from firestore that is converted into Model instance
        var appointment = await _gateway.GetAppointmentByIdAsync(appointmentId);

        // No record exists
        if (appointment == null) {
            return NotFound(new { Message = "Appointment not found" });
        }

        // Since Model getter and setter is private use the ToFirestoreDictionary to retrieve
        // Convert object to Dictionary for view
        var appointmentDetail = appointment.ToFirestoreDictionary();

        // Return as json to view in Web (I THINK)
        return View("Index", appointmentDetail);
    }
   
}
