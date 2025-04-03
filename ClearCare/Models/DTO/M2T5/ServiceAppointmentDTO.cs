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

        // New properties for names
        private string _patientName;
        private string _nurseName;
        private string _doctorName;

        // Constructor that maps data from ServiceAppointment entity
        public ServiceAppointmentDTO(ServiceAppointment appointment)
        {
            _appointmentId = appointment.getAttribute("AppointmentId");
            _patientId = appointment.getAttribute("PatientId");
            _nurseId = appointment.getAttribute("NurseId");
            _doctorId = appointment.getAttribute("DoctorId");
            _service = appointment.getAttribute("Service");
            _status = appointment.getAttribute("Status");

            // Ensure proper parsing of DateTime
            if (DateTime.TryParse(appointment.getAttribute("Datetime"), out DateTime parsedDateTime))
            {
                _dateTime = parsedDateTime;
            }
            else
            {
                _dateTime = DateTime.MinValue; // Default to MinValue or another fallback
            }

            // Ensure proper parsing of Slot
            if (int.TryParse(appointment.getAttribute("Slot"), out int parsedSlot))
            {
                _slot = parsedSlot;
            }
            else
            {
                _slot = 0; // Default value if parsing fails
            }

            _location = appointment.getAttribute("Location");
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

        public string PatientName
        {
            get => _patientName;
            private set => _patientName = value;
        }

        public string NurseName
        {
            get => _nurseName;
            private set => _nurseName = value;
        }

        public string DoctorName
        {
            get => _doctorName;
            private set => _doctorName = value;
        }

        // Public method to set all names at once
        public void SetNames(string patientName, string nurseName, string doctorName)
        {
            PatientName = patientName;
            NurseName = nurseName;
            DoctorName = doctorName;
        }

    }
}
