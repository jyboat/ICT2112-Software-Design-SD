using Google.Cloud.Firestore;
using ClearCare.Models.Entities;
using ClearCare.Interfaces;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ClearCare.DataSource
{
    public class NurseAvailabilityGateway : IAvailabilityDB_Send
    {
        private readonly FirestoreDb _db;

        public NurseAvailabilityGateway()
        {
            _db = FirebaseService.Initialize();
        }

        // Retrieve ALL Nurse Availabilities
        public List<NurseAvailability> retrieveAllStaffAvailability()
        {
            List<NurseAvailability> availabilityList = new List<NurseAvailability>();

            CollectionReference availabilitiesRef = _db.Collection("NurseAvailability");
            QuerySnapshot snapshot = availabilitiesRef.GetSnapshotAsync().Result; // Using .Result to keep it synchronous

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    var data = document.ToDictionary();
                    NurseAvailability availability = NurseAvailability.FromFirestoreData(data);
                    availabilityList.Add(availability);
                }
            }
            return availabilityList;
        }

        // Retrieve Availability by Staff ID
        public List<NurseAvailability> retrieveAvailabilityByStaff(string staffId)
        {
            List<NurseAvailability> availabilityList = new List<NurseAvailability>();

            CollectionReference availabilitiesRef = _db.Collection("NurseAvailability");
            Query query = availabilitiesRef
                .WhereEqualTo("nurseID", staffId)
                .OrderByDescending("availabilityId");

            QuerySnapshot snapshot = query.GetSnapshotAsync().Result;

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    var data = document.ToDictionary();
                    NurseAvailability availability = NurseAvailability.FromFirestoreData(data);
                    availabilityList.Add(availability);
                }
            }
            return availabilityList;
        }

        // Add Availability
        public void createAvailability(NurseAvailability availability)
        {
            DocumentReference docRef = _db.Collection("NurseAvailability").Document();
            docRef.SetAsync(availability.getAvailabilityDetails()).Wait(); 
        }

        // Update Availability
        public void modifyAvailability(NurseAvailability availability)
        {
            CollectionReference availabilitiesRef = _db.Collection("NurseAvailability");
            Query query = availabilitiesRef.WhereEqualTo("availabilityId", availability.getAvailabilityDetails()["availabilityId"]);

            QuerySnapshot snapshot = query.GetSnapshotAsync().Result;

            if (snapshot.Documents.Count == 0)
            {
                return;
            }

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Dictionary<string, object> availabilityData = availability.getAvailabilityDetails();
                document.Reference.SetAsync(availabilityData, SetOptions.MergeAll).Wait(); // Making it synchronous
            }
        }

        // Delete Availability
        public void removeAvailability(int availabilityId)
        {
            CollectionReference availabilitiesRef = _db.Collection("NurseAvailability");
            Query query = availabilitiesRef.WhereEqualTo("availabilityId", availabilityId);
            QuerySnapshot snapshot = query.GetSnapshotAsync().Result;

            if (snapshot.Documents.Count == 0)
            {
                return;
            }

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                document.Reference.DeleteAsync().Wait(); 
            }
        }
    }
}
