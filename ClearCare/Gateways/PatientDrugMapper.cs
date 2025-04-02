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

        /// <summary>
        ///   Initializes a new instance of the <see cref="PatientDrugMapper"/>
        ///   class.
        /// </summary>
        /// <param name="httpContextAccessor">
        ///   The HttpContextAccessor for accessing session data.
        /// </param>
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

            _db = FirestoreDb.Create(
                "ict2112"
            ); // Replace with your Firebase project ID
        }

        /// <summary>
        ///   Retrieves the drug log for the current patient.
        /// </summary>
        /// <returns>
        ///   A list of <see cref="PatientDrugLogModel"/> representing the
        ///   patient's drug log.
        /// </returns>
        public async Task<List<PatientDrugLogModel>> GetDrugLogAsync()
        {
            var drugLog = new List<PatientDrugLogModel>();

            // Safely access Session using null-conditional
            string patientId =
                _httpContextAccessor.HttpContext?.Session?.GetString("UserUUID") ?? "";

            try
            {
                var collection = _db.Collection("DrugInformation")
                    .WhereEqualTo("PatientId", patientId);
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

        /// <summary>
        ///   Retrieves all drug logs from Firestore.
        /// </summary>
        /// <returns>
        ///   A list of <see cref="PatientDrugLogModel"/> representing all drug
        ///   logs.
        /// </returns>
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

        /// <summary>
        ///   Uploads drug information to Firestore.
        /// </summary>
        /// <param name="drugInfo">The <see cref="PatientDrugLogModel"/> to
        ///   upload.</param>
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
