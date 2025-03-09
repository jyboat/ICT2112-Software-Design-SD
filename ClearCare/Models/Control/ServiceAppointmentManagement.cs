using System.Threading.Tasks;
using ClearCare.DataSource;
using ClearCare.Models;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Google.Protobuf.WellKnownTypes;

namespace ClearCare.Models.Control
{
    public class ServiceAppointmentManagement : IRetrieveAll
    {
        // Declare the field at the class level
        private readonly ServiceAppointmentGateway _serviceAppointmentGateway;

        // Constructor initializes the field
        public ServiceAppointmentManagement()
        {
            _serviceAppointmentGateway = new ServiceAppointmentGateway();
        }

        public async Task<List<Dictionary<string, object>>> RetrieveAll()
        {
            var appointment = await _serviceAppointmentGateway.GetAllAppointmentsAsync();
            var appointmentList = appointment.Select(a => a.ToFirestoreDictionary()).ToList();

            if (appointmentList != null)
            {
                return appointmentList;
            }
            else
            {
                // Return Empty List
                return new List<Dictionary<string, object>>();
            }
        }

        // i hardcode the "retrieval" of nurse and patirents first, later once get from mod 1, will update
        public List<Dictionary<string, string>> GetAllPatients()
        {
            return new List<Dictionary<string, string>>
            {
                new Dictionary<string, string> {{"id", "1"}, {"name", "John Doe"}},
                new Dictionary<string, string> {{"id", "2"}, {"name", "Jane Doe"}},
            };
        }

        public List<Dictionary<string, string>> GetAllNurses()
        {
            return new List<Dictionary<string, string>>
            {
                new Dictionary<string, string> {{"id", "1"}, {"name", "Mike Tyson"}},
                new Dictionary<string, string> {{"id", "2"}, {"name", "Rocky Balboa"}},
            };
        }

        public async Task<string> CreateAppt(string appointmentId, string patientId, string nurseId,
            string doctorId, string serviceTypeId, string status, DateTime dateTime, int slot, string location)
        {
            // Map JSON data to model
            var appointment = ServiceAppointment.setApptDetails(
                appointmentId, patientId, nurseId, doctorId, serviceTypeId, status, dateTime, slot, location
            );

            string appointmentID = await _serviceAppointmentGateway.CreateAppointmentAsync(appointment);

            if (appointmentID != "")
            {
                return appointmentID;
            }
            else
            {
                return "";
            }
        }

        public async Task<Dictionary<string, object>> GetAppt(string appointmentId)
        {
            // Pass the ID to the gateway
            // Appointment is data from firestore that is converted into Model instance
            var appointment = await _serviceAppointmentGateway.GetAppointmentByIdAsync(appointmentId);

            if (appointment != null)
            {
                return appointment.ToFirestoreDictionary();
            }
            else
            {
                // Return Empty List
                return new Dictionary<string, object>();
            }
        }

        public async Task<List<object>> GetAppointmentsForCalendar(string? doctorId, string? patientId, string? nurseId)
        {
            var appointments = await _serviceAppointmentGateway.GetAllAppointmentsAsync();

            if (appointments == null || !appointments.Any())
            {
                return new List<object>(); // Return an empty list if no appointments exist
            }

            // Apply filtering
            if (!string.IsNullOrEmpty(doctorId))
            {
                appointments = appointments.Where(a => a.ToFirestoreDictionary()["DoctorId"].ToString() == doctorId).ToList();
            }
            if (!string.IsNullOrEmpty(patientId))
            {
                appointments = appointments.Where(a => a.ToFirestoreDictionary()["PatientId"].ToString() == patientId).ToList();
            }
            if (!string.IsNullOrEmpty(nurseId))
            {
                appointments = appointments.Where(a => a.ToFirestoreDictionary()["NurseId"].ToString() == nurseId).ToList();
            }

            var eventList = appointments.Select(a => new
            {
                id = a.ToFirestoreDictionary()["AppointmentId"],
                title = "Appointment with " + a.ToFirestoreDictionary()["DoctorId"],
                start = ((DateTime)a.ToFirestoreDictionary()["DateTime"]).ToString("yyyy-MM-ddTHH:mm:ss"),
                extendedProps = new
                {
                    patientId = a.ToFirestoreDictionary()["PatientId"],
                    nurseId = a.ToFirestoreDictionary()["NurseId"],
                    doctorId = a.ToFirestoreDictionary()["DoctorId"],
                    status = a.ToFirestoreDictionary()["Status"],
                    location = a.ToFirestoreDictionary()["Location"]
                }
            }).Cast<Object>().ToList();

            return eventList;
        }

    }
}
