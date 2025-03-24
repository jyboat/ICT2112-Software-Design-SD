using ClearCare.Interfaces;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Interface;



using ClearCare.Models.DTO;


namespace ClearCare.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ServiceCompletionInputController : ControllerBase
    {
        private readonly IAppointmentStatus _appointmentStatus;

        // Constructor to inject IAppointmentStatus service
        public ServiceCompletionInputController(IAppointmentStatus appointmentStatus)
        {
            _appointmentStatus = appointmentStatus;
        }

        // [HttpGet("appointments")]
        // public async Task<IActionResult> GetAllAppointments()
        // {
        //     try
        //     {
        //         var appointmentDetails = await _appointmentStatus.getAppointmentDetails();
        //         return Ok(appointmentDetails); // Return all appointments with HTTP status 200
        //     }
        //     catch (System.Exception ex)
        //     {
        //         return StatusCode(500, $"Internal server error: {ex.Message}");
        //     }
        // }

        [HttpGet("appointments")]       
        public async Task<List<ServiceAppointmentDTO>> GetAppointmentDetails()
        {
            // Fetch appointments from the interface (IAppointmentStatus)
            List<ServiceAppointment> appointments = await _appointmentStatus.getAppointmentDetails();

            // Map the fetched ServiceAppointment entities to DTOs
            List<ServiceAppointmentDTO> appointmentDTOs = appointments.Select(appointment => new ServiceAppointmentDTO(appointment)).ToList();

            return appointmentDTOs;
        }

        // [HttpGet("appointments")]
        // public async Task<IActionResult> GetAllAppointments()
        // {
        //     try
        //     {
        //         // Assume that _appointmentStatus.getAppointmentDetails() returns a list of ServiceAppointment objects
        //         var appointments = await _appointmentStatus.getAppointmentDetails();

        //         // Convert to DTO using getter methods
        //         var appointmentDTOs = appointments.Select(appt => new ServiceAppointmentDTO
        //         {
        //             AppointmentId = appt.GetAppointmentID(),
        //             PatientId = appt.GetPatientID(),
        //             NurseId = appt.GetNurseID(),
        //             DoctorId = appt.GetDoctorID(),
        //             ServiceTypeId = appt.GetServiceType(),
        //             Status = appt.GetStatus(),
        //             DateTime = appt.GetDateTime(),
        //             Slot = appt.GetSlot(),
        //             Location = appt.GetLocation()
        //         }).ToList();

        //         return Ok(appointmentDTOs); // Return the DTO list
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, $"Internal server error: {ex.Message}");
        //     }
        // }


        // Endpoint to update the appointment status
        [HttpPut("appointments/{appointmentId}")]
        public async Task<IActionResult> UpdateAppointmentStatus(string appointmentId)
        {
            try
            {
                await _appointmentStatus.updateAppointmentStatus(appointmentId);
                return NoContent(); // Status code 204: Successfully updated, but no content to return
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}


