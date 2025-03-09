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
        private readonly INurseAvailability _getCurrentStaffAvailability;

        public CalendarManagement(IRetrieveAllAppointments retrieveAllAppointments, INurseAvailability getCurrentStaffAvailability)
        {
            _retrieveAllAppointments = retrieveAllAppointments;
            _getCurrentStaffAvailability = getCurrentStaffAvailability;
        }

        public async Task<JsonResult> GetAppointmentsForCalendar(string? doctorId, string? patientId, string? nurseId)
        {
            // Get all appointments from IRetrieveAllAppointments (implemented by ServiceAppointmentManagement)
            var appointments = await _retrieveAllAppointments.RetrieveAllAppointments();

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

            // Convert filtered data to JSON format required by FullCalendar
            var eventList = appointments.Select(a => new
            {
                id = a["AppointmentId"],
                title = "Appointment with " + a["DoctorId"],
                start = ((DateTime)a["DateTime"]).ToString("yyyy-MM-ddTHH:mm:ss"),
                extendedProps = new
                {
                    patientId = a["PatientId"],
                    nurseId = a.ContainsKey("NurseId") ? a["NurseId"] : null,
                    doctorId = a["DoctorId"],
                    status = a["Status"],
                    location = a["Location"]
                }
            }).ToList();

            return new JsonResult(eventList);
        }

        public async Task<JsonResult> GetCurrentStaffAvailability(string? currentNurseId)
        {
            var nurseAvailabilityList = await _getCurrentStaffAvailability.getAvailabilityByStaff(currentNurseId);

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
                    start = ((string)details["date"]), 
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
