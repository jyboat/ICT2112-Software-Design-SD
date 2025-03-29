namespace ClearCare.Models.Control.M3T1
{
    public class FireSafetyChecklistStrategy : BaseHazardChecklistStrategy
    {
        public override string HazardType => "Fire Safety";

        public override Dictionary<string, bool> getDefaultChecklist() => new()
        {
            { "Are smoke detectors installed on every floor?", false },
            { "Are fire extinguishers accessible and properly charged?", false },
            { "Are emergency exits clearly marked and unobstructed?", false },
            { "Are electrical cords and outlets in good condition?", false },
            { "Are space heaters kept 3+ feet from flammable materials?", false },
            { "Are smoking materials properly extinguished and stored?", false },
            { "Is cooking equipment monitored and kept clean?", false },
            { "Are flammable liquids stored in proper containers?", false },
            { "Is there an emergency evacuation plan posted?", false },
            { "Is a fire blanket available in kitchen areas?", false }
        };

        public override List<Dictionary<string, string>> getQualifiedDoctors() => new()
        {
            new Dictionary<string, string> { {"id", "D1"}, {"name", "Dr. Smith (Respiratory)"} },
            new Dictionary<string, string> { {"id", "D2"}, {"name", "Dr. Johnson (Emergency)"} },
            new Dictionary<string, string> { {"id", "D13"}, {"name", "Dr. Patel (Burn Specialist)"} },
            new Dictionary<string, string> { {"id", "D14"}, {"name", "Dr. Wong (Pulmonology)"} },
            new Dictionary<string, string> { {"id", "D15"}, {"name", "Dr. Kim (Critical Care)"} }
        };
    }
}