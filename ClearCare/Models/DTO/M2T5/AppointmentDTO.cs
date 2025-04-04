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
    public class appointmentDTO
    {
        // Private fields
        private string AppointmentId;
        private string PatientId;
        private string NurseId;
        private string DoctorId;
        private string Service;
        private string Status;
        private DateTime DateTime;
        private int Slot;
        private string Location;

        // New properties for names
        private string PatientName;
        private string NurseName;
        private string DoctorName;

        // Constructor that maps data from ServiceAppointment entity
        public appointmentDTO(ServiceAppointment appointment)
        {
            AppointmentId = appointment.getAttribute("AppointmentId");
            PatientId = appointment.getAttribute("PatientId");
            NurseId = appointment.getAttribute("NurseId");
            DoctorId = appointment.getAttribute("DoctorId");
            Service = appointment.getAttribute("Service");
            Status = appointment.getAttribute("Status");

            // Ensure proper parsing of DateTime
            if (DateTime.TryParse(appointment.getAttribute("Datetime"), out DateTime parsedDateTime))
            {
                DateTime = parsedDateTime;
            }
            else
            {
                DateTime = DateTime.MinValue; // Default to MinValue or another fallback
            }

            // Ensure proper parsing of Slot
            if (int.TryParse(appointment.getAttribute("Slot"), out int parsedSlot))
            {
                Slot = parsedSlot;
            }
            else
            {
                Slot = 0; // Default value if parsing fails
            }

            Location = appointment.getAttribute("Location");
        }

        // Public getter methods to access the private fields
        public string getAppointmentId => AppointmentId;
        public string getPatientId => PatientId;
        public string getNurseId => NurseId;
        public string getDoctorId => DoctorId;
        public string getService => Service;
        public string getStatus => Status;
        public DateTime getDateTime => DateTime;
        public int getSlot => Slot;
        public string getLocation => Location;

        public string getPatientName
        {
            get => PatientName;
            private set => PatientName = value;
        }

        public string getNurseName
        {
            get => NurseName;
            private set => NurseName = value;
        }

        public string getDoctorName
        {
            get => DoctorName;
            private set => DoctorName = value;
        }

        // Public method to set all names at once
        public void setNames(string patientName, string nurseName, string doctorName)
        {
            PatientName = patientName;
            NurseName = nurseName;
            DoctorName = doctorName;
        }

    }
}
