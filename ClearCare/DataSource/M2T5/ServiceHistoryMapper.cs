using ClearCare.Models.Entities;
using ClearCare.Interfaces;
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
        
                notifyObservers(serviceHistoryList);
                return serviceHistoryList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving service history: {ex.Message}");
                throw;
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

                // Console.WriteLine($"ServiceHistoryMapper.cs");
                // foreach (var ele in serviceHistoryData)
                // {
                //     Console.WriteLine($"Key: {ele.Key}, Value: {ele.Value}");
                // }

                // Overwrite field if exist, creat new if doesn't exist
                await docRef.SetAsync(serviceHistoryData);

                // Console.WriteLine($"Service History created: {serviceHistoryData["ServiceHistoryId"]}");

                notifyObservers(serviceHistory);

                return docRef.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating service history: {ex.Message}");
                throw;
            }
        }

        // TEMPORARY: GET ALL APPTS
        public async Task<List<ServiceAppointment>> fetchAllAppointments(int limit = 10)
        {
            CollectionReference appointmentsRef = _db.Collection("ServiceAppointments");
            Query query = appointmentsRef.Limit(limit); // limit to latest 10 appts
            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            List<ServiceAppointment> appointmentList = new List<ServiceAppointment>();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    var data = document.ToDictionary();
                    ServiceAppointment appointment = ServiceAppointment.FromFirestoreData(document.Id, data);
                    appointmentList.Add(appointment);
                }
            }
            return appointmentList;
        }

        // TEMPORARY: GET APPT BY ID
        public async Task<Dictionary<string, object>> fetchApptById(string apptId)
        {
            DocumentReference docRef = _db.Collection("ServiceAppointments").Document(apptId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Console.WriteLine($"Firestore Document Not Found: {apptId}");
                return null;
            }

            // Convert to Dictionary from firebase key-value format
            var data = snapshot.ToDictionary();
            // var appt = ServiceAppointment.FromFirestoreData(apptId, data).ToFirestoreDictionary();
            
            // await _receiver.receiveServiceAppointmentById(appt);
            return data;
        }

        // TEMPORARY: UPDATE APPT STATUS TO COMPLETED
        public async Task<bool> updateApptStatus(string apptId)
        {
            DocumentReference docRef = _db.Collection("ServiceAppointments").Document(apptId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                return false;
            }

            await docRef.UpdateAsync("Status", "Completed");
            return true;
        }
    }
}