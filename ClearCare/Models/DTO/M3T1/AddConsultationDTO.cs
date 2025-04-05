using ClearCare.Models.Entities;
using ClearCare.Models.Interfaces.M3T1;

namespace ClearCare.Models.DTO.M3T1;

public class AddConsultationDTO
{
    public AddConsultationDTO(
        List<Appointment> appointments,
        IZoomApi.MeetingResponse? meetingResponse
    )
    {
        Appointments = appointments;
        MeetingResponse = meetingResponse;
    }

    public readonly List<Appointment> Appointments;
    public readonly IZoomApi.MeetingResponse? MeetingResponse;
}