using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Entities.M3T2;

namespace ClearCare.Models.ViewModels
{
    public class SummaryViewModel
    {
        public DischargeSummary Summary { get; set; }
        public PrescriptionModel? Prescription { get; set; }
    }
}
