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
    }
}