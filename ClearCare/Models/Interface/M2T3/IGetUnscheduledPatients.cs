// implemented by Service Appointment (M2T3); 
// used by Manual Appointment Management (M2T3)
// used by Automatic Appointment Management (M2T3)

using System.Collections.Generic;
using ClearCare.Models.Entities;
using System.Threading.Tasks;


namespace ClearCare.Interfaces
{
    public interface IGetUnscheduledPatients
    {
       Task<(List<Dictionary<string, object>> appointments, Dictionary<string, string> patientNames)> getUnscheduledPatients();
       
    }
}