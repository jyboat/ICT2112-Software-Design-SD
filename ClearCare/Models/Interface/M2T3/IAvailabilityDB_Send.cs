using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Interfaces
{
    public interface IAvailabilityDB_Send
    {
        IAvailabilityDB_Receive Receiver { get; set; }

        // Fetches the list of all nurse availabilities from the database - implemented from NurseAvailabilityGateway; used in NurseAvailabilityManagement (getAllStaffAvailability)
        Task<List<NurseAvailability>> fetchAllStaffAvailability();

        // Fetches the list of availabilities for a specific nurse identified by staffId - implemented from NurseAvailabilityGateway; used in NurseAvailabilityManagement (getAvailabilityByStaff)
        Task<List<NurseAvailability>> fetchAvailabilityByStaff(string staffId);

        Task<List<NurseAvailability>> fetchAvailability();

        // Creates a new nurse availability record in the database - implemented from NurseAvailabilityGateway; used in NurseAvailabilityManagement (addAvailability)
        Task createAvailability(NurseAvailability availability);

        // Modifies an existing nurse availability record in the database - implemented from NurseAvailabilityGateway; used in NurseAvailabilityManagement (updateAvailability)

        Task modifyAvailability(NurseAvailability availability);

        // Removes a nurse availability record from the database - implemented from NurseAvailabilityGateway; used in NurseAvailabilityManagement (deleteAvailability)
        Task removeAvailability(int availabilityId);
    }
}