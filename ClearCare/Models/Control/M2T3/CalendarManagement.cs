using ClearCare.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClearCare.Models.Entities;
using System.Text.Json;

// TODO: 
// +getAppointmentsByPatient(patientId: Int): <List> ServiceAppointment
// +getAppointmentsByNurse(nurseId: Int): <List> ServiceAppointment
// +getAppointmentsByType(serviceType: String): <List> ServiceAppointment
// +getAppointmentsByDateRange(startDate: DateTime, endDate: DateTime): <List> ServiceAppointment

namespace ClearCare.Models.Control
{
    public class CalendarManagement
    {
        private readonly IRetrieveAllAppointments _retrieveAllAppointments;
        private readonly INurseAvailability _getAvailabilityByStaff;

        public CalendarManagement()
        {
            _retrieveAllAppointments = (IRetrieveAllAppointments) new ServiceAppointmentStatusManagement();
            _getAvailabilityByStaff = (INurseAvailability) new NurseAvailabilityManagement();
        }

        public async Task<JsonResult> getAppointmentsForCalendar(
            string? doctorId,
            string? patientId,
            string? nurseId,
            string? location,
            string? service)
        {
            // Get all appointments from IRetrieveAllAppointments
            var appointments = await _retrieveAllAppointments.getAllServiceAppointments();

            if (appointments == null || !appointments.Any())
            {
                return new JsonResult(new List<object>()); // Return an empty JSON list if no appointments exist
            }

            // Apply filtering within CalendarManagement
            if (!string.IsNullOrEmpty(doctorId))
            {
                appointments = appointments.Where(a => a.GetAttribute("DoctorId") == doctorId).ToList();
            }
            if (!string.IsNullOrEmpty(patientId))
            {
                appointments = appointments.Where(a => a.GetAttribute("PatientId") == patientId).ToList();
            }
            if (!string.IsNullOrEmpty(nurseId))
            {
                appointments = appointments.Where(a => a.GetAttribute("NurseId") == nurseId).ToList();
            }
            if (!string.IsNullOrEmpty(location))
            {
                appointments = appointments.Where(a => a.GetAttribute("Location") == location).ToList();
            }
            if (!string.IsNullOrEmpty(service))
            {
                appointments = appointments.Where(a => a.GetAttribute("Service") == service).ToList();
            }

            // Convert filtered data to JSON format required by FullCalendar
            var eventList = appointments.Select(a => new
            {
                id = a.GetAttribute("AppointmentId"),
                title = a.GetAttribute("Service") + " for " + a.GetAttribute("PatientId"),
                start = DateTime.Parse(a.GetAttribute("Datetime")).ToString("yyyy-MM-ddTHH:mm:ss"),
                extendedProps = new
                {
                    patientId = a.GetAttribute("PatientId"),
                    nurseId = a.GetAttribute("NurseId"),
                    doctorId = a.GetAttribute("DoctorId"),
                    status = a.GetAttribute("Status"),
                    serviceType = a.GetAttribute("Service"),
                    slot = a.GetAttribute("Slot"),
                    location = a.GetAttribute("Location"),
                    dateTime = DateTime.Parse(a.GetAttribute("Datetime")).ToString("yyyy-MM-ddTHH:mm:ss"),
                }
            }).ToList();

            return new JsonResult(eventList);
        }

        public async Task<JsonResult> getAvailabilityByNurseIdForCalendar(string? currentNurseId)
        {
            var nurseAvailabilityList = await _getAvailabilityByStaff.getAvailabilityByStaff(currentNurseId);

            if (nurseAvailabilityList == null || !nurseAvailabilityList.Any())
            {
                return new JsonResult(new List<object>()); // Return an empty list if no availability exists
            }

            // Convert availability data to JSON format required by FullCalendar
            var eventList = nurseAvailabilityList.Select(a =>
            {
                var details = a.getAvailabilityDetails();

                return new
                {
                    id = details["availabilityId"],
                    title = "Available",
                    start = (string)details["date"],
                    extendedProps = new
                    {
                        nurseId = details["nurseID"],
                        startTime = details["startTime"],
                        endTime = details["endTime"]
                    }
                };
            }).ToList();

            return new JsonResult(eventList);
        }

        
       public async Task<object> getSuggestedPatients()
        {
            var result = await _retrieveAllAppointments.suggestPatients();

            var asList = result as IEnumerable<object>;
            if (asList == null)
            {
               
                return new List<object>();
            }
            
            var filtered = asList
            .Where(patient =>
            {
                var patientType = patient.GetType();
                var servicesProp = patientType.GetProperty("Services");
                var services = servicesProp?.GetValue(patient) as IEnumerable<object>;

                if (services == null)
                    return false;

                foreach (var service in services)
                {
                    var status = service?.GetType().GetProperty("Status")?.GetValue(service)?.ToString();
                    if (!string.IsNullOrEmpty(status) && status != "Completed" && status != "Scheduled")
                        return true;
                }

                return false;
            })
            .ToList();     

            // Optional: raw JSON
            // string rawJson = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            // Console.WriteLine($"Raw suggestPatients() result:\n{rawJson}");

             string filteredJson = JsonSerializer.Serialize(filtered, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine($"✅ Filtered Patients:\n{filteredJson}");


            return filtered;
        }






    }
}