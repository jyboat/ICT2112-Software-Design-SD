using ClearCare.DataSource;
using ClearCare.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Models.Control
{
    public class NurseAvailabilityManager
    {
        private readonly NurseAvailabilityGateway _gateway;

         //  Get all availabilities for ALL nurses
        public async Task<List<NurseAvailability>> RetrieveAllStaffAvailability()
        {
            return await _gateway.GetAllStaffAvailabilityAsync();
        }

        public NurseAvailabilityManager()
        {
            _gateway = new NurseAvailabilityGateway();
        }

        // Get all availabilities for a specific Nurse
        public async Task<List<NurseAvailability>> RetrieveAll(string nurseId)
        {
            return await _gateway.GetAvailabilityByStaffAsync(nurseId);
        }

        // Add New Nurse Availability
        public async Task CreateAvailability(string nurseID, string date)
        {
            int nextAvailabilityId = await _gateway.GetNextAvailabilityIdAsync();

            NurseAvailability availability = NurseAvailability.SetAvailabilityDetails(
                nextAvailabilityId, nurseID, date, "08:00:00", "16:00:00"
            );

            await _gateway.AddAvailabilityAsync(availability);
        }

        // Update an existing availability
        public async Task UpdateAvailability(int availabilityId, string nurseID, string date)
        {
            NurseAvailability updatedAvailability = NurseAvailability.SetAvailabilityDetails(
                availabilityId, nurseID, date, "08:00:00", "16:00:00"
            );

            await _gateway.UpdateAvailabilityAsync(updatedAvailability);
        }

        // Delete an availability
        public async Task DeleteAvailability(int availabilityId)
        {
            await _gateway.DeleteAvailabilityAsync(availabilityId); // No return needed for Task
        }
    }
}
