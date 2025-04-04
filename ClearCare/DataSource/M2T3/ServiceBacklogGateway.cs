using Google.Cloud.Firestore;
using ClearCare.Models.Entities;
using ClearCare.Interfaces;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO.Pipes;
using ClearCare.Models.Control;

namespace ClearCare.DataSource
{
    public class ServiceBacklogGateway : IServiceBacklogDB_Send
    {
        private readonly FirestoreDb _db;
        private readonly CollectionReference _dbCollection;
        private IServiceBacklogDB_Receive _receiver;
        public IServiceBacklogDB_Receive Receiver
        {
            get { return _receiver; }
            set { _receiver = value; }
        }

        public ServiceBacklogGateway(IServiceBacklogDB_Receive receiver)
        {
            _db = FirebaseService.Initialize();
            _dbCollection = _db.Collection("ServiceBacklogs");
            Receiver = receiver;
        }
        
        // CREATE BACKLOG
        public async Task createServiceBacklog(ServiceBacklog backlog)
        {
            try
            {
                DocumentReference docRef = _dbCollection.Document();

                Dictionary<string, string> backlogInfo = backlog.getBacklogInformation();
                string appointmentId = backlogInfo["appointmentId"];
                
                backlog.setBacklogInformation(docRef.Id, appointmentId);
                Dictionary<string, object> backlogData = BacklogToFirestoreDictionary(backlog);

                // Attempt to write to Firestore
                await docRef.SetAsync(backlogData);

                await _receiver.receiveAddStatus("Success");
            }
            catch (Exception ex)
            {
                await _receiver.receiveAddStatus($"Error: {ex.Message}");
            }
        }

        // GET ALL BACKLOGS
        public async Task<List<Dictionary<string,string>>> fetchServiceBacklogs()
        {
            QuerySnapshot snapshot = await _dbCollection.GetSnapshotAsync();

            List<Dictionary<string, string>> backlogList = new List<Dictionary<string, string>>();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    var id = document.Id;
                    var data = document.ToDictionary();

                    Dictionary<string, string> backlog = FirestoreToBacklogDictionary(id, data);
                    backlogList.Add(backlog);
                }
            }
            await _receiver.receiveBacklogList(backlogList);
            return backlogList;
        }
        
        // GET ONE SERVICE BACKLOG BASED ON ID
        public async Task<Dictionary<string, string>> fetchServiceBacklogById(string backlogId)
        {
            DocumentReference docRef = _dbCollection.Document(backlogId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            var data = snapshot.ToDictionary();
            Dictionary<string, string> backlog = FirestoreToBacklogDictionary(snapshot.Id, data);

            await _receiver.receiveBacklogDetails(backlog);
            return backlog;
        }

        public async Task<bool> deleteServiceBacklog(string backlogId)
        {
            DocumentReference docRef = _dbCollection.Document(backlogId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                await _receiver.receiveDeleteStatus("Failed: backlog not found");
                return false;
            }

            await docRef.DeleteAsync();
            await _receiver.receiveDeleteStatus("Success");
            return true;
        }

        public Dictionary<string, string> FirestoreToBacklogDictionary(string backlogId, Dictionary<string, object> firestoreObject)
        {
            return new Dictionary<string, string>
            {
                { "backlogId", backlogId},
                { "appointmentId", firestoreObject["appointmentId"]?.ToString() ?? string.Empty},
            };
        }
        public Dictionary<string, object> BacklogToFirestoreDictionary(ServiceBacklog backlog)
        {
            Dictionary<string, string> backlogInfo = backlog.getBacklogInformation();
            return new Dictionary<string, object>
            {
                { "appointmentId", backlogInfo["appointmentId"]},
            };
        }
    }
}