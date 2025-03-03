using ClearCare.Models;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Gateways
{
    public class SideEffectGateway
    {
        private readonly FirestoreDb _db;

        public SideEffectGateway()
        {
            // Initialize Firebase if not already done
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.GetApplicationDefault()
                });
            }

            // Replace with your actual Firebase/GCP project ID
            _db = FirestoreDb.Create("ict2112");
        }

        /// <summary>
        /// Fetch all side effects from Firestore.
        /// </summary>
        public async Task<List<SideEffect>> GetAllSideEffectsAsync()
        {
            var sideEffects = new List<SideEffect>();

            try
            {
                var collection = _db.Collection("SideEffects");
                var snapshot = await collection.GetSnapshotAsync();

                foreach (var document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        // Convert the Firestore doc to a SideEffect object
                        var sideEffect = document.ConvertTo<SideEffect>();

                        // If you also want the doc ID in sideEffect.Id, 
                        // the [FirestoreDocumentId] attribute does that automatically.
                        // But you could do it manually if needed:
                        // sideEffect.Id = document.Id;

                        sideEffects.Add(sideEffect);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching side effects: {ex.Message}");
            }

            return sideEffects;
        }

        /// <summary>
        /// Add a new side effect document to Firestore.
        /// </summary>
        public async Task AddSideEffectAsync(SideEffect sideEffect)
        {
            try
            {
                var collection = _db.Collection("SideEffects");

                // If sideEffect.Id is null or empty, generate a new GUID
                if (string.IsNullOrEmpty(sideEffect.Id))
                {
                    sideEffect.Id = Guid.NewGuid().ToString();
                }

                // Reference the document by this ID
                var docRef = _db.Collection("SideEffects").Document(sideEffect.Id);

                // SetAsync(...) will create or overwrite the document with this ID
                await docRef.SetAsync(sideEffect);  // Overwrites or creates doc with this ID

                Console.WriteLine($"Side effect saved with ID: {sideEffect.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding side effect: {ex.Message}");
            }
        }


        // (Optional) You can add Update or Delete methods below:
        // e.g., public async Task UpdateSideEffectAsync(SideEffect sideEffect) { ... }
        //       public async Task DeleteSideEffectAsync(string id) { ... }
    }
}
