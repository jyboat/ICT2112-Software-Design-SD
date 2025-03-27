using System;
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
        public string Service { get; set; }
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
            Service = appointment.GetAttribute("Service");
            Status = appointment.GetAttribute("Status");

            // Ensure proper parsing of Datetime
            if (DateTime.TryParse(appointment.GetAttribute("Datetime"), out DateTime parsedDateTime))
            {
                DateTime = parsedDateTime;
            }
            else
            {
                // Handle parsing failure (set to default or log error)
                DateTime = DateTime.MinValue; // Default to MinValue or another fallback
            }

            // Ensure proper parsing of Slot
            if (int.TryParse(appointment.GetAttribute("Slot"), out int parsedSlot))
            {
                Slot = parsedSlot;
            }
            else
            {
                Slot = 0; // Default value if parsing fails
            }

            Location = appointment.GetAttribute("Location");
        }
    }
}