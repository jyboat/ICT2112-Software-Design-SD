using ClearCare.Models.Entities.M3T2;
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

            _db = FirebaseService.Initialize();
        }


        public async Task<List<PatientDrugLog>> GetDrugLogAsync()
        {
            /// <summary>
            ///   Retrieves the drug log for the current patient.
            /// </summary>
            /// <returns>
            ///   A list of <see cref="PatientDrugLogModel"/> representing the
            ///   patient's drug log.
            /// </returns>
            var drugLog = new List<PatientDrugLog>();

            // Safely access Session using null-conditional
            string patientId =
                _httpContextAccessor.HttpContext?.Session?.GetString("UserID") ?? "";

            try
            {
                var collection = _db.Collection("DrugInformation")
                    .WhereEqualTo("PatientId", patientId);
                var snapshot = await collection.GetSnapshotAsync();

                foreach (var document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        var drugInfo = document.ConvertTo<PatientDrugLog>();
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

        public async Task<List<PatientDrugLog>> GetAllDrugLogAsync()
        /// <summary>
        ///   Retrieves all drug logs from Firestore.
        /// </summary>
        /// <returns>
        ///   A list of <see cref="PatientDrugLogModel"/> representing all drug
        ///   logs.
        /// </returns>
        {
            var drugLog = new List<PatientDrugLog>();

            try
            {
                var collection = _db.Collection("DrugInformation");
                var snapshot = await collection.GetSnapshotAsync();

                foreach (var document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        var drugInfo = document.ConvertTo<PatientDrugLog>();
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

        public async Task UploadDrugInfo(PatientDrugLog drugInfo)
        /// <summary>
        ///   Uploads drug information to Firestore.
        /// </summary>
        /// <param name="drugInfo">The <see cref="PatientDrugLogModel"/> to
        ///   upload.</param>
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
