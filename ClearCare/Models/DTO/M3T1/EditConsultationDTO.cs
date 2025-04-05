using ClearCare.Models.Entities;
using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.DTO.M3T1;

public class EditConsultationDTO
{
    public EditConsultationDTO(List<Appointment> appointments, ConsultationSession session)
    {
        Appointments = appointments;
        Session = session;
    }

    public readonly List<Appointment> Appointments;
    public readonly ConsultationSession Session;
}