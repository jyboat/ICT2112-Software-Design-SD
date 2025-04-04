using ClearCare.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClearCare.Models.Entities;
using System.Text.Json;

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
            string? service,
            string? timeSlot)
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
                appointments = appointments.Where(a => a.getAttribute("DoctorId") == doctorId).ToList();
            }
            if (!string.IsNullOrEmpty(patientId))
            {
                appointments = appointments.Where(a => a.getAttribute("PatientId") == patientId).ToList();
            }
            if (!string.IsNullOrEmpty(nurseId))
            {
                appointments = appointments.Where(a => a.getAttribute("NurseId") == nurseId).ToList();
            }
            if (!string.IsNullOrEmpty(location))
            {
                appointments = appointments.Where(a => a.getAttribute("Location") == location).ToList();
            }
            if (!string.IsNullOrEmpty(service))
            {
                appointments = appointments.Where(a => a.getAttribute("Service") == service).ToList();
            }
            if (!string.IsNullOrEmpty(timeSlot))
            {
                appointments = appointments.Where(a => a.getAttribute("Slot") == timeSlot).ToList();
            }

            // Convert filtered data to JSON format required by FullCalendar
            var eventList = appointments.Select(a => new
            {
                id = a.getAttribute("AppointmentId"),
                title = a.getAttribute("Service") + " for " + a.getAttribute("PatientId"),
                start = DateTime.Parse(a.getAttribute("Datetime")).ToString("yyyy-MM-ddTHH:mm:ss"),
                extendedProps = new
                {
                    patientId = a.getAttribute("PatientId"),
                    nurseId = a.getAttribute("NurseId"),
                    doctorId = a.getAttribute("DoctorId"),
                    status = a.getAttribute("Status"),
                    serviceType = a.getAttribute("Service"),
                    slot = a.getAttribute("Slot"),
                    location = a.getAttribute("Location"),
                    dateTime = DateTime.Parse(a.getAttribute("Datetime")).ToString("yyyy-MM-ddTHH:mm:ss"),
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
                    if (string.IsNullOrEmpty(status) || status == "Missed" || status == "Backlog")
                        return true;
                }

                return false;
            })
            .ToList();     

            return filtered;
        }
    }
}