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
        // Private fields
        private string _appointmentId;
        private string _patientId;
        private string _nurseId;
        private string _doctorId;
        private string _service;
        private string _status;
        private DateTime _dateTime;
        private int _slot;
        private string _location;

        // Constructor that maps data from ServiceAppointment entity
        public ServiceAppointmentDTO(ServiceAppointment appointment)
        {
            _appointmentId = appointment.GetAttribute("AppointmentId");
            _patientId = appointment.GetAttribute("PatientId");
            _nurseId = appointment.GetAttribute("NurseId");
            _doctorId = appointment.GetAttribute("DoctorId");
            _service = appointment.GetAttribute("Service");
            _status = appointment.GetAttribute("Status");

            // Ensure proper parsing of DateTime
            if (DateTime.TryParse(appointment.GetAttribute("Datetime"), out DateTime parsedDateTime))
            {
                _dateTime = parsedDateTime;
            }
            else
            {
                _dateTime = DateTime.MinValue; // Default to MinValue or another fallback
            }

            // Ensure proper parsing of Slot
            if (int.TryParse(appointment.GetAttribute("Slot"), out int parsedSlot))
            {
                _slot = parsedSlot;
            }
            else
            {
                _slot = 0; // Default value if parsing fails
            }

            _location = appointment.GetAttribute("Location");
        }

        // Public getter methods to access the private fields
        public string AppointmentId => _appointmentId;
        public string PatientId => _patientId;
        public string NurseId => _nurseId;
        public string DoctorId => _doctorId;
        public string Service => _service;
        public string Status => _status;
        public DateTime DateTime => _dateTime;
        public int Slot => _slot;
        public string Location => _location;
    }
}
