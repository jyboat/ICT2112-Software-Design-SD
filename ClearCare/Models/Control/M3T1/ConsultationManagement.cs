using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T1;

namespace ClearCare.Models.Control.M3T1;

public class ConsultationManagement : IConsultReceive
{
    private readonly IConsultSend _gateway;

    public ConsultationManagement(IConsultSend gateway)
    {
        _gateway = gateway;
    }

    public void uploadRecording(string filePath)
    {
    }

    public void completeSession(string notes, bool completed)
    {
    }

    public Task<List<ConsultationSession>> getConsultations()
    {
        return _gateway.fetchConsultations();
    }

    public Task<ConsultationSession?> getConsultationById(string id)
    {
        return _gateway.fetchConsultationById(id);
    }

    // This is mainly for displaying a list of appointments in the creation page
    public Task<List<Appointment>> getAppointments()
    {
        // Return a hard-coded list for now
        var appointments = new List<Appointment>
        {
            new Appointment("A1", "Doctor's Visitation - Follow-Up", DateTime.Now),
            new Appointment("A2", "Monthly Healthcare Check-In", new DateTime(2025, 6, 1, 12, 30, 0)),
            new Appointment("A3", "Medicine Check", new DateTime(2025, 7, 20, 14, 0, 0)),
            new Appointment("A4", "Scheduled Doctor's Appointment", new DateTime(2025, 8, 20, 14, 0, 0)),
            new Appointment("A5", "Welfare Check", new DateTime(2025, 9, 5, 11, 40, 0)),
        };

        return Task.FromResult(appointments);
    }

    public Task receiveConsultations(List<ConsultationSession> sessions)
    {
        throw new NotImplementedException();
    }

    public Task receiveConsultation(ConsultationSession session)
    {
        throw new NotImplementedException();
    }

    public Task receiveAddStatus(bool success)
    {
        throw new NotImplementedException();
    }

    public Task receiveUpdateStatus(bool success)
    {
        throw new NotImplementedException();
    }

    public Task receiveDeleteStatus(bool success)
    {
        throw new NotImplementedException();
    }

    // Search filter for the ConsultationController
    public List<ConsultationSession> applySearchFilter(List<ConsultationSession> sessions, string query)
    {
        if (!string.IsNullOrWhiteSpace(query))
        {
            return sessions.Where(s =>
                s.Notes.Contains(query, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        return sessions;
    }

    // Pagination for the ConsultationController
    public List<ConsultationSession> applyPagination(List<ConsultationSession> sessions, int page, int pageSize)
    {
        var paginatedList = sessions
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return paginatedList;
    }
}