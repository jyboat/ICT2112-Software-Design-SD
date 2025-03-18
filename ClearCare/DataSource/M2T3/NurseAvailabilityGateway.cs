using Google.Cloud.Firestore;
using ClearCare.Models.Entities;
using ClearCare.Interfaces;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ClearCare.DataSource
{
    public class NurseAvailabilityGateway : IAvailabilityDB_Send
    {
        private readonly FirestoreDb _db;
        private IAvailabilityDB_Receive _receiver;

        public NurseAvailabilityGateway()
        {
            _db = FirebaseService.Initialize();
        }

        // Property for setting the receiver after instantiation (Since gateway handle receiver callback - creates circular dependency. SO need break cycle by property injection)
        public IAvailabilityDB_Receive Receiver
        {
            get { return _receiver; }
            set { _receiver = value; }
        }


        // Implementing IAvailabilityDB_Send Interfaces

        // Retrieve ALL Nurse Availabilities - implemented in IAvailabilityDB_Send; used in NurseAvailabilityManagement (getAllStaffAvailability)
        public async Task<List<NurseAvailability>> fetchAllStaffAvailability()
        {
            List<NurseAvailability> availabilityList = new List<NurseAvailability>();

            DateTime currentDate = DateTime.Now; // Gets the current date and time
            var currentDateString = currentDate.ToString("yyyy-MM-dd");

            Query availabilitiesQuery = _db.Collection("NurseAvailability")
                                       .WhereEqualTo("date", currentDateString);
            QuerySnapshot snapshot = await availabilitiesQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    var data = document.ToDictionary();
                    NurseAvailability availability = NurseAvailability.FromFirestoreData(data);
                    availabilityList.Add(availability);
                }
            }
            await _receiver.receiveAvailabilityList(availabilityList);
            return availabilityList;
        }

        // Retrieve Availability by Staff ID - implemented in IAvailabilityDB_Send; used in NurseAvailabilityManagement (getAvailabilityByStaff)
        public async Task<List<NurseAvailability>> fetchAvailabilityByStaff(string staffId)
        {
            List<NurseAvailability> availabilityList = new List<NurseAvailability>();

            CollectionReference availabilitiesRef = _db.Collection("NurseAvailability");
            Query query = availabilitiesRef
                .WhereEqualTo("nurseID", staffId)
                .OrderByDescending("availabilityId");

            await Task.Delay(1000);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    var data = document.ToDictionary();
                    NurseAvailability availability = NurseAvailability.FromFirestoreData(data);
                    availabilityList.Add(availability);
                }
            }
            await _receiver.receiveAvailabilityList(availabilityList);
            return availabilityList;
        }

        // Add Availability - implemented in IAvailabilityDB_Send; used in NurseAvailabilityManagement (addAvailability)
        public async Task createAvailability(NurseAvailability availability)
        {
            DocumentReference docRef = _db.Collection("NurseAvailability").Document();
            await docRef.SetAsync(availability.getAvailabilityDetails());
            await _receiver.receiveAddStatus("Success");
        }

        // Update Availability - implemented in IAvailabilityDB_Send; used in NurseAvailabilityManagement (updateAvailability)
        public async Task modifyAvailability(NurseAvailability availability)
        {
            CollectionReference availabilitiesRef = _db.Collection("NurseAvailability");
            Query query = availabilitiesRef.WhereEqualTo("availabilityId", availability.getAvailabilityDetails()["availabilityId"]);

            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            if (snapshot.Documents.Count == 0)
            {
                await _receiver.receiveUpdateStatus("Failed: availability not found");
                return;
            }

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Dictionary<string, object> availabilityData = availability.getAvailabilityDetails();
                await document.Reference.SetAsync(availabilityData, SetOptions.MergeAll);
            }

           await _receiver.receiveUpdateStatus("Success");
        }

        // Delete Availability - implemented in IAvailabilityDB_Send; used in NurseAvailabilityManagement (deleteAvailability)
        public async Task removeAvailability(int availabilityId)
        {
            CollectionReference availabilitiesRef = _db.Collection("NurseAvailability");
            Query query = availabilitiesRef.WhereEqualTo("availabilityId", availabilityId);
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
    }
}
