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
}