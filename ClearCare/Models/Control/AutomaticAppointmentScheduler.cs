using System.Threading.Tasks;
using ClearCare.DataSource;
using ClearCare.Models;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Google.Protobuf.WellKnownTypes;

namespace ClearCare.Models.Control
{
    public class AutomaticAppointmentScheduler
    {
        // Declare the field at the class level
        private readonly ServiceAppointmentGateway _serviceAppointmentGateway;

        // Constructor initializes the field
        public AutomaticAppointmentScheduler()
        {
            _serviceAppointmentGateway = new ServiceAppointmentGateway();
        }

        // Dummy model for testing
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

        void AssignNursesAndSlots(List<Nurse> nurses, List<ServiceAppointment> appointments)
        {
            int totalSlots = 14;
            int nurseIndex = 0; 

            // Sort patients by the number of services they need (those with fewer services go first)
            var groupedAppointments = appointments.GroupBy(a => a.GetAttribute("PatientId")).OrderBy(g => g.Count()).ToList();

            // Keep track of used slots per patient
            var patientSlotTracker = new Dictionary<string, List<int>>();

            // Keep track of used slots for each service type to prevent overlapping
            var serviceSlotTracker = new Dictionary<string, List<int>>();

            foreach (var patientAppointments in groupedAppointments)
            {
                // Assign a nurse to this patient
                var nurse = nurses[nurseIndex]; 

                // Make sure patient is assigned to the same nurse
                foreach (var appointment in patientAppointments)
                {
                    // Start from slot 1
                    int assignedSlot = 1; 

                    // Check that other patient dont have same service in the same slot
                    while ((patientSlotTracker.ContainsKey(appointment.GetAttribute("PatientId")) && patientSlotTracker[appointment.GetAttribute("PatientId")].Contains(assignedSlot)) ||
                        (serviceSlotTracker.ContainsKey(appointment.GetAttribute("ServiceTypeId")) && serviceSlotTracker[appointment.GetAttribute("ServiceTypeId")].Contains(assignedSlot)))
                    {
                        assignedSlot++;

                        // If all slots are used, reset to first available slot
                        if (assignedSlot > totalSlots)
                            assignedSlot = 1;
                    }

                    // Assign the nurse and slot
                    appointment.appointNurseToPatient(nurse.NurseId, assignedSlot);
                    // appointment.NurseId = nurse.NurseId;
                    // appointment.Slot = assignedSlot;

                    // Track patient slot usage
                    if (!patientSlotTracker.ContainsKey(appointment.GetAttribute("PatientId")))
                    {
                        patientSlotTracker[appointment.GetAttribute("PatientId")] = new List<int>();
                    }
                    patientSlotTracker[appointment.GetAttribute("PatientId")].Add(assignedSlot);

                    // Track service slot usage
                    if (!serviceSlotTracker.ContainsKey(appointment.GetAttribute("ServiceTypeId")))
                    {
                        serviceSlotTracker[appointment.GetAttribute("ServiceTypeId")] = new List<int>();
                    }
                    serviceSlotTracker[appointment.GetAttribute("ServiceTypeId")].Add(assignedSlot);
                }

                // Move to the next nurse in round-robin
                // Once last nurse assigned to everyone it will reset to the first one
                nurseIndex = (nurseIndex + 1) % nurses.Count;

                // Check if each service all the slot assigned
                bool fullyAssigned = serviceSlotTracker.All(entry => entry.Value.Distinct().Count() == 14);

                if (fullyAssigned)
                {
                    Console.WriteLine("All services have been fully assigned. STOP!!!!!!!!!!!");
                    return; 
                }

            }
        }

        public void TestAutoAssignment()
        {
            var nurses = new List<Nurse>
            {
                new Nurse { NurseId = "NURSE001", Name = "Nurse A" },
                new Nurse { NurseId = "NURSE002", Name = "Nurse B" },
                new Nurse { NurseId = "NURSE003", Name = "Nurse C" },
                new Nurse { NurseId = "NURSE004", Name = "Nurse D" },
                new Nurse { NurseId = "NURSE005", Name = "Nurse E" }
            };

            var patients = new List<Patient>
            {
                new Patient { PatientId = "PAT001", Name = "Patient 1" },
                new Patient { PatientId = "PAT002", Name = "Patient 2" },
                new Patient { PatientId = "PAT003", Name = "Patient 3" },
                new Patient { PatientId = "PAT004", Name = "Patient 4" },
                new Patient { PatientId = "PAT005", Name = "Patient 5" }
            };

            var services = new List<string> { "Home Safety Assessment", "Medication", "Counselling", "Caregiver Training" };
            var appointments = new List<ServiceAppointment>();

            foreach (var patient in patients)
            {
                foreach (var service in services)
                {
                    appointments.Add(ServiceAppointment.setApptDetails(
                        // Create unique id
                        appointmentId: Guid.NewGuid().ToString(), 
                        patientId: patient.PatientId, 
                        nurseId: "", 
                        doctorId: "",
                        serviceTypeId: service, 
                        status: "Pending", 
                        dateTime: DateTime.Now, 
                        slot: 0,
                        location: "Physical" 
                    ));
                }
            }

            // Call the auto-assignment function
            AssignNursesAndSlots(nurses, appointments);
        }



    }
}
