using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Entities.M3T2;

namespace ClearCare.Models.DTO.M3T1
{
    public class SummaryDTO
    {
        public DischargeSummary Summary { get; set; }
        public Assessment Assessment { get; set; } = null;
        public PrescriptionModel? Prescription { get; set; }
    }
}
