using System;
using System.Collections.Generic;
using ClearCare;
using ClearCare.Models;

namespace ClearCare.Models
{
    // PrescriptionControl implements iFetchPrescriptions
    public class PrescriptionControl : iFetchPrescriptions
    {
        // 1) Private static instance for the Singleton
        private static PrescriptionControl? _instance;

        // 2) Private constructor to prevent instantiation from outside
        private PrescriptionControl()
        {
            // Optionally initialize an in-memory list of prescriptions
            _prescriptions = new List<PrescriptionEntity_RDM>();
        }

        // 3) Public static property to get the single instance
        public static PrescriptionControl Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PrescriptionControl();
                }
                return _instance;
            }
        }

        // In-memory list (or you could fetch from DB in real-world scenario)
        private List<PrescriptionEntity_RDM> _prescriptions;

        // -----------------------------------------------------------
        // UML Methods
        // -----------------------------------------------------------

        /// <summary>
        /// Create a new prescription based on a "medicationPlan" (string).
        /// For simplicity, we’re just adding a dummy PrescriptionEntity_RDM.
        /// </summary>
        public void CreatePrescription(string medicationPlan)
        {
            // In a real scenario, parse medicationPlan or build the object properly.
            var newPrescription = new PrescriptionEntity_RDM
            {
                PatientId = 1,        // placeholder
                DoctorId = 100,       // placeholder
                DosageInfo = medicationPlan,
                Date = DateTime.Now,
                DrugList = new List<string> { "DrugA", "DrugB" }
            };

            // Add to the in-memory list (or to a database)
            _prescriptions.Add(newPrescription);

            // You might want to return or store an ID, log, etc.
        }

        /// <summary>
        /// Check for drug interactions based on a drug name.
        /// Returns a string describing any found interactions.
        /// </summary>
        public string CheckDrugInteractions(string drugName)
        {
            // For example, if 'drugName' is "Aspirin", check some logic:
            // You could do an external API call or database check here.
            // For now, returning a simple message:
            if (drugName.Equals("Aspirin", StringComparison.OrdinalIgnoreCase))
            {
                return "Warning: Aspirin may interact with certain anticoagulants.";
            }
            return "No known interactions found.";
        }

        /// <summary>
        /// Fetch a prescription based on user email (overloaded method).
        /// </summary>
        public void FetchPrescription(string email)
        {
            // Implementation details:
            // 1) Possibly look up user by email
            // 2) Retrieve prescriptions for that user
            // For demonstration, we’re just printing or returning them:
            Console.WriteLine($"Fetching prescription for email: {email}");

            // If you had a DB call, you would do it here.
        }

        /// <summary>
        /// Fetch a prescription based on userId (overloaded method).
        /// </summary>
        public void FetchPrescription(int userId)
        {
            // Implementation details:
            // 1) Possibly look up user by ID
            // 2) Retrieve prescriptions for that user
            // For demonstration, we’re just printing or returning them:
            Console.WriteLine($"Fetching prescription for user ID: {userId}");
        }

        /// <summary>
        /// Implementation of the iFetchPrescriptions interface method.
        /// Could fetch all prescriptions, for instance.
        /// </summary>
        public void FetchPrescriptions()
        {
            // Return or list all stored prescriptions
            Console.WriteLine("Fetching all prescriptions in the system...");

            foreach (var prescription in _prescriptions)
            {
                Console.WriteLine($"- PatientId: {prescription.PatientId}, " +
                                  $"DoctorId: {prescription.DoctorId}, " +
                                  $"Drugs: {string.Join(", ", prescription.DrugList)}, " +
                                  $"Dosage: {prescription.DosageInfo}, " +
                                  $"Date: {prescription.Date}");
            }
        }


        public List<PrescriptionEntity_RDM> GetAllPrescriptions()
{
    // _prescriptions is your in-memory list in PrescriptionControl
    return _prescriptions;
}

    }
}
