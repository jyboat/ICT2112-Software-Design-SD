using ClearCare.Models.DTO.M3T2;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.DataSource.M3T2
{
    public class PatientDrugMapper
    {
        private readonly FirestoreDb _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientDrugMapper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            _db = FirebaseService.Initialize();
        }

        public async Task<List<PatientDrugLogDTO>> GetDrugLogAsync()
        {
            var drugLog = new List<PatientDrugLogDTO>();

            // Retrieve the patient ID from session.
            string patientId = _httpContextAccessor.HttpContext.Session.GetString("UserUUID") ?? "";
            try
            {
                var collection = _db.Collection("DrugInformation").WhereEqualTo("PatientId", patientId);
                var snapshot = await collection.GetSnapshotAsync();

                foreach (var document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        var drugInfo = document.ConvertTo<PatientDrugLogDTO>();
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

        public async Task<List<PatientDrugLogDTO>> GetAllDrugLogAsync()
        {
            var drugLog = new List<PatientDrugLogDTO>();

            try
            {
                var collection = _db.Collection("DrugInformation");
                var snapshot = await collection.GetSnapshotAsync();

                foreach (var document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        var drugInfo = document.ConvertTo<PatientDrugLogDTO>();
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

        public async Task UploadDrugInfo(PatientDrugLogDTO drugInfo)
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
