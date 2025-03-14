using ClearCare.Models.Entities;

namespace ClearCare.Models.Interface
{
    public interface IMedicalRecordObserver
    {
        void onMedicalRecordUpdated(List<MedicalRecord> updatedRecords);
    }
}
