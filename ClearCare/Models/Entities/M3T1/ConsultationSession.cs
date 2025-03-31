using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities.M3T1;

[FirestoreData]
public class ConsultationSession
{
    // Firestore needs a parameterless constructor
    public ConsultationSession()
    {
    }

    public ConsultationSession(string id, DateTime timing, string notes, string zoomLink, string recordingPath,
        bool completed, string appointmentId)
    {
        Id = id;
        Timing = Timestamp.FromDateTime(timing);
        Notes = notes;
        ZoomLink = zoomLink;
        RecordingPath = recordingPath;
        Completed = completed;
        AppointmentId = appointmentId;
    }

    [FirestoreDocumentId] public string Id { get; set; }

    [FirestoreProperty] public Timestamp Timing { get; set; }

    [FirestoreProperty] public string Notes { get; set; }

    [FirestoreProperty] public string ZoomLink { get; set; }

    [FirestoreProperty] public string RecordingPath { get; set; }

    [FirestoreProperty] public bool Completed { get; set; }

    [FirestoreProperty] public string AppointmentId { get; set; }

    public void SetConsultDetails(DateTime timing, string zoomLink, string appointmentId)
    {
        Timing = Timestamp.FromDateTime(timing);
        ZoomLink = zoomLink;
        AppointmentId = appointmentId;
    }

    public void SetResultDetails(string notes, bool completed)
    {
        Notes = notes;
        Completed = completed;
    }
}