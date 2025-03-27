using ClearCare.Interfaces;
using ClearCare.Models.Entities;
using System.Text.Json;

namespace ClearCare.Models.Control
{
    public class ManualAppointmentScheduler
    {
        private readonly ICreateAppointment _iCreateAppointment;
        private readonly IRetrieveAllAppointments _iRetrieveAppointment;
        private readonly INurseAvailability _iNurseAvailability;
        public ManualAppointmentScheduler()
        {
            _iCreateAppointment = (ICreateAppointment) new ServiceAppointmentManagement();
            _iNurseAvailability = (INurseAvailability) new NurseAvailabilityManagement();
            _iRetrieveAppointment = (IRetrieveAllAppointments) new ServiceAppointmentStatusManagement();
        }

        public async Task<bool> ValidateAppointmentSlot(string patientId, string nurseId,
            string doctorId, DateTime dateTime, int slot, string currentAppointmentId = null)
        {
            bool isValid = true;
            
            // convert datetime to SGT for validation
            DateTime sgtDateTime = dateTime.AddHours(8);
            
            // 0th check: is the dateTime in the past?
            if (sgtDateTime.Date < DateTime.Now.Date)
            {
                return false; // invalid if date is in the past
            }
            
            // 1st check: is the nurse available on this day?
            
            // retrieve staff availability
            var nurseAvailability = await _iNurseAvailability.getAvailabilityByStaff(nurseId);
    
            // Use SGT datetime instead of UTC datetime
            var requestedDate = sgtDateTime.Date;
            var requestedTime = sgtDateTime.TimeOfDay;
            
            // if any availability records show that the nurse is unavailable for that day, return false
            foreach (var availability in nurseAvailability)
            {
                var availabilityDetails = availability.getAvailabilityDetails();

                // extract the date, start time, and end time from the availability
                DateTime availabilityDate = DateTime.Parse(availabilityDetails["date"].ToString());
                TimeSpan startTime = TimeSpan.Parse(availabilityDetails["startTime"].ToString());
                TimeSpan endTime = TimeSpan.Parse(availabilityDetails["endTime"].ToString());

                // check if the requested date matches the availability date
                if (requestedDate == availabilityDate.Date)
                {
                    // check if the requested time is within the unavailability window
                    if (requestedTime >= startTime && requestedTime <= endTime)
                    {
                        return false; // The nurse is unavailable - immediately return false
                    }
                }
            }

            // 2nd check: is the same nurse already booked for another appointment at this time?
            
            var nurseAppointments = await _iRetrieveAppointment.RetrieveAllAppointmentsByNurse(nurseId);

            foreach (var appointment in nurseAppointments)
            {   
                string apptId = appointment.GetAttribute("AppointmentId");
                
                // Skip current appointment if we are doing rescheduling
                if (currentAppointmentId != null && apptId == currentAppointmentId)
                {
                    continue;
                }
                
                var apptDateTime = DateTime.Parse(appointment.GetAttribute("Datetime"));
                var apptSlot = appointment.GetIntAttribute("Slot");
                
                // check if same date and slot
                if (apptDateTime.Date == requestedDate && apptSlot == slot)
                {
                    return false; // conflict found
                }
            }

            return isValid;
        }

        public async Task<string> ScheduleAppointment(string patientId, string nurseId,
            string doctorId, string serviceTypeId, string status, DateTime dateTime, int slot, string location)
        {
            bool isSlotValid = await ValidateAppointmentSlot(patientId, nurseId, doctorId, dateTime, slot);

            if (!isSlotValid)
            {
                throw new InvalidOperationException("The selected appointment slot is invalid. Nurse is unavailable");
            }

            DateTime dbDateTime = dateTime;  // Make a copy to track any conversions

            // calling the CreateAppointment method from the ICreateAppointment interface
            string createdAppointmentId = await _iCreateAppointment.CreateAppointment(
                patientId, nurseId, doctorId, serviceTypeId, status, dbDateTime, slot, location);

            if (!string.IsNullOrEmpty(createdAppointmentId))
            {
                // Appointment created successfully
            }
            else
            {
                throw new InvalidOperationException("Failed to create the appointment.");
            }

            return createdAppointmentId;
        }

        public async Task<bool> RescheduleAppointment(string appointmentId, string patientId, string nurseId,
            string doctorId, string serviceTypeId, string status, DateTime dateTime, int slot, string location)
        {
            // retrieve the current appointment to compare changes
            var currentAppointment = await _iRetrieveAppointment.getServiceAppointmentById(appointmentId);
            if (currentAppointment == null)
            {
                return false;
            }
            
            string currentDateTimeStr = currentAppointment.GetAttribute("Datetime");
            DateTime currentDateTime = DateTime.Parse(currentDateTimeStr);
            
            int currentSlot = currentAppointment.GetIntAttribute("Slot");
            
            // check if the nurse or timeslot is changing
            bool nurseChanged = nurseId != currentAppointment.GetAttribute("NurseId");
            bool dateChanged = dateTime.Date != currentDateTime.Date;
            bool slotChanged = slot != currentSlot;
            
            bool needsValidation = nurseChanged || dateChanged || slotChanged;
            
            // only validate if we're changing time or nurse
            if (needsValidation)
            {
                bool isSlotValid = await ValidateAppointmentSlot(patientId, nurseId, doctorId, dateTime, slot, appointmentId);

                if (!isSlotValid)
                {
                    throw new InvalidOperationException("The selected appointment slot is invalid. Nurse is unavailable or already booked.");
                }
            }
            
            DateTime sgtDateTime = dateTime.AddHours(8);
            
            // create a new object with the updated values
            var updatedAppointment = ServiceAppointment.setApptDetails(
                patientId, nurseId, doctorId, serviceTypeId, status, sgtDateTime, slot, location
            );
            
            // Set the appointment ID using reflection
            typeof(ServiceAppointment)
                .GetProperty("AppointmentId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(updatedAppointment, appointmentId);
            
            bool updated = await _iCreateAppointment.UpdateAppointment(updatedAppointment);
            
            if (!updated)
            {
                throw new InvalidOperationException("Failed to reschedule the appointment.");
            }

            return updated;
        }
    }
}