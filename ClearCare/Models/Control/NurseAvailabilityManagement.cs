using ClearCare.Interfaces;
using ClearCare.Models.Entities;
using System.Collections.Generic;

namespace ClearCare.Models.Control
{
    public class NurseAvailabilityManager : IAvailabilityDB_Receive
    {
        private readonly IAvailabilityDB_Send _dbGateway;

        public NurseAvailabilityManager(IAvailabilityDB_Send dbGateway)
        {
            _dbGateway = dbGateway;
        }

        // Get all availabilities for all nurses
        public List<NurseAvailability> retrieveAllStaffAvailability()
        {
            List<NurseAvailability> availabilities = _dbGateway.getAllStaffAvailability();
            return availabilities;
        }

        // Get availability for a specific nurse
        public List<NurseAvailability> retrieveAvailabilityByStaff(string nurseId)
        {
            List<NurseAvailability> availabilities = _dbGateway.getAvailabilityByStaff(nurseId);
            return availabilities;
        }

        // Add new availability
        public void createAvailability(string nurseID, string date)
        {
            // Retrieve all existing availabilities to find the highest ID
            List<NurseAvailability> allAvailabilities = _dbGateway.getAllStaffAvailability();

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

            _dbGateway.addAvailability(availability);
            
            receiveAddStatus("Success");
        }

        // Update existing availability
        public void modifyAvailability(int availabilityId, string nurseID, string date)
        {
            NurseAvailability updatedAvailability = NurseAvailability.setAvailabilityDetails(
                availabilityId, nurseID, date, "08:00:00", "16:00:00"
            );

            _dbGateway.updateAvailability(updatedAvailability);
            receiveUpdateStatus("Success");
        }

        // Delete an availability
        public void removeAvailability(int availabilityId)
        {
            _dbGateway.deleteAvailability(availabilityId);
            receiveDeleteStatus("Success");
        }

        // Implement IAvailabilityDB_Receive interface
        public void receiveAvailabilityList(List<NurseAvailability> allAvailability)
        {
            // Handle received data (maybe store it or send to UI)
        }

        public void receiveAddStatus(string status)
        {
            // Handle add operation status
        }

        public void receiveUpdateStatus(string status)
        {
            // Handle update operation status
        }

        public void receiveDeleteStatus(string status)
        {
            // Handle delete operation status
        }
    }
}
