using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClearCare.Models.Interface;
using ClearCare.DataSource;
using ClearCare.Models.Entities;

namespace ClearCare.Models.Control
{
    public class AuditManagement : IAuditSubject, IAuditLog

    {
        private readonly AuditGateway _auditGateway;
        private static List<IAuditObserver> _auditObservers = new List<IAuditObserver>(); // ✅ Observer list

        public AuditManagement()
        {
            _auditGateway = new AuditGateway();
        }

        // ✅ Add an observer (Ensures no duplicates)
        public void AddObserver(IAuditObserver observer)
        {
            if (!_auditObservers.Any(o => o.GetType() == observer.GetType()))
            {
                _auditObservers.Add(observer);
                Console.WriteLine($"Observer {observer.GetType().Name} added.");
            }
        }

        // ✅ Remove an observer
        public void RemoveObserver(IAuditObserver observer)
        {
            _auditObservers.Remove(observer);
        }

        // ✅ Notify observers about a new audit log
        public async Task NotifyObservers()
        {
            var updatedAudit = await _auditGateway.RetrieveAllAuditLogs();
            foreach (var observer in _auditObservers)
            {
                Console.WriteLine("🔔 Notifying observers about new audit log...");
                observer.OnAuditLogInserted(updatedAudit); // ✅ Pass the new log to observers
            }
        }

        // ✅ Insert audit log & notify observers
        public async Task<string> InsertAuditLog(string action, string performedBy)
        {
            AuditLog newLog = await _auditGateway.InsertAuditLog(action, performedBy);

            if (newLog != null)
            {
                NotifyObservers(); // ✅ Notify observers about the new log
                return newLog.AuditLogID; // ✅ Return new log ID
            }

            return "Error: Could not log action"; // ✅ Ensure return value if log insertion fails
        }

        // ✅ Fetch all audit logs
        public async Task<List<AuditLog>> GetAllAuditLogsAsync()
        {
            return await _auditGateway.RetrieveAllAuditLogs();
        }

        // ✅ Get specific audit log by ID
        public async Task<AuditLog> GetAuditLogAsync(string auditLogId)
        {
            return await _auditGateway.RetrieveAuditLogById(auditLogId);
        }
    }
}
