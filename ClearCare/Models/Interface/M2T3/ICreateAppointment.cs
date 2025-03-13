// implemented by Service Appointment (M2T3); 
// used by Manual Appointment Management (M2T3)
// used by Automatic Appointment Management (M2T3)

using System.Collections.Generic;
using ClearCare.Models.Entities;
using System.Threading.Tasks;


namespace ClearCare.Interfaces
{
    public interface ICreateAppointment
    {
        // Implemented by Service Appointment Management: Models/Control/ServiceAppointmentManagement.cs
        // Used by Automatic Appointment Management: TBC [TO BE CODED]
        // Used by ManualAppointment Management : TBC [TO BE CODED]
        Task CreateAppointment();
    }
}