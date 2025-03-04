using System.Collections.Generic;
using ClearCare.Models.Entities;

namespace ClearCare.Interfaces
{
    public interface INurseAvailability
    {
        List<NurseAvailability> getAvailabilityByStaff(string staffId);
        List<NurseAvailability> getAllStaffAvailability();
    }
}