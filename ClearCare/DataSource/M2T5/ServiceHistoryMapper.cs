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
        public async Task<List<ServiceHistory>> fetchAllServiceHistory()
        {
            try
            {
                QuerySnapshot snapshot = await _dbCollection.GetSnapshotAsync();
                List<ServiceHistory> serviceHistoryList = new List<ServiceHistory>();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        var data = document.ToDictionary();
                        ServiceHistory serviceHistory = ServiceHistory.FromFirestoreData(document.Id, data);
                        serviceHistoryList.Add(serviceHistory);
                    }
                }
        
                return serviceHistoryList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ServiceHistoryMapper] Error retrieving service history: {ex.Message}");
                return null;
            }
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

                notifyObservers(true);
                return docRef.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating service history: {ex.Message}");
                notifyObservers(false);
                return null;
            }
        }
    }
}