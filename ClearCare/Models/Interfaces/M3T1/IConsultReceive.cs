using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Interfaces.M3T1;

public interface IConsultReceive
{
    public Task receiveConsultations(List<ConsultationSession> sessions);

    public Task receiveConsultation(ConsultationSession session);
    
    public Task receiveAddStatus(bool success);

    public Task receiveUpdateStatus(bool success);

    public Task receiveDeleteStatus(bool success);
}