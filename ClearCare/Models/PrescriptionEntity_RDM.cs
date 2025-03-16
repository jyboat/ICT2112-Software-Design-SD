using System;
using System.Collections.Generic;

namespace ClearCare.Models
{
    public class PrescriptionEntity_RDM
    {
        // Fields/Properties based on your UML
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public List<string> DrugList { get; set; }
        public string DosageInfo { get; set; }
        public DateTime Date { get; set; }

        // Constructor
        public PrescriptionEntity_RDM()
        {
            // Initialize lists or other complex properties here
            DrugList = new List<string>();
        }

        // If you want explicit setter methods:
        public void SetPatientId(int id)
        {
            PatientId = id;
        }

        public void SetDoctorId(int id)
        {
            DoctorId = id;
        }

        public void SetDrugList(List<string> drugs)
        {
            DrugList = drugs;
        }

        public void SetDate(DateTime date)
        {
            Date = date;
        }

        public void SetDosageInfo(string info)
        {
            DosageInfo = info;
        }
    }
}
