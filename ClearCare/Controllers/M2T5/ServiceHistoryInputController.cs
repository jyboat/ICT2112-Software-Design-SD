using Microsoft.AspNetCore.Mvc;
using ClearCare.DataSource;
using ClearCare.Models.Control;
using ClearCare.Interfaces;
using System.Text.Json;

namespace ClearCare.Controllers
{
    [Route("ServiceHistory")] // Set base route for controller
    public class ServiceHistoryInputController : Controller
    {
        private readonly ServiceHistoryManager _ServiceHistoryManager;
        private readonly IAppointmentStatus _ApptStatusService;

        public ServiceHistoryInputController()
        {
            _ApptStatusService = new ServiceAppointmentStatusManagement();

            var _ServiceHistoryMapper = new ServiceHistoryMapper();
            _ServiceHistoryManager = new ServiceHistoryManager(_ServiceHistoryMapper);
        }

        // DISPLAY LIST OF SERVICE HISTORY
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> displayServiceHistory()
        {
            List<Dictionary<string, object>> serviceHistoryList = await _ServiceHistoryManager.getAllServiceHistory();
            return View("~/Views/M2T5/ServiceHistory/Index.cshtml", serviceHistoryList);
        }

        // CREATE SERVICE HISTORY
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> createServiceHistory([FromBody] Dictionary<string, JsonElement> requestData)
        {
            try
            {
                // Console.WriteLine("Received JSON request body: " + JsonSerializer.Serialize(requestData));

                var serviceHistoryId = await _ServiceHistoryManager.createServiceHistory(
                    requestData["appointmentId"].GetString() ?? "",
                    requestData["serviceTypeId"].GetString() ?? "",
                    requestData["patientId"].GetString() ?? "",
                    requestData["nurseId"].GetString() ?? "",
                    requestData["doctorId"].GetString() ?? "",
                    requestData["serviceDate"].GetDateTime(),
                    requestData["location"].GetString() ?? "",
                    requestData["serviceOutcomes"].GetString() ?? ""
                );

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

        // TEMPORARY: NURSE APPTS
        [HttpGet]
        [Route("NurseAppt")]
        public async Task<IActionResult> displayNurseAppt()
        {
            var appointments = await _ApptStatusService.getAppointmentDetails();
            var nurseId = HttpContext.Session.GetString("UserID");
            Console.WriteLine($"Session UserID: {nurseId}");

            var filteredAppts = appointments
                .Where(appt => !string.IsNullOrEmpty(appt.GetAttribute("NurseId")) && appt.GetAttribute("NurseId") == nurseId)
                .ToList();

            if (filteredAppts.Any())
            {
                return View("~/Views/M2T5/ServiceHistory/NurseAppt.cshtml", filteredAppts);
            }
            else
            {
                return NotFound(new { Message = "Appointments not found" });
            }
        }

        // TEMPORARY: UPDATE APPT STATUS
        [HttpPut]
        [Route("Update/{apptId}")]
        public async Task<IActionResult> updateApptStatus(string apptId)
        {
            try
            {
                await _ApptStatusService.updateAppointmentStatus(apptId);
                
                return Ok(new { Success = true, Message = "Appointment updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An error occurred", Error = ex.Message });
            }
        }
    }
}