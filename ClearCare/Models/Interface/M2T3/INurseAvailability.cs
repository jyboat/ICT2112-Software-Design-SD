// implemented by NurseAvailabilityManagement; used by ServiceAppointment

using System.Collections.Generic;
using ClearCare.Models.Entities;

namespace ClearCare.Interfaces
{
    public interface INurseAvailability
    {
        Task<List<NurseAvailability>> getAvailabilityByStaff(string staffId);
        Task<List<NurseAvailability>> getAllStaffAvailability();
    }
}