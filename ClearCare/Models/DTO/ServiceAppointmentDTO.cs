using System.Threading.Tasks;
using ClearCare.DataSource;
using ClearCare.Models;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Google.Protobuf.WellKnownTypes;
using ClearCare.Interfaces;
using ClearCare.Models.Interface;

namespace ClearCare.Models.DTO
{
public class ServiceAppointmentDTO
    {
        public string AppointmentId { get; set; }
        public string PatientId { get; set; }
        public string NurseId { get; set; }
        public string DoctorId { get; set; }
        public string ServiceTypeId { get; set; }
        public string Status { get; set; }
        public DateTime DateTime { get; set; }
        public int Slot { get; set; }
        public string Location { get; set; }


        // Constructor that maps data from ServiceAppointment entity
        public ServiceAppointmentDTO(ServiceAppointment appointment)
        {
            AppointmentId = appointment.GetAttribute("AppointmentId");
            PatientId = appointment.GetAttribute("PatientId");
            NurseId = appointment.GetAttribute("NurseId");
            DoctorId = appointment.GetAttribute("DoctorId");
            ServiceTypeId = appointment.GetAttribute("ServiceTypeId");
            Status = appointment.GetAttribute("Status");
            DateTime = DateTime.Parse(appointment.GetAttribute("DateTime"));
            Slot = appointment.GetIntAttribute("Slot");
            Location = appointment.GetAttribute("Location");
        }
    }
}
