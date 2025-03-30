using Microsoft.AspNetCore.Mvc;
using ClearCare.DataSource;
using ClearCare.Models.Control;
using ClearCare.Interfaces;
using System.Text.Json;

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
    [Route("ServiceHistory")] // Set base route for controller
    public class ServiceHistoryInputController : Controller
    {
        private readonly ServiceHistoryManager _ServiceHistoryManager;
        private readonly IUserList _UserList;

        public ServiceHistoryInputController()
        {
            _UserList = new AdminManagement(new UserGateway());

            var _ServiceHistoryMapper = new ServiceHistoryMapper();
            _ServiceHistoryManager = new ServiceHistoryManager(_ServiceHistoryMapper, _UserList);
        }

        // DISPLAY LIST OF SERVICE HISTORY
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> displayServiceHistory()
        {
            // Get logged-in user role and ID from session
            string userRole = HttpContext.Session.GetString("Role") ?? "";
            string userId = HttpContext.Session.GetString("UserID") ?? "";

            List<Dictionary<string, object>> serviceHistoryList = await _ServiceHistoryManager.getAllServiceHistory(userRole, userId);
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
                    requestData["service"].GetString() ?? "",
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
    }
}