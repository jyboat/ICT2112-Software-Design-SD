using ClearCare.Interfaces;

namespace ClearCare.Models.Control
{
    public class ManualAppointmentScheduler
    {
        private readonly ICreateAppointment _iCreateAppointment;
        private readonly INurseAvailability _iNurseAvailability;
        public ManualAppointmentScheduler(ICreateAppointment ICreateAppointment, INurseAvailability INurseAvailability)
        {
            _iCreateAppointment = ICreateAppointment;
            _iNurseAvailability = INurseAvailability;
        }

        public async Task TestInterface()
        {
            //  await _iCreateAppointment.CreateAppointment();
            var staffAvailability = await _iNurseAvailability.getAllStaffAvailability();
        }

        public async Task<string> CreateAppointment(string patientId, string nurseId,
    string doctorId, string serviceTypeId, string status, DateTime dateTime, int slot, string location)
        {
            // Calling CreateAppointment method from the ICreateAppointment interface
            return await _iCreateAppointment.CreateAppointment(patientId, nurseId, doctorId, serviceTypeId, status, dateTime, slot, location);
        }

        public async Task<bool> ValidateAppointmentSlot(string patientId, string nurseId,
    string doctorId, DateTime dateTime, int slot)
        {
            bool isValid = true;
            Console.WriteLine($"nurseId: {nurseId}");
            Console.WriteLine($"dateTime: {dateTime}");
            
            // 1st check: is the nurse available on this day?

            // retrieve staff availability
            var nurseAvailability = await _iNurseAvailability.getAvailabilityByStaff(nurseId);
            var requestedDate = dateTime.Date;
            var requestedTime = dateTime.TimeOfDay;
            
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
                    // check if the requested time is within the start and end time
                    if (requestedTime >= startTime && requestedTime <= endTime)
                    {
                        Console.WriteLine($"Nurse {availabilityDetails["nurseID"]} is unavailable at {dateTime}.");
                        isValid = false; // The nurse is unavailable
                        break; // exit the loop since availability conflict is found
                    }
                }
            }

            // 2nd check: is the same nurse already booked for another appointment at this time?
            // get all appointments for the nurse
            // probably will need to open interface for this
            var nurseAppointments = await _iCreateAppointment.RetrieveAllAppointmentsByNurse(nurseId);

            foreach (var appointment in nurseAppointments)
            {
                var apptDateTime = DateTime.Parse(appointment.GetAttribute("Datetime"));
                var apptSlot = appointment.GetIntAttribute("Slot");
    
                // check if same date and slot
                if (apptDateTime.Date == requestedDate && apptSlot == slot)
                {
                    Console.WriteLine($"Nurse {nurseId} already has an appointment at slot {slot} on {requestedDate.ToShortDateString()}");
                    return false; // conflict found
                }
            }

            return isValid;
        }

        public async Task<string> ScheduleAppointment(string patientId, string nurseId,
    string doctorId, string serviceTypeId, string status, DateTime dateTime, int slot, string location)
        {
            Console.WriteLine("Scheduling appointment...");

            bool isSlotValid = await ValidateAppointmentSlot(patientId, nurseId, doctorId, dateTime, slot);

            if (!isSlotValid)
            {
                Console.WriteLine("Cannot schedule appointment: invalid slot.");
                throw new InvalidOperationException("The selected appointment slot is invalid. Nurse is unavailable");
            }

            // calling the CreateAppointment method from the ICreateAppointment interface
            string createdAppointmentId = await _iCreateAppointment.CreateAppointment(
                patientId, nurseId, doctorId, serviceTypeId, status, dateTime, slot, location);

            if (!string.IsNullOrEmpty(createdAppointmentId))
            {
                Console.WriteLine($"Appointment created with ID: {createdAppointmentId}");
            }
            else
            {
                Console.WriteLine("Failed to create appointment.");
                throw new InvalidOperationException("Failed to create the appointment."); // Throw an exception
            }

            return createdAppointmentId;
        }
    }
}