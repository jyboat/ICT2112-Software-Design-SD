using ClearCare.Interfaces;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClearCare.Models.Interface;
using ClearCare.Models.DTO;

namespace ClearCare.Controllers
{
    [Route("ServiceCompletionInput")] // Removed "api/" to make it an MVC controller
    public class ServiceCompletionInputController : Controller // Changed from ControllerBase to Controller
    {
        private readonly IAppointmentStatus _appointmentStatus;

        public ServiceCompletionInputController(IAppointmentStatus appointmentStatus)
        {
            _appointmentStatus = appointmentStatus;
        }

        // Returns an HTML page or JSON based on the 'json' query parameter
        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointmentDetails(bool json = false)
        {
            // Retrieve userId from the session
            string userId = HttpContext.Session.GetString("UserID");

            // Log or print the session value for debugging
            Console.WriteLine($"UserId from session: {userId}");  // For debugging

            if (string.IsNullOrEmpty(userId))
            {
                userId = "Not logged in"; // This will let you know if the session is empty
            }

            // Fetch the appointment details
            List<ServiceAppointment> appointments = await _appointmentStatus.getAppointmentDetails();

            // Map appointments to DTOs
            List<ServiceAppointmentDTO> appointmentDTOs = appointments
                .Select(appointment => new ServiceAppointmentDTO(appointment))
                .ToList();

            // Filter the appointments to only show those that match the doctorId from session
            if (!string.IsNullOrEmpty(userId))
            {
                appointmentDTOs = appointmentDTOs
                    .Where(dto => dto.DoctorId == userId) // Only appointments for this doctor
                    .ToList();
            }

            // Create the message to display in the view or return as part of the JSON
            string message = $"UserId (from session): {userId} | DoctorId: {string.Join(", ", appointmentDTOs.Select(dto => dto.DoctorId).Distinct())}";

            if (json)
            {
                return Json(new { message, appointments = appointmentDTOs }); // Return JSON with message and filtered appointments
            }

            // Pass the message to the view
            ViewBag.Message = message;

            // Explicitly specify the custom path for the View
            return View("~/Views/M2T5/ServiceCompletion.cshtml", appointmentDTOs);
        }

        // Endpoint to update the appointment status
        [HttpPut("appointments/{appointmentId}")]
        public async Task<IActionResult> UpdateAppointmentStatus(string appointmentId)
        {
            try
            {
                await _appointmentStatus.updateAppointmentStatus(appointmentId);
                return NoContent(); // 204: Successfully updated, no content
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
