using Google.Cloud.Firestore;
using ClearCare.Models.Entities;
using ClearCare.Interfaces;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ClearCare.DataSource
{
    public class NurseAvailabilityGateway : INurseAvailability
    {
        private readonly FirestoreDb _db;

        public NurseAvailabilityGateway()
        {
            // Initialize Firebase
            _db = FirebaseService.Initialize();
        }

        // Retrieve Nurse Availability
        public async Task<List<NurseAvailability>> GetAvailabilityByStaffAsync(string staffId)
        {
            // Console.WriteLine($"Fetching availability for Nurse ID: {staffId}");

            CollectionReference availabilitiesRef = _db.Collection("NurseAvailability");
            Query query = availabilitiesRef
                .WhereEqualTo("nurseID", staffId)
                .OrderByDescending("availabilityId");
            // Small delay to allow Firestore sync
            await Task.Delay(1000);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            List<NurseAvailability> availabilityList = new List<NurseAvailability>();

            // Console.WriteLine($"Found {snapshot.Documents.Count} documents for Nurse ID: {staffId}");

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    var data = document.ToDictionary();
                    // Console.WriteLine($"Found document: {document.Id} → {JsonConvert.SerializeObject(data, Formatting.Indented)}");

                    int availabilityId = Convert.ToInt32(data["availabilityId"]);
                    string nurseID = data["nurseID"].ToString();
                    string dateStr = data["date"].ToString();
                    string startTimeStr = data["startTime"].ToString();
                    string endTimeStr = data["endTime"].ToString();

                    NurseAvailability availability = NurseAvailability.SetAvailabilityDetails(
                        availabilityId,
                        nurseID,
                        dateStr,
                        startTimeStr,
                        endTimeStr
                    );

                    availabilityList.Add(availability);
                }
            }

            return availabilityList;
        }

        // Fetch Next Availability ID (Start from 10)
        public async Task<int> GetNextAvailabilityIdAsync()
        {
            CollectionReference availabilitiesRef = _db.Collection("NurseAvailability");
            QuerySnapshot snapshot = await availabilitiesRef.GetSnapshotAsync();

            int maxId = 9;
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists && document.ContainsField("availabilityId"))
                {
                    int currentId = Convert.ToInt32(document.GetValue<int>("availabilityId"));
                    if (currentId > maxId)
                    {
                        maxId = currentId;
                    }
                }
            }
            return maxId + 1;
        }

        // Add Availability
        public async Task AddAvailabilityAsync(NurseAvailability availability)
        {
            DocumentReference docRef = _db.Collection("NurseAvailability").Document();
            await docRef.SetAsync(availability.GetAvailabilityDetails());
        }

        // Update Availability
        public async Task UpdateAvailabilityAsync(NurseAvailability availability)
        {
            CollectionReference availabilitiesRef = _db.Collection("NurseAvailability");

            Query query = availabilitiesRef.WhereEqualTo("availabilityId", availability.GetAvailabilityDetails()["availabilityId"]);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            if (snapshot.Documents.Count == 0)
            {
                // Console.WriteLine($"No document found with availabilityId: {availability.GetAvailabilityDetails()["availabilityId"]}");
                return;
            }

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                // Console.WriteLine($"Updating document {document.Id} with availabilityId: {availability.GetAvailabilityDetails()["availabilityId"]}");

                Dictionary<string, object> availabilityData = new Dictionary<string, object>
        {
            { "availabilityId", availability.GetAvailabilityDetails()["availabilityId"] },
            { "nurseID", availability.GetAvailabilityDetails()["nurseID"] },
            { "date", availability.GetAvailabilityDetails()["date"] },
            { "startTime", availability.GetAvailabilityDetails()["startTime"] },
            { "endTime", availability.GetAvailabilityDetails()["endTime"] }
        };

                await document.Reference.SetAsync(availabilityData, SetOptions.MergeAll);
            }
        }

        // Delete Availability
        public async Task DeleteAvailabilityAsync(int availabilityId)
        {
            CollectionReference availabilitiesRef = _db.Collection("NurseAvailability");

            Query query = availabilitiesRef.WhereEqualTo("availabilityId", availabilityId);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            if (snapshot.Documents.Count == 0)
            {
                // Console.WriteLine($"No document found with availabilityId: {availabilityId}");
                return;
            }

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                // Console.WriteLine($"Deleting document {document.Id} with availabilityId: {availabilityId}");
                await document.Reference.DeleteAsync();
            }
        }
    }
}
