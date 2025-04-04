using System.Collections.Generic;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface IHazardChecklistStrategy
    {
        Dictionary<string, bool> getDefaultChecklist();
    }
}