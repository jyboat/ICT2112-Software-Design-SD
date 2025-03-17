using System.Collections.Generic;
using ClearCare.Models.Entities;
using ClearCare.Models.Control;
using ClearCare.Interfaces;


namespace ClearCare.Control
{
    public class PreferredNurseStrategy: IAutomaticScheduleStrategy
    {

        // Inital insert of service appointment w/o nurse and slot attribute filled
        public List<ServiceAppointment> InitialInsert(List<AutomaticAppointmentScheduler.Patient> patients, List<string> allServices){
            var services = allServices;
            var appointments = new List<ServiceAppointment>();

            // Need add backlog entries into the starting entries of appointments 

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

            return appointments;
        }

        public void AutomaticallySchedule(
            List<AutomaticAppointmentScheduler.Nurse> nurses, 
            List<AutomaticAppointmentScheduler.Patient> patients,
            List<string> services)
        {
            var appointments = InitialInsert(patients, services);
            int totalSlots = 14;
            int maxPatientsPerSlot = 3;
            int nurseIndex = 0;

            // Sort patients by the number of services they need (fewest services first)
            var groupedAppointments = appointments.GroupBy(a => a.GetAttribute("PatientId"))
                                                .OrderBy(g => g.Count())
                                                .ToList();

            // Track slots used per patient
            var patientSlotTracker = new Dictionary<string, List<int>>();

            // Track used slots for each service to prevent overlaps
            var serviceSlotTracker = new Dictionary<string, Dictionary<int, int>>();

            // Track nurse availability per slot
            var nurseSlotTracker = new Dictionary<string, List<int>>();

            foreach (var patientAppointments in groupedAppointments)
            {
                // Assign a nurse to this patient
                var nurse = nurses[nurseIndex];

                foreach (var appointment in patientAppointments)
                {
                    // Start from slot 1
                    int assignedSlot = 1; 

                    string patientId = appointment.GetAttribute("PatientId");
                    string serviceTypeId = appointment.GetAttribute("ServiceTypeId");
                    string nurseId = nurse.NurseId;

                    // Ensure tracking dictionaries are initialized
                    if (!serviceSlotTracker.ContainsKey(serviceTypeId))
                        serviceSlotTracker[serviceTypeId] = new Dictionary<int, int>();

                    if (!patientSlotTracker.ContainsKey(patientId))
                        patientSlotTracker[patientId] = new List<int>();

                    if (!nurseSlotTracker.ContainsKey(nurseId))
                        nurseSlotTracker[nurseId] = new List<int>();

                    // Find the first available slot that meets all the conditions
                    while (true)
                    {
                        // Check if slot is available
                        // Patient doesnt have conflicting slot
                        // Check if slot exist in tracker 
                        // If it doesn't exist = not occupied can use
                        // Else if it exist need check whether it has 3 records (3 patients for each timeslot for each service)
                        // Nurse doesnt have conflicting slot
                        bool slotOccupied = patientSlotTracker[patientId].Contains(assignedSlot) ||
                                            (serviceSlotTracker[serviceTypeId].ContainsKey(assignedSlot) &&
                                            serviceSlotTracker[serviceTypeId][assignedSlot] >= maxPatientsPerSlot) ||
                                            nurseSlotTracker[nurseId].Contains(assignedSlot);

                        if (!slotOccupied)
                            // Still got slot
                            break;  

                        assignedSlot++;

                        // No more slot, print and stop the algorithm
                        if (assignedSlot > totalSlots)
                        {
                            // // Print results for debugging
                            foreach (var appt in appointments)
                            {
                                Console.WriteLine($"Appointment ID: {appt.GetAttribute("AppointmentId")}, " +
                                                $"Patient: {appt.GetAttribute("PatientId")}, " +
                                                $"Nurse: {appt.GetAttribute("NurseId")}, " +
                                                $"Service: {appt.GetAttribute("ServiceTypeId")}, " +
                                                $"Slot: {appt.GetIntAttribute("Slot")}");
                            }
                            // Probably here that needs observer
                            Console.WriteLine($"Error: No available slots for patient left");
                        
                            return; 
                        }
                    }

                    // Assign nurse and slot
                    appointment.appointNurseToPatient(nurse.NurseId, assignedSlot);

                    // Track patient's used slots
                    patientSlotTracker[patientId].Add(assignedSlot);

                    // Track nurse's assigned slots
                    nurseSlotTracker[nurseId].Add(assignedSlot);

                    // Track the number of patients assigned to this slot for the service
                    if (!serviceSlotTracker[serviceTypeId].ContainsKey(assignedSlot))
                        serviceSlotTracker[serviceTypeId][assignedSlot] = 0;

                    serviceSlotTracker[serviceTypeId][assignedSlot]++;
                }

                // Move to the next nurse (RR)
                nurseIndex = (nurseIndex + 1) % nurses.Count;
            }

            foreach (var appt in appointments)
            {
                Console.WriteLine($"Appointment ID: {appt.GetAttribute("AppointmentId")}, " +
                                $"Patient: {appt.GetAttribute("PatientId")}, " +
                                $"Nurse: {appt.GetAttribute("NurseId")}, " +
                                $"Service: {appt.GetAttribute("ServiceTypeId")}, " +
                                $"Slot: {appt.GetIntAttribute("Slot")}");
            }
        }
    }
}