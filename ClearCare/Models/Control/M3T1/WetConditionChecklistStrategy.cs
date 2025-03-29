namespace ClearCare.Models.Control.M3T1
{
    public class WetConditionChecklistStrategy : BaseHazardChecklistStrategy
    {
        public override string HazardType => "Wet Condition";

        public override Dictionary<string, bool> getDefaultChecklist()
        {
            return new Dictionary<string, bool>
            {
                { "Are bathroom floors covered with non-slip mats?", false },
                { "Are grab bars installed in shower/tub areas?", false },
                { "Are spills cleaned up immediately in kitchen areas?", false },
                { "Are floor drainage systems working properly?", false },
                { "Are wet floor warning signs available and used?", false },
                { "Is proper non-slip footwear worn in wet areas?", false },
                { "Are all rugs secured with non-slip backing?", false },
                { "Are high-traffic pathways kept dry?", false },
                { "Are handrails installed near bathroom sinks?", false },
                { "Are floor surfaces slip-resistant when wet?", false }
            };
        }

        public override List<Dictionary<string, string>> getQualifiedDoctors() => new()
        {
            new Dictionary<string, string> { {"id", "D5"}, {"name","Dr. Davis (Dermatology)"} },
            new Dictionary<string, string> { {"id", "D6"}, {"name","Dr. Miller (General Practice)"} },
            new Dictionary<string, string> { {"id", "D7"}, {"name","Dr. Rodriguez (Podiatry)"} },
            new Dictionary<string, string> { {"id", "D8"}, {"name","Dr. Wilson (Sports Medicine)"} },
            new Dictionary<string, string> { {"id", "D9"}, {"name","Dr. Taylor (Occupational Medicine)"} }
        };
    }
}