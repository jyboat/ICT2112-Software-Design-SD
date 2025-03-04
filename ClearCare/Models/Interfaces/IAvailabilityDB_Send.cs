using ClearCare.Models.Entities;
using System.Collections.Generic;

namespace ClearCare.Interfaces
{
    public interface IAvailabilityDB_Send
    {
        List<NurseAvailability> retrieveAllStaffAvailability();
        List<NurseAvailability> retrieveAvailabilityByStaff(string staffId);
        void createAvailability(NurseAvailability availability);
        void modifyAvailability(NurseAvailability availability);
        void removeAvailability(int availabilityId);
    }
}
