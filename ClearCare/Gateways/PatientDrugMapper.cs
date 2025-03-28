using ClearCare.Models;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;


namespace ClearCare.Gateways
{
    public class PatientDrugMapper
    {
        private readonly FirestoreDb _db;
        private readonly string _userRole;
        private readonly string _userUUID;

        public PatientDrugMapper(IHttpContextAccessor httpContextAccessor)
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.GetApplicationDefault()
                });
            }

            _db = FirestoreDb.Create("ict2112"); // Replace with your Firebase project ID

            // Retrieve the session values using the provided IHttpContextAccessor.
            _userRole = httpContextAccessor.HttpContext?.Session.GetString("UserRole") ?? "Unknown";
            _userUUID = httpContextAccessor.HttpContext?.Session.GetString("UserUUID") ?? "";
        }

        public async Task<List<PatientDrugLogModel>> getDrugLogAsync()
        {
            var drugLog = new List<PatientDrugLogModel>(); 

            try
            {
                // Use the userUUID retrieved from session
                var collection = _db.Collection("DrugInformation").WhereEqualTo("PatientId", _userUUID);
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

        public async Task<List<PatientDrugLogModel>> getAllDrugLogAsync() {
            var DrugLog = new List<PatientDrugLogModel>(); 

            try {
                var collection = _db.Collection("DrugInformation");
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

        public async Task uploadDrugInfo(PatientDrugLogModel drugInfo)
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
