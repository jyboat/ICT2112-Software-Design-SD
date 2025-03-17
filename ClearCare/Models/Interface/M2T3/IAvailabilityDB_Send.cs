// defines the methods for sending (or initiating) database operations such as fetching, creating, updating, and deleting nurse availabilities.
// implemented by NurseAvailabilityGateway; used by NurseAvailabilityManagement 

using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Interfaces
{
    public interface IAvailabilityDB_Send
    {
        // Fetches the list of all nurse availabilities from the database - implemented from NurseAvailabilityGateway; used in NurseAvailabilityManagement (getAllStaffAvailability)
        Task<List<NurseAvailability>> fetchAllStaffAvailability();

        // Fetches the list of availabilities for a specific nurse identified by staffId - implemented from NurseAvailabilityGateway; used in NurseAvailabilityManagement (getAvailabilityByStaff)
        Task<List<NurseAvailability>> fetchAvailabilityByStaff(string staffId);

        // Creates a new nurse availability record in the database - implemented from NurseAvailabilityGateway; used in NurseAvailabilityManagement (addAvailability)
        Task createAvailability(NurseAvailability availability);

        // Modifies an existing nurse availability record in the database - implemented from NurseAvailabilityGateway; used in NurseAvailabilityManagement (updateAvailability)

        Task modifyAvailability(NurseAvailability availability);

        // Removes a nurse availability record from the database - implemented from NurseAvailabilityGateway; used in NurseAvailabilityManagement (deleteAvailability)
        Task removeAvailability(int availabilityId);
    }
}
