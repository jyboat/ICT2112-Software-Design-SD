using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.DataSource;  // Correct namespace for AuditGateway
using ClearCare.Models.Entities;

namespace ClearCare.Business
{
    public class AuditManagement
    {
        private readonly AuditGateway _auditGateway;

        public AuditManagement()
        {
            _auditGateway = new AuditGateway();
        }

        // Logs an action
        public async Task<string> LogActionAsync(string action, string performedBy)
        {
            AuditLog newLog = await _auditGateway.InsertAuditLog(action, performedBy);
            return newLog?.AuditLogID ?? "Error: Could not log action";
        }

        // Fetch all audit logs
        public async Task<List<AuditLog>> GetAllAuditLogsAsync()
        {
            return await _auditGateway.RetrieveAllAuditLogs();
        }

        // Get specific audit log by ID
        public async Task<AuditLog> GetAuditLogAsync(string auditLogId)
        {
            return await _auditGateway.RetrieveAuditLogById(auditLogId);
        }
    }
}
