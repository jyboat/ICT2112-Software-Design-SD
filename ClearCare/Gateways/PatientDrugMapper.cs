using ClearCare.Models;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using System;
using System.Threading.Tasks;

namespace ClearCare.Gateways
{
    public class PatientDrugMapper
    {
        private readonly FirestoreDb _db;

        public PatientDrugMapper()
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.GetApplicationDefault()
                });
            }

            _db = FirestoreDb.Create("ict2112"); // Replace with your Firebase project ID
        }

        public async Task<List<PatientDrugLogModel>> getDrugLogAsync() {
            var DrugLog = new List<PatientDrugLogModel>(); 

            try {
                var collection = _db.Collection("DrugInformation").WhereEqualTo("PatientId", HardcodedUUIDs.UserUUID);;
                var snapshot = await collection.GetSnapshotAsync(); 

                foreach(var document in snapshot.Documents) {
                    if (document.Exists) {
                        var drugInfo = document.ConvertTo<PatientDrugLogModel>();
                        DrugLog.Add(drugInfo);
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine($"Error fetching drug log: {e.Message}");
            }

            return DrugLog;
        }

        public async Task uploadDrugInfo(PatientDrugLogModel drugInfo) {
            try {
                var collection = _db.Collection("DrugInformation");
                await collection.AddAsync(drugInfo);
                Console.WriteLine($"Drug Info added: {drugInfo.DrugName}");
            }catch (Exception e)
            {
                Console.WriteLine($"Error uploading drug information: {e.Message}");
            }
        }
    }
}
