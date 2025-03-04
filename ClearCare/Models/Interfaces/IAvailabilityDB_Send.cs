using ClearCare.Models.Entities;
using System.Collections.Generic;

namespace ClearCare.Interfaces
{
    public interface IAvailabilityDB_Send
    {
        List<NurseAvailability> getAllStaffAvailability();
        List<NurseAvailability> getAvailabilityByStaff(string staffId);
        void addAvailability(NurseAvailability availability);
        void updateAvailability(NurseAvailability availability);
        void deleteAvailability(int availabilityId);
    }
}
