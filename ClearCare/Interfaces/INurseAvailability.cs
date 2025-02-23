// Interfaces/INurseAvailability.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models;

namespace ClearCare.Interfaces
{
    public interface INurseAvailability
    {
        Task<List<NurseAvailability>> GetAvailabilityByStaffAsync(string staffId);
        Task AddAvailabilityAsync(NurseAvailability availability);
        Task UpdateAvailabilityAsync(NurseAvailability availability);
        Task DeleteAvailabilityAsync(int availabilityId);
    }
}