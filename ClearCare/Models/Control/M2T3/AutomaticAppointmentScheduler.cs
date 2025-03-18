using System.Threading.Tasks;
using ClearCare.DataSource;
using ClearCare.Models;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Google.Protobuf.WellKnownTypes;
using ClearCare.Interfaces;
using Google.Cloud.Firestore;
using ClearCare.Models.Interface.M2T3;

namespace ClearCare.Models.Control
{
    public class AutomaticAppointmentScheduler : AbstractSchedulingNotifier
    {
        // Interfaces Automatic Requires
        private readonly ICreateAppointment _iCreateAppointment;
        private readonly INurseAvailability _iNurseAvailability;
        private IAutomaticScheduleStrategy? _iAutomaticScheduleStrategy;
        private FirestoreDb db;

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
            db = FirebaseService.Initialize();
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
        }

        public class Patient
        {
            public string PatientId { get; set; }  = string.Empty;
            public string Name { get; set; } = string.Empty;
        }

        public async void AutomaticallyScheduleAppointment()
        {
            if (_iAutomaticScheduleStrategy == null)
            {
                throw new InvalidOperationException("Scheduling strategy has not been set. Use SetAlgorithm() first.");
            }

            Query patientQuery = db.Collection("User").WhereEqualTo("Role", "Patient");
            QuerySnapshot snapshot1 = await patientQuery.GetSnapshotAsync();

            var patients = new List<Patient>();

            foreach(DocumentSnapshot document in snapshot1.Documents)
            {
                patients.Add(new Patient
                {
                    PatientId = document.Id,
                    Name = document.GetValue<string>("Name")
                });
            }

            Query serviceQuery = db.Collection("service_type");
            QuerySnapshot snapshot2 = await serviceQuery.GetSnapshotAsync();

            var services = new List<string>();

            foreach(DocumentSnapshot document in snapshot2.Documents)
            {
                services.Add(document.GetValue<string>("name"));
            }

            var nurses = new List<Nurse>();

            var AvailableNurse = await _iNurseAvailability.getAllStaffAvailability();

            foreach (var nurse in AvailableNurse)
            {
                var availabilityDetails = nurse.getAvailabilityDetails();
                if (availabilityDetails.ContainsKey("nurseID"))
                {
                    nurses.Add(new Nurse
                    {
                        NurseId = availabilityDetails["nurseID"].ToString() ?? " "    
                    });
                }
            }
        
            // Call the auto-assignment function
            var serviceAppointment = _iAutomaticScheduleStrategy.AutomaticallySchedule(nurses, patients, services/*, notify*/);

            foreach (var serviceAppt in serviceAppointment)
            {
                var appointmentId = await _iCreateAppointment.CreateAppointment(
                    serviceAppt.GetAttribute("PatientId"),
                    serviceAppt.GetAttribute("NurseId"),
                    "Hardcode Doctor",
                    serviceAppt.GetAttribute("ServiceTypeId"),
                    "Pending",
                    DateTime.UtcNow,
                    serviceAppt.GetIntAttribute("Slot"),
                    "Physical"
                );

                if (string.IsNullOrEmpty(serviceAppt.GetAttribute("NurseId")))
                {
                    Console.WriteLine($"Failed to schedule Appointment: {appointmentId}");
                    notify(appointmentId, "false");
                }
                else
                {
                    Console.WriteLine($"Successfully scheduled Appointment: {appointmentId}");
                    notify(appointmentId, "success");
                }
            }

        }

        public async Task TestInterface()
        {
           //  await _iCreateAppointment.CreateAppointment();
            var staffAvailability = await _iNurseAvailability.getAllStaffAvailability();
        }   

    }
}