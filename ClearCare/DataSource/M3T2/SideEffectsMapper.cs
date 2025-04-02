using ClearCare.Models.Entities.M3T2;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using System;
using System.Threading.Tasks;

namespace ClearCare.DataSource.M3T2
{
    public class SideEffectsMapper
    {
        private readonly FirestoreDb _db;

        public SideEffectsMapper()
        {
            _db = FirebaseService.Initialize();
        }

        public async Task<List<SideEffectModel>> getAllSideEffectsAsync()
        {
            var sideEffects = new List<SideEffectModel>();

            try
            {
                var collection = _db.Collection("SideEffects");
                var snapshot = await collection.GetSnapshotAsync();

                foreach (var document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        var sideEffect = document.ConvertTo<SideEffectModel>();
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

        public async Task addSideEffectAsync(SideEffectModel sideEffect)
        {
            try
            {
                var collection = _db.Collection("SideEffects");
                await collection.AddAsync(sideEffect);
                Console.WriteLine($"Side effect added: {sideEffect.DrugName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding side effect: {ex.Message}");
            }
        }
    }
}
