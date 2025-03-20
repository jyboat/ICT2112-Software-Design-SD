using ClearCare.Interfaces;
using ClearCare.Models;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Gateways
{
    public class PrescriptionMapper : IFetchPrescriptions
    {
        private readonly FirestoreDb _db;

        public PrescriptionMapper()
        {
            // Ensure Firebase is only initialized once
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.GetApplicationDefault()
                });
            }

            // Replace "ict2112" with your actual Firebase Project ID
            _db = FirestoreDb.Create("ict2112");
        }

        // Gets all prescriptions from the "Prescriptions" collection
        public async Task<List<PrescriptionModel>> GetAllPrescriptionsAsync()
        {
            var prescriptions = new List<PrescriptionModel>();
            try
            {
                var snapshot = await _db.Collection("Prescriptions").GetSnapshotAsync();
                foreach (var doc in snapshot.Documents)
                {
                    if (doc.Exists)
                    {
                        var item = doc.ConvertTo<PrescriptionModel>();
                        prescriptions.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching prescriptions: {ex.Message}");
            }
            return prescriptions;
        }

        // Adds a new prescription document
        public async Task AddPrescriptionAsync(PrescriptionModel prescription)
        {
            try
            {
                // Convert the DateIssued to UTC if it's not already
                prescription.DateIssued = DateTime.SpecifyKind(prescription.DateIssued, DateTimeKind.Utc);

                await _db.Collection("Prescriptions").AddAsync(prescription);
                Console.WriteLine($"Prescription added for Patient: {prescription.PatientId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding prescription: {ex.Message}");
            }
        }


        // Saves a prescription "plan" string, for example
        public async Task SavePrescriptionsAsync(string medicationPlan)
        {
            try
            {
                // You might parse the plan and store it in a structured way
                var data = new { Plan = medicationPlan, Created = DateTime.UtcNow };
                await _db.Collection("Prescriptions").AddAsync(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving prescription plan: {ex.Message}");
            }
        }

        // Fetches prescriptions for a specific user from "SharedPrescriptions" or similar
        public async Task FetchSharedPrescriptionsAsync(string userId)
        {
            try
            {
                var query = _db.Collection("SharedPrescriptions")
                               .WhereEqualTo("UserId", userId);
                var snapshot = await query.GetSnapshotAsync();

                foreach (var doc in snapshot.Documents)
                {
                    Console.WriteLine($"Shared prescription doc: {doc.Id}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching shared prescriptions: {ex.Message}");
            }
        }

         public async Task<List<PrescriptionModel>> FetchPrescriptions()
        {
            var prescriptions = new List<PrescriptionModel>();
            try
            {
                var snapshot = await _db.Collection("Prescriptions").GetSnapshotAsync();
                foreach (var doc in snapshot.Documents)
                {
                    if (doc.Exists)
                    {
                        var item = doc.ConvertTo<PrescriptionModel>();
                        prescriptions.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching prescriptions: {ex.Message}");
            }

            return prescriptions;
        }
    }
}
