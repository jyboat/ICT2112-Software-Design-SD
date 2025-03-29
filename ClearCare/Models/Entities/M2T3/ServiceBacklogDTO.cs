
namespace ClearCare.Models.Entities
{
    public class ServiceBacklogDTO
    {
        public string BacklogId { get; set; }
        public string AppointmentId { get; set; }
        // public string DateTimeFormatted { get; set; }
        public DateTime DateTime {get; set;}
        public string PatientId {get; set;}
        public string NurseId { get; set; }
        public string DoctorId { get; set; }
        public string ServiceType { get; set; }
    }
}