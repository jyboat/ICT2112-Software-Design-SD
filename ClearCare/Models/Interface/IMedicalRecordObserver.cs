using System.Collections.Generic;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ClearCare.Models.Interface
{
    public interface IMedicalRecordObserver
    {
        IActionResult OnMedicalRecordUpdated(List<MedicalRecord> updatedRecords);
    }
}
