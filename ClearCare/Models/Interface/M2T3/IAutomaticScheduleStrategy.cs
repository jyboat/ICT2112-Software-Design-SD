using System.Collections.Generic;
using ClearCare.Models.Entities;
using ClearCare.Models.Control;

namespace ClearCare.Interfaces
{
    public interface IAutomaticScheduleStrategy
    {
        List<ServiceAppointment> AutomaticallySchedule(
            List<AutomaticAppointmentScheduler.Nurse> nurses, 
            List<AutomaticAppointmentScheduler.Patient> appointments,
            List<string> services/*,
            Action<string, string> notify*/);
    }
}