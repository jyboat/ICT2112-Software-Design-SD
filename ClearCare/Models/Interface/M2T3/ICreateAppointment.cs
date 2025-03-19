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
        Task<string> CreateAppointment (string patientId, string nurseId,
                string doctorId, string serviceTypeId, string status, DateTime dateTime, int slot, string location);
           
        Task<Dictionary<string, object>> getAppointmentByID(string appointmentId);

        Task<bool> DeleteAppointment (string appointmentId);
    }
}