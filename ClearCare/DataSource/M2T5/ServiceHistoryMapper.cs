using ClearCare.Models.Entities;
using Google.Cloud.Firestore;

namespace ClearCare.DataSource
{
    public class ServiceHistoryMapper : AbstractDatabaseSubject
    {
        private readonly FirestoreDb _db;
        private readonly CollectionReference _dbCollection;

        public ServiceHistoryMapper()
        {
            _db = FirebaseService.Initialize();
            _dbCollection = _db.Collection("ServiceHistory");
        }

        // GET ALL SERVICE HISTORY
        public async Task fetchAllServiceHistory()
        {
            QuerySnapshot snapshot = await _dbCollection.GetSnapshotAsync();
            List<ServiceHistory> serviceHistoryList = new List<ServiceHistory>();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    var data = document.ToDictionary();
                    var serviceHistory = fromFirestoreData(document.Id, data);
                    serviceHistoryList.Add(serviceHistory);
                }
            }
            notifyObservers(serviceHistoryList);
        }

        // CREATE SERVICE HISTORY
        public async Task<string> createServiceHistory(ServiceHistory serviceHistory)
        {
            try
            {
                DocumentReference docRef = _dbCollection.Document();

                serviceHistory.updateServiceHistoryId(docRef.Id);

                // Convert input data to Firestore data format for insert
                Dictionary<string, object> serviceHistoryData = serviceHistory.getServiceHistoryDetails();

                // Overwrite field if exist, creat new if doesn't exist
                await docRef.SetAsync(serviceHistoryData);

                return docRef.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating service history: {ex.Message}");
                return null;
            }
        }

        // Data Normalization
        // Convert firebase key-value pair into ServiceHistory Structure so it can be used directly
        // No more key-value but return the object
        // Extracts values from { "PatientId": "USR010", "NurseId": "USR001", .... }
        // and maps them into the ServiceHistory model
        // ServiceHistory { PatientId = "USR010", NurseId = "USR001", ... }
        // Rich Domain Model Mapping
        public static ServiceHistory fromFirestoreData(string serviceHistoryId, Dictionary<string, object> data)
        {   
            string appointmentId = data["AppointmentId"].ToString() ?? "";
            string service = data["Service"].ToString() ?? "";
            string patientId = data["PatientId"].ToString() ?? "";
            string nurseId = data.ContainsKey("NurseId") ? data["NurseId"].ToString() ?? "" : "" ;
            string doctorId = data["DoctorId"].ToString() ?? "";
            DateTime serviceDate = ((Timestamp)data["ServiceDate"]).ToDateTime().ToLocalTime();
            string location = data["Location"].ToString()  ?? "";
            string serviceOutcomes = data["ServiceOutcomes"].ToString()  ?? "";

            ServiceHistory serviceHistory = ServiceHistory.setServiceHistoryDetails(appointmentId, service, patientId, nurseId, doctorId, serviceDate, location, serviceOutcomes);
            return serviceHistory;
        }
    }
}