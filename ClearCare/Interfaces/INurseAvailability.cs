// Interfaces/INurseAvailability.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities;

namespace ClearCare.Interfaces
{
    public interface INurseAvailability
    {
        Task<List<NurseAvailability>> GetAvailabilityByStaffAsync(string staffId);
        Task<List<NurseAvailability>> GetAllStaffAvailabilityAsync();
        Task AddAvailabilityAsync(NurseAvailability availability);
        Task UpdateAvailabilityAsync(NurseAvailability availability);
        Task DeleteAvailabilityAsync(int availabilityId);
    }
}