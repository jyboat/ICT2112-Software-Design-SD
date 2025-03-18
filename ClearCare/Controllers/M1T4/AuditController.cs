using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.Models.Control;

namespace ClearCare.Controllers
{
    [ApiController]
    [Route("api/audit")]
    public class AuditController : ControllerBase
    {
        private readonly AuditManagement _auditManagement;

        public AuditController()
        {
            _auditManagement = new AuditManagement();
        }

        // Endpoint to log an audit entry
        [HttpPost("log")]
        public async Task<IActionResult> LogAction([FromBody] AuditLog auditLog)
        {
            if (auditLog == null || string.IsNullOrEmpty(auditLog.Action) || string.IsNullOrEmpty(auditLog.PerformedBy))
            {
                return BadRequest(new { Message = "Invalid audit log data. Action and PerformedBy are required." });
            }

            string auditLogId = await _auditManagement.InsertAuditLog(auditLog.Action, auditLog.PerformedBy);

            return Ok(new { Message = "Audit log created successfully", AuditLogID = auditLogId });
        }

        // Endpoint to retrieve all audit logs
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAuditLogs()
        {
            List<AuditLog> logs = await _auditManagement.GetAllAuditLogsAsync();
            return Ok(logs);
        }

        // Endpoint to retrieve a specific audit log by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuditLog(string id)
        {
            AuditLog log = await _auditManagement.GetAuditLogAsync(id);
            if (log == null)
            {
                return NotFound(new { Message = "Audit log not found" });
            }
            return Ok(log);
        }
    }
}
