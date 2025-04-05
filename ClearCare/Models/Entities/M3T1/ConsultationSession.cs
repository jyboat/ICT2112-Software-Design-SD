using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities.M3T1;

[FirestoreData]
public class ConsultationSession
{
    // Firestore needs a parameterless constructor
    public ConsultationSession()
    {
    }

    public ConsultationSession(string id, DateTime timing, string notes, string zoomLink,
        bool completed, string appointmentId)
    {
        Id = id;
        Timing = Timestamp.FromDateTime(timing);
        Notes = notes;
        ZoomLink = zoomLink;
        Completed = completed;
        AppointmentId = appointmentId;
    }

    [FirestoreDocumentId] private string Id { get; set; }

    private string getId() => Id;
    private void setId(string id) => Id = id;

    [FirestoreProperty] private Timestamp Timing { get; set; }

    private Timestamp getTiming() => Timing;
    private void setTiming(Timestamp timing) => Timing = timing;

    [FirestoreProperty] private string Notes { get; set; }

    private string getNotes() => Notes;
    private void setNotes(string notes) => Notes = notes;
    
    [FirestoreProperty] private string ZoomLink { get; set; }
    
    private string getZoomLink() => ZoomLink;
    private void setZoomLink(string zoomLink) => ZoomLink = zoomLink;

    [FirestoreProperty] private string ZoomPwd { get; set; }
    
    private string getZoomPwd() => ZoomPwd;
    private void setZoomPwd(string zoomPwd) => ZoomPwd = zoomPwd;

    [FirestoreProperty] private bool Completed { get; set; }

    private bool getCompleted() => Completed;
    private void setCompleted(bool isCompleted) => Completed = isCompleted;

    [FirestoreProperty] private string AppointmentId { get; set; }

    private string getAppointmentId() => AppointmentId;
    private void setAppointmentId(string appointmentId) => AppointmentId = appointmentId;

    public void setConsultDetails(DateTime timing, string zoomLink, string appointmentId)
    {
        Timing = Timestamp.FromDateTime(timing);
        ZoomLink = zoomLink;
        AppointmentId = appointmentId;
    }

    public Dictionary<string, object> getConsultDetails()
    {
        return new Dictionary<string, object>
        {
            { "Id", Id },
            { "Timing", getTiming().ToDateTime() },
            { "ZoomLink", getZoomLink() },
            { "ZoomPwd", getZoomPwd() },
            { "AppointmentId", getAppointmentId() }
        };
    }
    
    public void setResultDetails(string notes, bool completed)
    {
        Notes = notes;
        Completed = completed;
    }

    public Dictionary<string, object> getResultDetails()
    {
        return new Dictionary<string, object>
        {
            { "Notes", getNotes() },
            { "Completed", getCompleted() }
        };
    }
}