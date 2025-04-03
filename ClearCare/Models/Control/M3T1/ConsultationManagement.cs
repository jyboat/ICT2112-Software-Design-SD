using ClearCare.API;
using ClearCare.Models.Entities;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T1;

namespace ClearCare.Models.Control.M3T1;

public class ConsultationManagement : IConsultReceive
{
    private readonly IConsultSend _gateway;

    private readonly ZoomApi _zoomApi;

    public ConsultationManagement(IConsultSend gateway, ZoomApi zoomApi)
    {
        _gateway = gateway;
        _zoomApi = zoomApi;
    }

    public Task uploadRecording(string filePath)
    {
        // TODO
        return Task.CompletedTask;
    }

    public Task<string> insertConsultation(
        Appointment appointment,
        string notes, string zoomLink, string zoomPwd, bool completed
    )
    {
        return _gateway.insertConsultation(appointment.Timing, notes, zoomLink, zoomPwd, appointment.Id);
    }

    public Task<ZoomApi.MeetingResponse?> generateZoomLink(string accessToken)
    {
        return _zoomApi.createMeeting(
            accessToken,
            new ZoomApi.MeetingData
            {
                Agenda = $"Test meeting at {DateTime.Now}",
                Settings = new ZoomApi.MeetingData.SettingsData
                {
                    AutoRecording = ZoomApi.MeetingData.SettingsData.AutoRecordingOption.Cloud
                }
            }
        );
    }

    public string getOAuthZoomRedirectUri(
        string callbackUri
    )
    {
        return _zoomApi.generateAuthorizeLink(callbackUri);
    }

    public Task<ZoomApi.TokenResponse?> generateAccessToken(
        string authCode,
        string redirectUri
        )
    {
        // return _zoomApi.generateAccessToken(authCode, redirectUri);
        // TODO: REMOVE ACCOUNT ID!!!!
        return _zoomApi.generateServerAccessToken("OXUEiX4PQqWwK-YTuAGKNA");
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

    public async Task<Appointment?> getAppointmentById(string appointmentId)
    {
        // Real-world would delegate to the appointment gateway
        var appointments = await getAppointments();
        return appointments.Find(a => a.Id == appointmentId);
    }

    public Task deleteConsultationById(string id)
    {
        return _gateway.deleteConsultationById(id);
    }

    public Task updateConsultationById(
        string id, Appointment appointment, string notes, string zoomLink, string recordingPath, bool isCompleted
    )
    {
        return _gateway.updateConsultationById(id, appointment.Timing, notes, zoomLink, appointment.Id);
    }

    public Task receiveConsultations(List<ConsultationSession> sessions)
    {
        Console.WriteLine($"Consultation list received: {sessions}");
        return Task.CompletedTask;
    }

    public Task receiveConsultation(ConsultationSession session)
    {
        Console.WriteLine($"Consultation received: {session}");
        return Task.CompletedTask;
    }

    public Task receiveAddStatus(bool success)
    {
        Console.WriteLine($"Add status: {success}");
        return Task.CompletedTask;
    }

    public Task receiveUpdateStatus(bool success)
    {
        Console.WriteLine($"Update status: {success}");
        return Task.CompletedTask;
    }

    public Task receiveDeleteStatus(bool success)
    {
        Console.WriteLine($"Delete status: {success}");
        return Task.CompletedTask;
    }

    // Search filter for the ConsultationController
    public List<ConsultationSession> applySearchFilter(List<ConsultationSession> sessions, string query)
    {
        Console.WriteLine($"Received query {query}");
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