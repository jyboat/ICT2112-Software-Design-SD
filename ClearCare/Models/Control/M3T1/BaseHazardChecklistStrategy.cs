// BaseHazardChecklistStrategy.cs
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.DataSource;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T1;

namespace ClearCare.Models.Control.M3T1
{
    public abstract class BaseHazardChecklistStrategy : IHazardChecklistStrategy
    {
        public abstract string HazardType { get; }
        public abstract Dictionary<string, bool> getDefaultChecklist();
        public abstract List<Dictionary<string, string>> getQualifiedDoctors(); // Changed to Dictionary
        public virtual bool canHandle(string hazardType) => 
            string.Equals(HazardType, hazardType, StringComparison.OrdinalIgnoreCase);
    }
}