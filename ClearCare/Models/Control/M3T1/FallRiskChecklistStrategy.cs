namespace ClearCare.Models.Control.M3T1
{
    public class FallRiskChecklistStrategy : BaseHazardChecklistStrategy
    {
        public override string HazardType => "Fall Risk";

        public override Dictionary<string, bool> getDefaultChecklist() => new()
        {
            { "Are all stair railings secure and at proper height?", false },
            { "Are walking surfaces non-slip?", false },
            { "Are step edges visually marked to avoid tripping?", false },
            { "Is lighting sufficient to see potential hazards?", false },
            { "Are walkways free of clutter and obstructions?", false },
            { "Is furniture at proper height for safe transfers?", false },
            { "Is an emergency call system installed and working?", false },
            { "Are grab bars properly installed near toilets?", false },
            { "Are floors kept free of tripping hazards?", false },
            { "Is proper footwear available and worn?", false }
        };

        public override List<Dictionary<string, string>> getQualifiedDoctors() => new()
        {
            new Dictionary<string, string> { {"id", "D3"}, {"name", "Dr. Williams (Geriatrics)"} },
            new Dictionary<string, string> { {"id", "D4"}, {"name", "Dr. Brown (Orthopedics)"} },
            new Dictionary<string, string> { {"id", "D10"}, {"name", "Dr. Lee (Physical Medicine & Rehabilitation)"} },
            new Dictionary<string, string> { {"id", "D11"}, {"name", "Dr. Garcia (Rheumatology)"} },
            new Dictionary<string, string> { {"id", "D12"}, {"name", "Dr. Chen (Neurology)"} }
        };
    }
}