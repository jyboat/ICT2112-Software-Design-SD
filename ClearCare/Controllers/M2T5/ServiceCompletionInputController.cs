using ClearCare.Interfaces;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClearCare.Models.Interface;
using ClearCare.Models.DTO;
using ClearCare.Models.Control;

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
            string message = $"UserId (from session): {userId} | DoctorId: {string.Join(", ", appointmentDTOs.Select(dto => dto.DoctorId).Distinct())}";

            if (json)
            {
                return Json(new { message, appointments = appointmentDTOs });
            }

            ViewBag.Message = message;

            return View("~/Views/M2T5/ServiceCompletion.cshtml", appointmentDTOs);
        }

        // [HttpPut("appointments/{appointmentId}")]
        // public async Task<IActionResult> UpdateAppointmentStatus(string appointmentId)
        // {
        //     try
        //     {
        //         // Retrieve userId from the session for notification
        //         string userId = HttpContext.Session.GetString("UserID");

        //         if (string.IsNullOrEmpty(userId))
        //         {
        //             Console.WriteLine("User not logged in, cannot update appointment status.");
        //             return Unauthorized("User not logged in.");
        //         }

        //         Console.WriteLine($"Updating appointment status for Appointment ID: {appointmentId}");

        //         // Update the appointment status
        //         await _appointmentStatus.updateAppointmentStatus(appointmentId);

        //         // Create a notification after the status is updated
        //         string notificationContent = $"The status of your appointment (ID: {appointmentId}) has been updated.";

        //         Console.WriteLine($"Creating notification for userId: {userId} with content: {notificationContent}");

        //         await _notificationManager.createNotification(int.Parse(userId), notificationContent); // Create the notification using INotification

        //         // Log success
        //         Console.WriteLine("Notification successfully created.");

        //         return NoContent(); // 204: Successfully updated, no content
        //     }
        //     catch (System.Exception ex)
        //     {
        //         Console.WriteLine($"Error occurred: {ex.Message}"); // Log the error message
        //         return StatusCode(500, $"Internal server error: {ex.Message}");
        //     }
        // }

        [HttpPut("appointments/{appointmentId}")]
        public async Task<IActionResult> UpdateAppointmentStatus(string appointmentId)
        {
            try
            {
                // Hardcoded integer userId for testing
                string userId = "USR2";  // Use a valid integer value here

                // Update appointment status
                await _appointmentStatus.UpdateAppointmentStatus(appointmentId);

                // // Create a notification after the status is updated
                // string notificationContent = $"The status of your appointment (ID: {appointmentId}) has been updated.";
                // await _notificationManager.createNotification(userId, notificationContent); // Create the notification using INotification
                // TempData["NotificationMessage"] = "Notification sent to the user successfully.";

                return NoContent(); // 204: Successfully updated, no content
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}