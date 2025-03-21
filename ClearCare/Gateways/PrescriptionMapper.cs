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
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.GetApplicationDefault()
                });
            }

            _db = FirestoreDb.Create("ict2112");
        }

        public async Task<List<PrescriptionModel>> getAllPrescriptionsAsync()
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

        public async Task addPrescriptionAsync(PrescriptionModel prescription)
        {
            try
            {
                prescription.DateIssued = DateTime.SpecifyKind(prescription.DateIssued, DateTimeKind.Utc);
                await _db.Collection("Prescriptions").AddAsync(prescription);
                Console.WriteLine($"Prescription added for Patient: {prescription.PatientId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding prescription: {ex.Message}");
            }
        }

        public async Task savePrescriptionsAsync(string medicationPlan)
        {
            try
            {
                var data = new { Plan = medicationPlan, Created = DateTime.UtcNow };
                await _db.Collection("Prescriptions").AddAsync(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving prescription plan: {ex.Message}");
            }
        }

        public async Task fetchSharedPrescriptionsAsync(string userId)
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

        public async Task<List<PrescriptionModel>> fetchPrescriptions()
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
