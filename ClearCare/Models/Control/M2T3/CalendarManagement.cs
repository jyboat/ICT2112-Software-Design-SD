using ClearCare.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public CalendarManagement(IRetrieveAllAppointments retrieveAllAppointments, INurseAvailability getAvailabilityByStaff)
        {
            _retrieveAllAppointments = retrieveAllAppointments;
            _getAvailabilityByStaff = getAvailabilityByStaff;
        }

        public async Task<JsonResult> getAppointmentsForCalendar(
            string? doctorId, 
            string? patientId, 
            string? nurseId,
            string? location,
            string? service)
        {
            // Get all appointments from IRetrieveAllAppointments (implemented by ServiceAppointmentManagement)
            var appointments = await _retrieveAllAppointments.retrieveAllAppointments();

            if (appointments == null || !appointments.Any())
            {
                return new JsonResult(new List<object>()); // Return an empty JSON list if no appointments exist
            }

            // Apply filtering within CalendarManagement
            if (!string.IsNullOrEmpty(doctorId))
            {
                appointments = appointments.Where(a => a["DoctorId"].ToString() == doctorId).ToList();
            }
            if (!string.IsNullOrEmpty(patientId))
            {
                appointments = appointments.Where(a => a["PatientId"].ToString() == patientId).ToList();
            }
            if (!string.IsNullOrEmpty(nurseId))
            {
                appointments = appointments.Where(a => a.ContainsKey("NurseId") && a["NurseId"].ToString() == nurseId).ToList();
            }
            if (!string.IsNullOrEmpty(location))
            {
                appointments = appointments.Where(a => a.ContainsKey("Location") && a["Location"].ToString() == location).ToList();
            }
            if (!string.IsNullOrEmpty(service))
            {
                appointments = appointments.Where(a => a.ContainsKey("ServiceTypeId") && a["ServiceTypeId"].ToString() == service).ToList();
            }

            // Convert filtered data to JSON format required by FullCalendar
            var eventList = appointments.Select(a => new
            {
                id = a["AppointmentId"],
                title = a["ServiceTypeId"] + " for " + a["PatientId"],
                start = ((DateTime)a["DateTime"]).ToString("yyyy-MM-ddTHH:mm:ss"),
                extendedProps = new
                {
                    patientId = a["PatientId"],
                    nurseId = a.ContainsKey("NurseId"),
                    doctorId = a["DoctorId"],
                    status = a["Status"],
                    serviceType = a["ServiceTypeId"],
                    slot = a["Slot"],
                    location = a["Location"],
                    dateTime = ((DateTime)a["DateTime"]).ToString("yyyy-MM-ddTHH:mm:ss"),
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
    }
}
