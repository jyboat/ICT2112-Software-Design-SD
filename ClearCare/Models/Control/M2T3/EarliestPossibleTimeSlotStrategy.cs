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

            // Concat backlog entries with the unscheduled appointments
            var combinedAppointments = backlogEntries.Concat(unscheduledAppointment).ToList();

            // Group and sort backlog to be scheduled first, if any
            var groupedAppointments = unscheduledAppointment
                .GroupBy(a => a.getAttribute("PatientId"))
                .OrderBy(g => g.Count());

            var groupedBacklog = backlogEntries
                .GroupBy(a => a.getAttribute("PatientId"))
                .OrderBy(g => g.Count());

            var combinedGroups = groupedBacklog.Concat(groupedAppointments);

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
                        foreach (var appt in combinedAppointments)
                        {
                            Console.WriteLine($"Appointment ID: {appt.getAttribute("AppointmentId")}, " +
                                            $"Patient: {appt.getAttribute("PatientId")}, " +
                                            $"Nurse: {appt.getAttribute("NurseId")}, " +
                                            $"Service: {appt.getAttribute("Service")}, " +
                                            $"Slot: {appt.getIntAttribute("Slot")}");
                        }
                        Console.WriteLine("Error: No available slots for patient(s) left");

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
                    nurseIndex = (nurseIndex + 1) % sortedNurses.Count;
                }
            }

            foreach (var appt in combinedAppointments)
            {
                Console.WriteLine($"Appointment ID: {appt.getAttribute("AppointmentId")}, " +
                                $"Patient: {appt.getAttribute("PatientId")}, " +
                                $"Nurse: {appt.getAttribute("NurseId")}, " +
                                $"Service: {appt.getAttribute("Service")}, " +
                                $"Slot: {appt.getIntAttribute("Slot")}");
            }
            return combinedAppointments;
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
    }
}
