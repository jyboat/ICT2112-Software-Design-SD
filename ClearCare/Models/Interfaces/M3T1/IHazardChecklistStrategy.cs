using System.Collections.Generic;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface IHazardChecklistStrategy
    {
        string HazardType { get; }
        Dictionary<string, bool> getDefaultChecklist();
        List<Dictionary<string, string>> getQualifiedDoctors();
        bool canHandle(string hazardType);
    }
}