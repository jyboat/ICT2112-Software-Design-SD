using System.Collections.Generic;
using ClearCare.Models.Entities;
using ClearCare.Models.Control;

namespace ClearCare.Interfaces
{
    public interface IAutomaticScheduleStrategy
    {
        List<ServiceAppointment> AutomaticallySchedule(
            List<ServiceAppointment> unscheduledAppointment,
            List<AutomaticAppointmentScheduler.Nurse> nurses, 
            List<string> services,
            List<ServiceAppointment> backlogEntries,
            Dictionary<string, List<int>> patientSlotTracker,
            Dictionary<string, Dictionary<int,int>> serviceSlotTracker,
            Dictionary<string, List<int>> nurseSlotTracker);
    }
}