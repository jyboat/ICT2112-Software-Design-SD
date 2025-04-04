using ClearCare.Interfaces;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClearCare.Models.Interface;
using ClearCare.Models.DTO;
using ClearCare.Models.Control;
using ClearCare.DataSource;
using System.Text.Json;

namespace ClearCare.Controllers
{
    [Route("ServiceCompletionInput")]
    public class ServiceCompletionInputController : Controller
    {
        private readonly ServiceCompletionManager _appointmentManager;
        private readonly IUserList _UserList;
        // private readonly INotification _notificationManager;

        // Inject IAppointmentStatus and INotification through the constructor
        public ServiceCompletionInputController(ServiceCompletionManager appointmentManager)
        {
            _appointmentManager = appointmentManager;
            _UserList = new AdminManagement(new UserGateway());
            // _notificationManager = notificationManager;
        }

        [HttpGet("appointments")]
        public async Task<IActionResult> getAppointmentDetails(bool json = false)
        {
            // Retrieve userId from the session
            string userId = HttpContext.Session.GetString("UserID");

            Console.WriteLine($"UserId from session: {userId}");  // Log userId from session for debugging

            if (string.IsNullOrEmpty(userId))
            {
                userId = "Not logged in";
                Console.WriteLine("User is not logged in.");
            }

            // Fetch the appointment details
            List<ServiceAppointment> appointments = await _appointmentManager.getAllServiceCompletion();

            // Map appointments to DTOs
            List<appointmentDTO> appointmentDTOs = appointments
                .Select(appointment => new appointmentDTO(appointment))
                .ToList();

            // Filter appointments based on the doctorId from session
            if (!string.IsNullOrEmpty(userId))
            {
                appointmentDTOs = appointmentDTOs
                    .Where(dto => dto.getNurseId == userId) // Only appointments for this doctor
                    .ToList();
            }

            List<User> users = await _UserList.retrieveAllUsers();

            // Fetch user names from IUserList
            foreach (var dto in appointmentDTOs)
            {
                var patient = users.FirstOrDefault(u => u.getProfileData()["UserID"].ToString() == dto.getPatientId);
                var nurse = users.FirstOrDefault(u => u.getProfileData()["UserID"].ToString() == dto.getNurseId);
                var doctor = users.FirstOrDefault(u => u.getProfileData()["UserID"].ToString() == dto.getDoctorId);

                var patientName = patient != null ? patient.getProfileData()["Name"].ToString() ?? "Unknown" : "Unknown";
                var nurseName = nurse != null ? nurse.getProfileData()["Name"].ToString() ?? "Unknown" : "Unknown";
                var doctorName = doctor != null ? doctor.getProfileData()["Name"].ToString() ?? "Unknown" : "Unknown";
                
                dto.setNames(patientName, nurseName, doctorName);
            }

            // Create the message to display in the view or return as part of the JSON
            string message = $"UserId (from session): {userId} | DoctorId: {string.Join(", ", appointmentDTOs.Select(dto => dto.getNurseId).Distinct())}";

            if (json)
            {
                return Json(new { message, appointments = appointmentDTOs });
            }

            ViewBag.Message = message;

            return View("~/Views/M2T5/ServiceCompletion.cshtml", appointmentDTOs);
        }


        [HttpPut("appointments/{appointmentId}")]
        public async Task<IActionResult> editServiceCompletionStatus(string appointmentId, [FromQuery] string patientId, [FromQuery] string nurseId)
        {
            try
            {
                // // Hardcoded integer userId for testing
                // string userId = "USR2";  // Use a valid integer value here

                // Update appointment status
                await _appointmentManager.updateServiceCompletionStatus(appointmentId, patientId, nurseId);

                return NoContent(); // 204: Successfully updated, no content
            }
            catch (System.Exception ex)
            {   
                Console.WriteLine("CompletionInput: EROROREOREOREOR");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("createServiceCompletionHistory")]
        public async Task<IActionResult> createServiceCompletionHistory([FromBody] Dictionary<string, JsonElement> requestData)
        {
            try
            {
                // Console.WriteLine("Received JSON request body: " + JsonSerializer.Serialize(requestData));

                var serviceHistoryId = await _appointmentManager.logServiceCompletion(
                    requestData["appointmentId"].GetString() ?? "",
                    requestData["service"].GetString() ?? "",
                    requestData["patientId"].GetString() ?? "",
                    requestData["nurseId"].GetString() ?? "",
                    requestData["doctorId"].GetString() ?? "",
                    requestData["serviceDate"].GetDateTime(),
                    requestData["location"].GetString() ?? "",
                    requestData["serviceOutcomes"].GetString() ?? ""
                );

                // Console.WriteLine("Step 1: Values extracted:");
                // Console.WriteLine($"  - Appointment ID: {requestData["appointmentId"].GetString()}");
                // Console.WriteLine($"  - Service Type: {requestData["service"].GetString()}");
                // Console.WriteLine($"  - Patient ID: {requestData["patientId"].GetString()}");
                // Console.WriteLine($"  - Nurse ID: {requestData["nurseId"].GetString()}");
                // Console.WriteLine($"  - Doctor ID: {requestData["doctorId"].GetString()}");
                // Console.WriteLine($"  - Service Date: {requestData["serviceDate"].GetDateTime()}");
                // Console.WriteLine($"  - Location: {requestData["location"].GetString()}");
                // Console.WriteLine($"  - Service Outcomes: {requestData["serviceOutcomes"].GetString()}");

                if (!string.IsNullOrEmpty(serviceHistoryId))
                {
                    return Ok(new { Message = "Service History created successfully" });
                }
                else
                {
                    return NotFound(new { Message = "Error Creating Service History" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}\n{ex.StackTrace}"); // Log the error for debugging
                return StatusCode(500, new { Message = "Server error", Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }


    }
}

