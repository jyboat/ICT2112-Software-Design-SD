using ClearCare.Models.Entities;

namespace ClearCare.Models.Interface
{
    public interface IMedicalRecordObserver
    {
        void OnMedicalRecordUpdated(List<MedicalRecord> updatedRecords);
    }
}
