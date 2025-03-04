using ClearCare.Interfaces;
using ClearCare.Models.Entities;
using System.Collections.Generic;

namespace ClearCare.Models.Control
{
    public class NurseAvailabilityManager : INurseAvailability, IAvailabilityDB_Receive
    {
        private readonly IAvailabilityDB_Send _dbGateway;

        public NurseAvailabilityManager(IAvailabilityDB_Send dbGateway)
        {
            _dbGateway = dbGateway;
        }

        // Get all availabilities for all nurses
        public List<NurseAvailability> getAllStaffAvailability()
        {
            List<NurseAvailability> availabilities = _dbGateway.retrieveAllStaffAvailability();
            return availabilities;
        }

        // Get availability for a specific nurse
        public List<NurseAvailability> getAvailabilityByStaff(string nurseId)
        {
            List<NurseAvailability> availabilities = _dbGateway.retrieveAvailabilityByStaff(nurseId);
            return availabilities;
        }

        // Add new availability
        public void addAvailability(string nurseID, string date)
        {
            // Retrieve all existing availabilities to find the highest ID
            List<NurseAvailability> allAvailabilities = _dbGateway.retrieveAllStaffAvailability();

            // Find the highest availability ID in the existing records
            int maxAvailabilityId = 0;
            foreach (var existingAvailability in allAvailabilities)
            {
                if (existingAvailability.getAvailabilityDetails().ContainsKey("availabilityId"))
                {
                    int currentId = Convert.ToInt32(existingAvailability.getAvailabilityDetails()["availabilityId"]);
                    if (currentId > maxAvailabilityId)
                    {
                        maxAvailabilityId = currentId;
                    }
                }
            }

            int newAvailabilityId = maxAvailabilityId + 1;

            NurseAvailability availability = NurseAvailability.setAvailabilityDetails(
                newAvailabilityId, nurseID, date, "08:00:00", "16:00:00"
            );

            _dbGateway.createAvailability(availability);
            
            // receiveAddStatus("Success");
        }

        // Update existing availability
        public void updateAvailability(int availabilityId, string nurseID, string date)
        {
            NurseAvailability updatedAvailability = NurseAvailability.setAvailabilityDetails(
                availabilityId, nurseID, date, "08:00:00", "16:00:00"
            );

            _dbGateway.modifyAvailability(updatedAvailability);
            // receiveUpdateStatus("Success");
        }

        // Delete an availability
        public void deleteAvailability(int availabilityId)
        {
            _dbGateway.removeAvailability(availabilityId);
            // receiveDeleteStatus("Success");
        }

        // Implement IAvailabilityDB_Receive interface
        // public void receiveAvailabilityList(List<NurseAvailability> allAvailability)
        // {
            // Handle received data (right now use case is caching the availablity list for future use)
            // private List<NurseAvailability> _cachedAvailabilityList = new List<NurseAvailability>();

        // }

        // public void receiveAddStatus(string status)
        // {
        //     // Handle add operation status
        // }

        // public void receiveUpdateStatus(string status)
        // {
        //     // Handle update operation status
        // }

        // public void receiveDeleteStatus(string status)
        // {
        //     // Handle delete operation status
        // }
    }
}
