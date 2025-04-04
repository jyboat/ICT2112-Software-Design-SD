using ClearCare.Interfaces;
using ClearCare.Models.Entities;
using System.Text.Json;
using ClearCare.Models.Interface;

namespace ClearCare.Models.Control
{
    public class ManualAppointmentScheduler : IRescheduleAppointment
    {
        private readonly ICreateAppointment _iCreateAppointment;
        private readonly IRetrieveAllAppointments _iRetrieveAppointment;
        private readonly INurseAvailability _iNurseAvailability;
        private readonly INotification _iNotification;
        private readonly IServiceType _iServiceType;
        public ManualAppointmentScheduler()
        {
            _iCreateAppointment = (ICreateAppointment)new ServiceAppointmentManagement();
            _iNurseAvailability = (INurseAvailability)new NurseAvailabilityManagement();
            _iNotification = (INotification)new NotificationManager();
            _iRetrieveAppointment = (IRetrieveAllAppointments)new ServiceAppointmentStatusManagement();
            _iServiceType = (IServiceType)new ServiceTypeManager();
        }

        public async Task<List<ServiceType>> getServices()
        {
            List<ServiceType> services = await _iServiceType.getServiceTypes();
            return services;
        }

        private async Task<bool> validateAppointmentSlot(string patientId, string nurseId,
            string doctorId, DateTime dateTime, int slot, string currentAppointmentId = null)
        {
            bool isValid = true;

            // convert datetime to SGT for validation
            DateTime sgtDateTime = dateTime.AddHours(8);

            // 1st check: is the dateTime in the past?
            if (sgtDateTime.Date < DateTime.Now.Date)
            {
                return false; // invalid if date is in the past
            }

            // 2nd check: is the nurse available on this day?

            // retrieve staff availability
            var nurseAvailability = await _iNurseAvailability.getAvailabilityByStaff(nurseId);

            // Use SGT datetime instead of UTC datetime
            var requestedDate = sgtDateTime.Date;

            // return false if there's NO availability record for the nurse on the requested day
            bool hasValidAvailability = false;

            foreach (var availability in nurseAvailability)
            {
                var availabilityDetails = availability.getAvailabilityDetails();

                DateTime availabilityDate = DateTime.Parse(availabilityDetails["date"].ToString());

                // Check if the requested date matches the availability date
                if (requestedDate == availabilityDate.Date)
                {
                    hasValidAvailability = true;
                    break;
                }
            }

            // If no valid availability was found, return false
            if (!hasValidAvailability)
            {
                return false;
            }

            // 3rd check: is the same nurse already booked for another appointment at this time?
            var nurseAppointments = await _iRetrieveAppointment.retrieveAllAppointmentsByNurse(nurseId);

            foreach (var appointment in nurseAppointments)
            {
                string apptId = appointment.getAttribute("AppointmentId");

                // Skip current appointment if doing rescheduling
                if (currentAppointmentId != null && apptId == currentAppointmentId)
                {
                    continue;
                }

                var apptDateTime = DateTime.Parse(appointment.getAttribute("Datetime"));
                var apptSlot = appointment.getIntAttribute("Slot");

                // check if same date and slot
                if (apptDateTime.Date == requestedDate && apptSlot == slot)
                {
                    return false; // conflict found
                }
            }
            return isValid;
        }

        public async Task<string> scheduleAppointment(string patientId, string nurseId,
            string doctorId, string Service, string status, DateTime dateTime, int slot, string location)
        {
            bool isSlotValid = await validateAppointmentSlot(patientId, nurseId, doctorId, dateTime, slot);

            if (!isSlotValid)
            {
                throw new InvalidOperationException("The selected appointment slot is invalid. Nurse is unavailable");
            }

            DateTime dbDateTime = dateTime;  // Make a copy to track any conversions

            // calling the CreateAppointment method from the ICreateAppointment interface
            string createdAppointmentId = await _iCreateAppointment.createAppointment(
                patientId, nurseId, doctorId, Service, status, dbDateTime, slot, location);

            if (string.IsNullOrEmpty(createdAppointmentId))
            {
                throw new InvalidOperationException("Failed to create the appointment.");
            }

            var message = "Your appointment has been scheduled.";

            await _iNotification.createNotification(patientId, message);

            return createdAppointmentId;
        }

        public async Task<bool> rescheduleAppointment(string appointmentId, string patientId, string nurseId,
            string doctorId, string Service, string status, DateTime dateTime, int slot, string location)
        {
            // retrieve the current appointment to compare changes
            var currentAppointment = await _iRetrieveAppointment.getServiceAppointmentById(appointmentId);
            if (currentAppointment == null)
            {
                return false;
            }

            string currentDateTimeStr = currentAppointment.getAttribute("Datetime");
            DateTime currentDateTime = DateTime.Parse(currentDateTimeStr);

            int currentSlot = currentAppointment.getIntAttribute("Slot");

            // check if the nurse or timeslot is changing
            bool nurseChanged = nurseId != currentAppointment.getAttribute("NurseId");
            bool dateChanged = dateTime.Date != currentDateTime.Date;
            bool slotChanged = slot != currentSlot;

            bool needsValidation = nurseChanged || dateChanged || slotChanged;

            // only validate if we're changing time or nurse
            if (needsValidation)
            {
                bool isSlotValid = await validateAppointmentSlot(patientId, nurseId, doctorId, dateTime, slot, appointmentId);

                if (!isSlotValid)
                {
                    throw new InvalidOperationException("The selected appointment slot is invalid. Nurse is unavailable or already booked.");
                }
            }

            DateTime sgtDateTime = dateTime.AddHours(8);

            // create a new object with the updated values
            var updatedAppointment = ServiceAppointment.setApptDetails(
                patientId, nurseId, doctorId, Service, status, sgtDateTime, slot, location
            );

            // Set the appointment ID using reflection
            typeof(ServiceAppointment)
                .GetProperty("AppointmentId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(updatedAppointment, appointmentId);

            bool updated = await _iCreateAppointment.updateAppointment(updatedAppointment);

            if (!updated)
            {
                throw new InvalidOperationException("Failed to reschedule the appointment.");
            }

            var message = "manual scheduling works reschedule";

            await _iNotification.createNotification("USR007", message);

            return updated;
        }

        public async Task<bool> deleteAppointment(string appointmentId)
        {
            bool deleted = await _iCreateAppointment.deleteAppointment(appointmentId);
            return deleted;
        }
    }
}