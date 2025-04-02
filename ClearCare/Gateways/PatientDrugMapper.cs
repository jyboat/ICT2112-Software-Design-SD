using ClearCare.Models;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Gateways
{
    public class PatientDrugMapper
    {
        private readonly FirestoreDb _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientDrugMapper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.GetApplicationDefault()
                });
            }

            _db = FirestoreDb.Create("ict2112"); // Replace with your Firebase project ID
        }

       public async Task<List<PatientDrugLogModel>> GetDrugLogAsync()
{
    var drugLog = new List<PatientDrugLogModel>();
    
    // Safely access Session using null-conditional
    string patientId = _httpContextAccessor.HttpContext?.Session?.GetString("UserUUID") ?? "";

    try
    {
        var collection = _db.Collection("DrugInformation").WhereEqualTo("PatientId", patientId);
        var snapshot = await collection.GetSnapshotAsync();

        foreach (var document in snapshot.Documents)
        {
            if (document.Exists)
            {
                var drugInfo = document.ConvertTo<PatientDrugLogModel>();
                drugLog.Add(drugInfo);
            }
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error fetching drug log: {e.Message}");
    }

    return drugLog;
}

        public async Task<List<PatientDrugLogModel>> GetAllDrugLogAsync()
        {
            var drugLog = new List<PatientDrugLogModel>();

            try
            {
                var collection = _db.Collection("DrugInformation");
                var snapshot = await collection.GetSnapshotAsync();

                foreach (var document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        var drugInfo = document.ConvertTo<PatientDrugLogModel>();
                        drugLog.Add(drugInfo);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error fetching drug log: {e.Message}");
            }

            return drugLog;
        }

        public async Task UploadDrugInfo(PatientDrugLogModel drugInfo)
        {
            try
            {
                var collection = _db.Collection("DrugInformation");
                await collection.AddAsync(drugInfo);
                Console.WriteLine($"Drug Info added: {drugInfo.DrugName}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error uploading drug information: {e.Message}");
            }
        }
    }
}
