using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClearCare.Models.Control
{
    public class CalendarManagement
    {
        private readonly IRetrieveAllAppointmentsRaw _retrieveAllAppointmentRaw;

        public CalendarManagement(IRetrieveAllAppointmentsRaw retrieveAllAppointmentRaw)
        {
            _retrieveAllAppointmentRaw = retrieveAllAppointmentRaw;
        }

        public async Task<List<object>> GetAppointmentsForCalendar(string? doctorId, string? patientId, string? nurseId)
        {
            // Get all appointments from IRetrieveAllAppointmentsRaw (implemented by ServiceAppointmentController)
            var appointments = await _retrieveAllAppointmentRaw.RetrieveAllAppointmentsRaw();

            if (appointments == null || !appointments.Any())
            {
                return new List<object>(); // Return an empty list if no appointments exist
            }

            // Apply filtering within CalendarManagement
            if (!string.IsNullOrEmpty(doctorId))
            {
                appointments = appointments.Where(a => a["DoctorId"].ToString() == doctorId).ToList();
            }
            if (!string.IsNullOrEmpty(patientId))
            {
                appointments = appointments.Where(a => a["PatientId"].ToString() == patientId).ToList();
            }
            if (!string.IsNullOrEmpty(nurseId))
            {
                appointments = appointments.Where(a => a.ContainsKey("NurseId") && a["NurseId"].ToString() == nurseId).ToList();
            }

            // Convert filtered data to JSON format required by FullCalendar
            var eventList = appointments.Select(a => new
            {
                id = a["AppointmentId"],
                title = "Appointment with " + a["DoctorId"],
                start = ((DateTime)a["DateTime"]).ToString("yyyy-MM-ddTHH:mm:ss"),
                extendedProps = new
                {
                    patientId = a["PatientId"],
                    nurseId = a.ContainsKey("NurseId") ? a["NurseId"] : null,
                    doctorId = a["DoctorId"],
                    status = a["Status"],
                    location = a["Location"]
                }
            }).Cast<object>().ToList();

            return eventList;
        }
    }
}
