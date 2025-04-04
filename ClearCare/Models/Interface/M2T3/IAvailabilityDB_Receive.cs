using System.Collections.Generic;
using ClearCare.Models.Entities;
using System.Threading.Tasks;

namespace ClearCare.Interfaces
{
    public interface IAvailabilityDB_Receive
    {
        // Receive the list of all nurse availabilities - implemented from NurseAvailabilityManagement; used in NurseAvailabilityGateway (fetchAvailabilityByStaff & fetchAllStaffAvailability)
        Task receiveAvailabilityList(List<NurseAvailability> allAvailability);

        // Receive the status of adding an availability - implemented from NurseAvailabilityManagement; used in NurseAvailabilityGateway (createAvailability)
        Task receiveAddStatus(string status);
        // Receive the status of updating an availability - implemented from NurseAvailabilityManagement; used in NurseAvailabilityGateway (modifyAvailability)

        Task receiveUpdateStatus(string status);

        // Receive the status of deleting an availability - implemented from NurseAvailabilityManagement; used in NurseAvailabilityGateway (removeAvailability)
        Task receiveDeleteStatus(string status);
    }
}
