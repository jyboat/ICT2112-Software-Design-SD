using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Interfaces.M3T1;

public interface IConsultSend
{
    public Task<string> insertConsultation(DateTime timing, string notes, string zoomLink, string zoomPwd,
        string appointmentId);

    public Task<bool> updateConsultationById(string id, DateTime time, string notes, string zoomLink, string appointmentId);

    public Task deleteConsultationById(string id);

    public Task<ConsultationSession?> fetchConsultationById(string id);

    public Task<List<ConsultationSession>> fetchConsultations();
}