using System.Collections.Generic;
using ClearCare.Models.Entities;

namespace ClearCare.Interfaces
{
    public interface IAvailabilityDB_Receive
    {
        // Receive the list of all nurse availabilities
        void receiveAvailabilityList(List<NurseAvailability> allAvailability);

        // Receive the status of adding an availability
        // void receiveAddStatus(string status);

        // Receive the status of updating an availability
        // void receiveUpdateStatus(string status);

        // Receive the status of deleting an availability
        // void receiveDeleteStatus(string status);
    }
}
