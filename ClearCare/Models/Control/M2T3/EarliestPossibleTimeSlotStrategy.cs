using System.Collections.Generic;
using ClearCare.Models.Entities;
using ClearCare.Models.Control;
using ClearCare.Interfaces;

namespace ClearCare.Control
{
    public class EarliestsPossibleTimeSlotStrategy : IAutomaticScheduleStrategy
    {
        public List<ServiceAppointment> automaticallySchedule(
            List<ServiceAppointment> unscheduledAppointment,
            List<string> nurses, 
            List<string> services,
            List<ServiceAppointment> backlogEntries,
            Dictionary<string, List<int>> patientSlotTracker,
            Dictionary<string, Dictionary<int,int>> serviceSlotTracker,
            Dictionary<string, List<int>> nurseSlotTracker)
        {
            int totalSlots = 8;
            int maxPatientsPerSlot = nurses.Count;
            int nurseIndex = 0;

            var sortedNurses = nurses.OrderBy(n =>
                nurseSlotTracker.ContainsKey(n) ? nurseSlotTracker[n].Count : 0
            ).ToList();

            // Concat backlog entries with the newly created appointments
            var combinedAppointments = backlogEntries.Concat(unscheduledAppointment).ToList();

            // Group and sort backlog will be scheduled first if any
            var combinedGroups = getCombinedAppointmentGroups(unscheduledAppointment, backlogEntries);

            foreach (var patientAppointments in combinedGroups)
            {
                foreach (var appointment in patientAppointments)
                {
                    // Assign a nurse to this group using round-robin
                    var nurse = sortedNurses[nurseIndex];

                    int assignedSlot = getFirstAvailableSlot(
                        appointment, nurse, patientSlotTracker, serviceSlotTracker, nurseSlotTracker,
                        totalSlots, maxPatientsPerSlot, combinedAppointments);

                    if (assignedSlot == -1)
                    {
                        // Print all appointments for debugging purposes
                        foreach (var appt in combinedAppointments)
                        {
                            Console.WriteLine($"Appointment ID: {appt.getAttribute("AppointmentId")}, " +
                                            $"Patient: {appt.getAttribute("PatientId")}, " +
                                            $"Nurse: {appt.getAttribute("NurseId")}, " +
                                            $"Service: {appt.getAttribute("Service")}, " +
                                            $"Slot: {appt.getIntAttribute("Slot")}");
                        }
                        Console.WriteLine("Error: No available slots for patient left");

                        // Also print the trackers
                        printTrackers(patientSlotTracker, serviceSlotTracker, nurseSlotTracker);
                        return combinedAppointments;
                    }

                    // Assign nurse and slot
                    appointment.appointNurseToPatient(nurse, assignedSlot);

                    string patientId = appointment.getAttribute("PatientId");
                    string Service = appointment.getAttribute("Service");
                    string nurseId = nurse;

                    // Update patient slot tracker
                    if (!patientSlotTracker.ContainsKey(patientId))
                        patientSlotTracker[patientId] = new List<int>();
                    patientSlotTracker[patientId].Add(assignedSlot);

                    // Update nurse slot tracker
                    if (!nurseSlotTracker.ContainsKey(nurseId))
                        nurseSlotTracker[nurseId] = new List<int>();
                    nurseSlotTracker[nurseId].Add(assignedSlot);

                    // Update service slot tracker
                    if (!serviceSlotTracker.ContainsKey(Service))
                        serviceSlotTracker[Service] = new Dictionary<int, int>();
                    if (!serviceSlotTracker[Service].ContainsKey(assignedSlot))
                        serviceSlotTracker[Service][assignedSlot] = 0;
                    serviceSlotTracker[Service][assignedSlot]++;
                    
                    // Rotate nurse index for round-robin scheduling
                    // Inside the for loop as theres no need for nurse to be tied to patient
                    // Might have a issue
                    nurseIndex = (nurseIndex + 1) % sortedNurses.Count;
                }
            }

            // Print all appointments
            foreach (var appt in combinedAppointments)
            {
                Console.WriteLine($"Appointment ID: {appt.getAttribute("AppointmentId")}, " +
                                $"Patient: {appt.getAttribute("PatientId")}, " +
                                $"Nurse: {appt.getAttribute("NurseId")}, " +
                                $"Service: {appt.getAttribute("Service")}, " +
                                $"Slot: {appt.getIntAttribute("Slot")}");
            }

            // Print tracking dictionaries
            printTrackers(patientSlotTracker, serviceSlotTracker, nurseSlotTracker);

            return combinedAppointments;
        }

        // Groups new appointments and backlog appointments, placing backlog infront prioritizing them
        private IEnumerable<IGrouping<string, ServiceAppointment>> getCombinedAppointmentGroups(
            List<ServiceAppointment> appointments, List<ServiceAppointment> backlogEntries)
        {
            var groupedAppointments = appointments
                .GroupBy(a => a.getAttribute("PatientId"))
                .OrderBy(g => g.Count());

            var groupedBacklog = backlogEntries
                .GroupBy(a => a.getAttribute("PatientId"))
                .OrderBy(g => g.Count());

            return groupedBacklog.Concat(groupedAppointments);
        }

        // Finds the first available slot for the given appointment based on current trackers.
        private int getFirstAvailableSlot(
            ServiceAppointment appointment,
            string nurse,
            Dictionary<string, List<int>> patientSlotTracker,
            Dictionary<string, Dictionary<int, int>> serviceSlotTracker,
            Dictionary<string, List<int>> nurseSlotTracker,
            int totalSlots,
            int maxPatientsPerSlot,
            List<ServiceAppointment> combinedAppointments)
        {
            int assignedSlot = 1;
            string patientId = appointment.getAttribute("PatientId");
            string Service = appointment.getAttribute("Service");
            string nurseId = nurse;

            // Ensure tracking dictionaries are initialized
            if (!patientSlotTracker.ContainsKey(patientId))
                patientSlotTracker[patientId] = new List<int>();
            if (!serviceSlotTracker.ContainsKey(Service))
                serviceSlotTracker[Service] = new Dictionary<int, int>();
            if (!nurseSlotTracker.ContainsKey(nurseId))
                nurseSlotTracker[nurseId] = new List<int>();

            while (true)
            {
                bool slotOccupied = patientSlotTracker[patientId].Contains(assignedSlot) ||
                                    (serviceSlotTracker[Service].ContainsKey(assignedSlot) &&
                                    serviceSlotTracker[Service][assignedSlot] >= maxPatientsPerSlot) ||
                                    nurseSlotTracker[nurseId].Contains(assignedSlot);

                if (!slotOccupied)
                    return assignedSlot;

                assignedSlot++;

                if (assignedSlot > totalSlots)
                    return -1; // No available slot found
            }
        }

        private void printTrackers(
            Dictionary<string, List<int>> patientSlotTracker,
            Dictionary<string, Dictionary<int, int>> serviceSlotTracker,
            Dictionary<string, List<int>> nurseSlotTracker)
        {
            Console.WriteLine("Patient Slot Tracker:");
            foreach (var kvp in patientSlotTracker)
            {
                Console.WriteLine($"PatientId: {kvp.Key} -> Slots: {string.Join(", ", kvp.Value)}");
            }

            Console.WriteLine("Service Slot Tracker:");
            foreach (var service in serviceSlotTracker)
            {
                Console.WriteLine($"Service: {service.Key}");
                foreach (var slot in service.Value)
                {
                    Console.WriteLine($"  Slot: {slot.Key} -> Count: {slot.Value}");
                }
            }

            Console.WriteLine("Nurse Slot Tracker:");
            foreach (var kvp in nurseSlotTracker)
            {
                Console.WriteLine($"NurseId: {kvp.Key} -> Slots: {string.Join(", ", kvp.Value)}");
            }
        }
    }
}