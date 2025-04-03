using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T1;
using Google.Cloud.Firestore;

namespace ClearCare.DataSource.M3T1;

public class ConsultationGateway : IConsultSend
{
    private readonly FirestoreDb _db;
    public IConsultReceive? receiver;

    private readonly CollectionReference _collectionRef;

    public ConsultationGateway()
    {
        _db = FirebaseService.Initialize();
        _collectionRef = _db.Collection("Consultations");
    }

    public async Task<string> insertConsultation(DateTime timing, string notes, string zoomLink, string appointmentId)
    {
        DocumentReference docRef = _collectionRef.Document();

        var consultation = new Dictionary<string, object>
        {
            { "Timing", Timestamp.FromDateTime(timing.ToUniversalTime()) },
            { "Notes", notes },
            { "ZoomLink", zoomLink },
            { "AppointmentId", appointmentId }
        };
        await docRef.SetAsync(consultation);
        if (receiver != null) await receiver.receiveAddStatus(true);

        return docRef.Id;
    }

    public async Task<bool> updateConsultationById(string id, DateTime time, string notes, string zoomLink,
        string appointmentId)
    {
        DocumentReference docRef = _collectionRef.Document(id);

        var updatedData = new Dictionary<string, object>
        {
            { "Timing", time },
            { "Notes", notes },
            { "ZoomLink", zoomLink },
            { "AppointmentId", appointmentId }
        };

        // Check if document exists before updating
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (!snapshot.Exists)
        {
            return false; // Document not found
        }

        await docRef.UpdateAsync(updatedData);
        if (receiver != null) await receiver.receiveUpdateStatus(true);

        return true;
    }

    public async Task<List<ConsultationSession>> fetchConsultations()
    {
        List<ConsultationSession> sessions = new List<ConsultationSession>();

        QuerySnapshot snapshot = await _collectionRef.GetSnapshotAsync();

        foreach (DocumentSnapshot doc in snapshot.Documents)
        {
            if (!doc.Exists) continue;
            try
            {
                var session = doc.ConvertTo<ConsultationSession>();
                sessions.Add(session);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting session {doc.Id}: {ex.Message}: {ex}");
            }
        }

        if (receiver != null) await receiver.receiveConsultations(sessions);

        return sessions;
    }

    public Task deleteConsultationById(string id)
    {
        DocumentReference docRef = _collectionRef.Document(id);

        return docRef.DeleteAsync();
    }

    public async Task<ConsultationSession?> fetchConsultationById(string id)
    {
        DocumentReference docRef = _collectionRef.Document(id);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (!snapshot.Exists)
        {
            Console.WriteLine($"No consultation found {snapshot.Id}");
            return null;
        }

        ConsultationSession session = snapshot.ConvertTo<ConsultationSession>();
        if (receiver != null) await receiver.receiveConsultation(session);

        return session;
    }

    public async Task<bool> delete(string id)
    {
        DocumentReference docRef = _collectionRef.Document(id);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (!snapshot.Exists)
        {
            Console.WriteLine($"No consultation found {snapshot.Id}");
            return false;
        }

        await docRef.DeleteAsync();
        if (receiver != null) await receiver.receiveDeleteStatus(true);

        return true;
    }
}