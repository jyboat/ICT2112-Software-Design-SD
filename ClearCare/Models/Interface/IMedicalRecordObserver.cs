using System.Collections.Generic;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ClearCare.Models.Interface
{
    public interface IMedicalRecordObserver
    {
        void OnMedicalRecordUpdated(List<MedicalRecord> updatedRecords);
    }
}
