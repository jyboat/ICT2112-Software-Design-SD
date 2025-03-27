using ClearCare.Interfaces;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClearCare.Models.Interface;
using ClearCare.Models.DTO;
using ClearCare.Models.Control;
using System.Text.Json;

namespace ClearCare.Controllers
{
    [Route("ServiceCompletionInput")]
    public class ServiceCompletionInputController : Controller
    {
        private readonly ServiceCompletionManager _appointmentStatus;
        // private readonly INotification _notificationManager;

        // Inject IAppointmentStatus and INotification through the constructor
        public ServiceCompletionInputController(ServiceCompletionManager appointmentStatus)
        {
            _appointmentStatus = appointmentStatus;
            // _notificationManager = notificationManager;
        }

        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointmentDetails(bool json = false)
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
            List<ServiceAppointment> appointments = await _appointmentStatus.GetAllAppointmentDetails();

            // Map appointments to DTOs
            List<ServiceAppointmentDTO> appointmentDTOs = appointments
                .Select(appointment => new ServiceAppointmentDTO(appointment))
                .ToList();

            // Filter appointments based on the doctorId from session
            if (!string.IsNullOrEmpty(userId))
            {
                appointmentDTOs = appointmentDTOs
                    .Where(dto => dto.NurseId == userId) // Only appointments for this doctor
                    .ToList();
            }

            // Create the message to display in the view or return as part of the JSON
            string message = $"UserId (from session): {userId} | DoctorId: {string.Join(", ", appointmentDTOs.Select(dto => dto.NurseId).Distinct())}";

            if (json)
            {
                return Json(new { message, appointments = appointmentDTOs });
            }

            ViewBag.Message = message;

            return View("~/Views/M2T5/ServiceCompletion.cshtml", appointmentDTOs);
        }


        [HttpPut("appointments/{appointmentId}")]
        public async Task<IActionResult> UpdateAppointmentStatus(string appointmentId)
        {
            try
            {
                // Hardcoded integer userId for testing
                string userId = "USR2";  // Use a valid integer value here

                // Update appointment status
                await _appointmentStatus.UpdateAppointmentStatus(appointmentId);

                return NoContent(); // 204: Successfully updated, no content
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("createServiceHistory")]
        public async Task<IActionResult> createServiceHistory([FromBody] Dictionary<string, JsonElement> requestData)
        {
            try
            {
                // Console.WriteLine("Received JSON request body: " + JsonSerializer.Serialize(requestData));

                var serviceHistoryId = await _appointmentStatus.CreateServiceHistory(
                    requestData["appointmentId"].GetString() ?? "",
                    requestData["serviceTypeId"].GetString() ?? "",
                    requestData["patientId"].GetString() ?? "",
                    requestData["nurseId"].GetString() ?? "",
                    requestData["doctorId"].GetString() ?? "",
                    requestData["serviceDate"].GetDateTime(),
                    requestData["location"].GetString() ?? "",
                    requestData["serviceOutcomes"].GetString() ?? ""
                );

        Console.WriteLine("Step 1: Values extracted:");
        Console.WriteLine($"  - Appointment ID: {requestData["appointmentId"].GetString()}");
        Console.WriteLine($"  - Service Type ID: {requestData["serviceTypeId"].GetString()}");
        Console.WriteLine($"  - Patient ID: {requestData["patientId"].GetString()}");
        Console.WriteLine($"  - Nurse ID: {requestData["nurseId"].GetString()}");
        Console.WriteLine($"  - Doctor ID: {requestData["doctorId"].GetString()}");
        Console.WriteLine($"  - Service Date: {requestData["serviceDate"].GetDateTime()}");
        Console.WriteLine($"  - Location: {requestData["location"].GetString()}");
        Console.WriteLine($"  - Service Outcomes: {requestData["serviceOutcomes"].GetString()}");

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

