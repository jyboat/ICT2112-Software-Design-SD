using ClearCare.Interfaces;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Interface;



using ClearCare.Models.DTO;


using ClearCare.Interfaces;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            List<ServiceAppointment> appointments = await _appointmentStatus.getAppointmentDetails();
            List<ServiceAppointmentDTO> appointmentDTOs = appointments
                .Select(appointment => new ServiceAppointmentDTO(appointment))
                .ToList();

            if (json)
            {
                return Json(appointmentDTOs); // Returns JSON if requested
            }

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



