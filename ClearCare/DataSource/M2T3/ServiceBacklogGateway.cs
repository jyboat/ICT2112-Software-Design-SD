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

        public async Task createServiceBacklog(ServiceBacklog backlog)
        {
            try
            {
                DocumentReference docRef = _dbCollection.Document();

                Dictionary<string, string> backlogInfo = backlog.getBacklogInformation();
                string appointmentId = backlogInfo["appointmentId"];
                
                backlog.setBacklogInformation(docRef.Id, appointmentId);
                Dictionary<string, object> backlogData = BacklogToFirestoreDictionary(backlog);
                Console.WriteLine($"Writing backlog... {docRef.Id},{appointmentId}");

                // Attempt to write to Firestore
                await docRef.SetAsync(backlogData);

                await _receiver.receiveAddStatus("Success");
            }
            catch (Exception ex)
            {
                await _receiver.receiveAddStatus($"Error: {ex.Message}");
            }
        }


        public async Task fetchServiceBacklogs()
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
        }


        public async Task deleteServiceBacklog(int backlogId)
        {
            // TODO trycatch for failure
            Query query = _dbCollection.WhereEqualTo("backlogId", backlogId);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            if (snapshot.Documents.Count == 0)
            {
                await _receiver.receiveDeleteStatus("Failed: availability not found");
                return;
            }

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                await document.Reference.DeleteAsync();
            }

            await _receiver.receiveDeleteStatus("Success");
        }


        public Dictionary<string, object> BacklogToFirestoreDictionary(ServiceBacklog backlog)
        {
            Dictionary<string, string> backlogInfo = backlog.getBacklogInformation();
            return new Dictionary<string, object>
            {
                { "appointmentId", backlogInfo["appointmentId"]},
            };
        }

        
        public Dictionary<string, string> FirestoreToBacklogDictionary(string backlogId, Dictionary<string, object> firestoreObject)
        {
            return new Dictionary<string, string>
            {
                { "backlogId", backlogId},
                { "appointmentId", firestoreObject["appointmentId"]?.ToString() ?? string.Empty},
            };
        }

    }
}
