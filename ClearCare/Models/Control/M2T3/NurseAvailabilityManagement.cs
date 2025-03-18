using ClearCare.Interfaces;
using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.DataSource;

namespace ClearCare.Models.Control
{
    public class NurseAvailabilityManagement : INurseAvailability, IAvailabilityDB_Receive
    {
        private readonly IAvailabilityDB_Send _dbGateway;

        public NurseAvailabilityManagement()
        {
            _dbGateway = (IAvailabilityDB_Send) new NurseAvailabilityGateway();
            _dbGateway.Receiver = this;
        }

        // Implementing INurseAvailability Interface - used by ServiceAppointments not the DB Interfaces!!

        // Get all availabilities for all nurses - implemented in INurseAvailability 
        public async Task<List<NurseAvailability>> getAllStaffAvailability()
        {
            List<NurseAvailability> availabilities = await _dbGateway.fetchAllStaffAvailability();

            // returning availabilities as a List cuz ServiceAppointments need it
            return availabilities;
        }

        // Get availability for a specific nurse - implemented in INurseAvailability 
        public async Task<List<NurseAvailability>> getAvailabilityByStaff(string nurseId)
        {
            List<NurseAvailability> availabilities = await _dbGateway.fetchAvailabilityByStaff(nurseId);

            // returning availabilities as a List cuz ServiceAppointments need it
            return availabilities;
        }

        // Implementing IAvailabilityDB_Receive interface

        // Handle retrieving operation - implemented in IAvailabilityDB_Receive; used in NurseAvailabilityGateway (fetchAvailabilityByStaff & fetchAllStaffAvailability)
        public Task receiveAvailabilityList(List<NurseAvailability> allAvailability)
        {
            if (allAvailability.Count == 0)
            {
                Console.WriteLine("No nurse availabilities found.");
            }
            else
            {
                Console.WriteLine($"Received {allAvailability.Count} nurse availabilities.");
            }
            return Task.CompletedTask;
        }

        // Handle add operation success/failure - implemented in IAvailabilityDB_Receive; used in NurseAvailabilityGateway (createAvailability)
        public Task receiveAddStatus(string status)
        {
            if (status == "Success")
            {
                Console.WriteLine("Availability added successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to add availability: {status}");
            }
            return Task.CompletedTask;
        }

        // Handle update operation success/failure - implemented in IAvailabilityDB_Receive; used in NurseAvailabilityGateway (modifyAvailability)
        public Task receiveUpdateStatus(string status)
        {
            if (status == "Success")
            {
                Console.WriteLine("Availability updated successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to update availability: {status}");
            }
            return Task.CompletedTask;
        }

        // Handle delete operation success/failure - implemented in IAvailabilityDB_Receive; used in NurseAvailabilityGateway (removeAvailability)
        public Task receiveDeleteStatus(string status)
        {
            if (status == "Success")
            {
                Console.WriteLine("Availability deleted successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to delete availability: {status}");
            }
            return Task.CompletedTask;
        }

        // Add new availability - not implemented in any intefaces; used in NurseAvailabilityController (AddAvailability)
        public async Task addAvailability(string nurseID, string date)
        {
            // Retrieve all existing availabilities to find the highest ID
            List<NurseAvailability> allAvailabilities = await _dbGateway.fetchAllStaffAvailability();

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

            await _dbGateway.createAvailability(availability);
        }

        // Update existing availability - not implemented in any intefaces; used in NurseAvailabilityController (UpdateAvailability)
        public async Task updateAvailability(int availabilityId, string nurseID, string date)
        {
            NurseAvailability updatedAvailability = NurseAvailability.setAvailabilityDetails(
                availabilityId, nurseID, date, "08:00:00", "16:00:00"
            );

            await _dbGateway.modifyAvailability(updatedAvailability);
        }

        // Delete an availability - not implemented in any intefaces; used in NurseAvailabilityController (DeleteAvailability)
        public async Task deleteAvailability(int availabilityId)
        {
             await _dbGateway.removeAvailability(availabilityId);
        }
    }
    
}
