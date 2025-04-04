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