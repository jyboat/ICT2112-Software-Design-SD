using System.Collections.Generic;
using ClearCare.Models.Entities;
using ClearCare.Models.Control;

namespace ClearCare.Interfaces
{
    public interface IAutomaticScheduleStrategy
    {
        List<ServiceAppointment> automaticallySchedule(
            List<ServiceAppointment> unscheduledAppointment,
            List<string> nurses, 
            List<string> services,
            List<ServiceAppointment> backlogEntries,
            Dictionary<string, List<int>> patientSlotTracker,
            Dictionary<string, Dictionary<int,int>> serviceSlotTracker,
            Dictionary<string, List<int>> nurseSlotTracker);
    }
}