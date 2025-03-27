using System.Collections.Generic;
using ClearCare.Models.Entities;
using ClearCare.Models.Control;
using ClearCare.Interfaces;

namespace ClearCare.Control
{
    public class PreferredNurseStrategy : IAutomaticScheduleStrategy
    {
        // Inital insert of service appointment w/o nurse and slot attribute filled
        public List<ServiceAppointment> InitialInsert(List<string> patients, List<string> allServices)
        {
            var services = allServices;
            var appointments = new List<ServiceAppointment>();

            foreach (var patient in patients)
            {
                foreach (var service in services)
                {
                    appointments.Add(ServiceAppointment.setApptDetails(
                        patientId: patient,
                        nurseId: "",
                        doctorId: "",
                        serviceTypeId: service,
                        status: "Pending",
                        dateTime: DateTime.UtcNow,
                        slot: 0,
                        location: "Physical"
                    ));
                }
            }

            return appointments;
        }

        public List<ServiceAppointment> AutomaticallySchedule(
            List<ServiceAppointment> unscheduledAppointment,
            List<AutomaticAppointmentScheduler.Nurse> nurses, 
            List<string> services,
            List<ServiceAppointment> backlogEntries,
            Dictionary<string, List<int>> patientSlotTracker,
            Dictionary<string, Dictionary<int,int>> serviceSlotTracker,
            Dictionary<string, List<int>> nurseSlotTracker)
        {
            int totalSlots = 7;
            int maxPatientsPerSlot = nurses.Count;
            int nurseIndex = 0;

            var sortedNurses = nurses.OrderBy(n =>
            (!nurseSlotTracker.ContainsKey(n.NurseId) || nurseSlotTracker[n.NurseId].Count == 0) ? 0 : 1)
            .ToList();

            // Concat backlog entries with the newly created appointments
            var combinedAppointments = backlogEntries.Concat(unscheduledAppointment).ToList();

            // Group and sort backlog will be scheduled first if any
            var combinedGroups = GetCombinedAppointmentGroups(unscheduledAppointment, backlogEntries);

            // Tracking dictionaries
            // var patientSlotTracker = new Dictionary<string, List<int>>();
            // var serviceSlotTracker = new Dictionary<string, Dictionary<int, int>>();

            foreach (var patientAppointments in combinedGroups)
            {
                // Assign a nurse to this group using round-robin
                var nurse = sortedNurses[nurseIndex];

                foreach (var appointment in patientAppointments)
                {
                    int assignedSlot = GetFirstAvailableSlot(
                        appointment, nurse, patientSlotTracker, serviceSlotTracker, nurseSlotTracker,
                        totalSlots, maxPatientsPerSlot, combinedAppointments);

                    if (assignedSlot == -1)
                    {
                        // Print all appointments for debugging purposes
                        foreach (var appt in combinedAppointments)
                        {
                            Console.WriteLine($"Appointment ID: {appt.GetAttribute("AppointmentId")}, " +
                                            $"Patient: {appt.GetAttribute("PatientId")}, " +
                                            $"Nurse: {appt.GetAttribute("NurseId")}, " +
                                            $"Service: {appt.GetAttribute("ServiceTypeId")}, " +
                                            $"Slot: {appt.GetIntAttribute("Slot")}");
                        }
                        Console.WriteLine("Error: No available slots for patient left");

                        // Also print the trackers
                        PrintTrackers(patientSlotTracker, serviceSlotTracker, nurseSlotTracker);
                        return combinedAppointments;
                    }

                    // Assign nurse and slot
                    appointment.appointNurseToPatient(nurse.NurseId, assignedSlot);

                    string patientId = appointment.GetAttribute("PatientId");
                    string serviceTypeId = appointment.GetAttribute("ServiceTypeId");
                    string nurseId = nurse.NurseId;

                    // Update patient slot tracker
                    if (!patientSlotTracker.ContainsKey(patientId))
                        patientSlotTracker[patientId] = new List<int>();
                    patientSlotTracker[patientId].Add(assignedSlot);

                    // Update nurse slot tracker
                    if (!nurseSlotTracker.ContainsKey(nurseId))
                        nurseSlotTracker[nurseId] = new List<int>();
                    nurseSlotTracker[nurseId].Add(assignedSlot);

                    // Update service slot tracker
                    if (!serviceSlotTracker.ContainsKey(serviceTypeId))
                        serviceSlotTracker[serviceTypeId] = new Dictionary<int, int>();
                    if (!serviceSlotTracker[serviceTypeId].ContainsKey(assignedSlot))
                        serviceSlotTracker[serviceTypeId][assignedSlot] = 0;
                    serviceSlotTracker[serviceTypeId][assignedSlot]++;
                }

                // Rotate nurse assignment using round-robin
                nurseIndex = (nurseIndex + 1) % sortedNurses.Count;
            }

            // Print all appointments
            foreach (var appt in combinedAppointments)
            {
                Console.WriteLine($"Appointment ID: {appt.GetAttribute("AppointmentId")}, " +
                                $"Patient: {appt.GetAttribute("PatientId")}, " +
                                $"Nurse: {appt.GetAttribute("NurseId")}, " +
                                $"Service: {appt.GetAttribute("ServiceTypeId")}, " +
                                $"Slot: {appt.GetIntAttribute("Slot")}");
            }

            // Print tracking dictionaries
            PrintTrackers(patientSlotTracker, serviceSlotTracker, nurseSlotTracker);

            return combinedAppointments;
        }

        // Groups new appointments and backlog appointments, placing backlog infront prioritizing them
        private IEnumerable<IGrouping<string, ServiceAppointment>> GetCombinedAppointmentGroups(
            List<ServiceAppointment> appointments, List<ServiceAppointment> backlogEntries)
        {
            var groupedAppointments = appointments
                .GroupBy(a => a.GetAttribute("PatientId"))
                .OrderBy(g => g.Count());

            var groupedBacklog = backlogEntries
                .GroupBy(a => a.GetAttribute("PatientId"))
                .OrderBy(g => g.Count());

            return groupedBacklog.Concat(groupedAppointments);
        }

        // Finds the first available slot for the given appointment based on current trackers.
        private int GetFirstAvailableSlot(
            ServiceAppointment appointment,
            AutomaticAppointmentScheduler.Nurse nurse,
            Dictionary<string, List<int>> patientSlotTracker,
            Dictionary<string, Dictionary<int, int>> serviceSlotTracker,
            Dictionary<string, List<int>> nurseSlotTracker,
            int totalSlots,
            int maxPatientsPerSlot,
            List<ServiceAppointment> combinedAppointments)
        {
            int assignedSlot = 1;
            string patientId = appointment.GetAttribute("PatientId");
            string serviceTypeId = appointment.GetAttribute("ServiceTypeId");
            string nurseId = nurse.NurseId;

            // Ensure tracking dictionaries are initialized
            if (!patientSlotTracker.ContainsKey(patientId))
                patientSlotTracker[patientId] = new List<int>();
            if (!serviceSlotTracker.ContainsKey(serviceTypeId))
                serviceSlotTracker[serviceTypeId] = new Dictionary<int, int>();
            if (!nurseSlotTracker.ContainsKey(nurseId))
                nurseSlotTracker[nurseId] = new List<int>();

            while (true)
            {
                bool slotOccupied = patientSlotTracker[patientId].Contains(assignedSlot) ||
                                    (serviceSlotTracker[serviceTypeId].ContainsKey(assignedSlot) &&
                                    serviceSlotTracker[serviceTypeId][assignedSlot] >= maxPatientsPerSlot) ||
                                    nurseSlotTracker[nurseId].Contains(assignedSlot);

                if (!slotOccupied)
                    return assignedSlot;

                assignedSlot++;

                if (assignedSlot > totalSlots)
                    return -1; // No available slot found
            }
        }

        private void PrintTrackers(
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
                Console.WriteLine($"ServiceTypeId: {service.Key}");
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