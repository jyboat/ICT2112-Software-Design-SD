using System.Threading.Tasks;
using ClearCare.DataSource;
using ClearCare.Models;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Google.Protobuf.WellKnownTypes;
using ClearCare.Interfaces;

namespace ClearCare.Models.Control
{
    public class AutomaticAppointmentScheduler
    {
        // Interfaces Automatic Requires
        private readonly ICreateAppointment _iCreateAppointment;
        private readonly INurseAvailability _iNurseAvailability;
        private IAutomaticScheduleStrategy? _iAutomaticScheduleStrategy;

        // // Declare the field at the class level
        // private readonly ServiceAppointmentGateway _serviceAppointmentGateway;

        // Constructor initializes the field
        public AutomaticAppointmentScheduler(
            ICreateAppointment ICreateAppointment, 
            INurseAvailability INurseAvailability,
            IAutomaticScheduleStrategy? IAutomaticScheduleStrategy = null)
        {
            _iCreateAppointment = ICreateAppointment;
            _iNurseAvailability = INurseAvailability;
            // To be set at runtime later
            _iAutomaticScheduleStrategy = IAutomaticScheduleStrategy; 
        }


        public void SetAlgorithm(IAutomaticScheduleStrategy IAutomaticScheduleStrategy)
        {
            _iAutomaticScheduleStrategy = IAutomaticScheduleStrategy; 
        }

        // Dummy Entity for testing
        public class Nurse 
        {
            public string NurseId { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public List<int> AssignedSlots { get; set; } = new List<int>(); 
        }

        public class Patient
        {
            public string PatientId { get; set; }  = string.Empty;
            public string Name { get; set; } = string.Empty;
        }

        public void AutomaticallyScheduleAppointment()
        {
            if (_iAutomaticScheduleStrategy == null)
            {
                throw new InvalidOperationException("Scheduling strategy has not been set. Use SetAlgorithm() first.");
            }

            // Dummy Data :'(
            var nurses = new List<Nurse>
            {
                new Nurse { NurseId = "NURSE001", Name = "Nurse A" },
                new Nurse { NurseId = "NURSE002", Name = "Nurse B" },
                new Nurse { NurseId = "NURSE003", Name = "Nurse C" },
                new Nurse { NurseId = "NURSE004", Name = "Nurse D" }
            };

            var patients = new List<Patient>
            {
                new Patient { PatientId = "PAT001", Name = "Patient 1" },
                new Patient { PatientId = "PAT002", Name = "Patient 2" },
                new Patient { PatientId = "PAT003", Name = "Patient 3" },
                new Patient { PatientId = "PAT004", Name = "Patient 4" },
                new Patient { PatientId = "PAT005", Name = "Patient 5" },
                new Patient { PatientId = "PAT006", Name = "Patient 6" },
                new Patient { PatientId = "PAT007", Name = "Patient 7" },
                new Patient { PatientId = "PAT008", Name = "Patient 8" },
                new Patient { PatientId = "PAT009", Name = "Patient 9" },
                new Patient { PatientId = "PAT010", Name = "Patient 10" },
                new Patient { PatientId = "PAT011", Name = "Patient 11" },
                new Patient { PatientId = "PAT012", Name = "Patient 12" },
                new Patient { PatientId = "PAT013", Name = "Patient 13" },
                new Patient { PatientId = "PAT014", Name = "Patient 14" },
                new Patient { PatientId = "PAT015", Name = "Patient 15" }
            };


            // Call the auto-assignment function
            _iAutomaticScheduleStrategy.AutomaticallySchedule(nurses, patients);
        }

        public async Task TestInterface()
        {
           //  await _iCreateAppointment.CreateAppointment();
            var staffAvailability = await _iNurseAvailability.getAllStaffAvailability();
        }   

    }
}
