using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities
{
    [FirestoreData] 
    public class NurseAvailability
    {
        [Key]
        [FirestoreProperty]
        private int AvailabilityId { get; set; }

        [FirestoreProperty]
        private string NurseID { get; set; } = "USR003"; // Dummy NurseID for testing

        [FirestoreProperty]
        private string Date { get; set; } = string.Empty;

        [FirestoreProperty]
        private string StartTime { get; set; } = "08:00:00";

       [FirestoreProperty]
        private string EndTime { get; set; } = "16:00:00";

    
        // 🔹 Private Getter Methods (Encapsulation)
        private int GetAvailabilityId() => AvailabilityId;
        private string GetNurseId() => NurseID;
        private string GetDate() => Date;
        private string GetStartTime() => StartTime;
        private string GetEndTime() => EndTime;

        // 🔹 Private Setter Methods
        private void SetAvailabilityId(int availabilityId) => AvailabilityId = availabilityId;
        private void SetNurseId(string nurseId) => NurseID = nurseId;
        private void SetDate(string date) => Date = date;
        private void SetStartTime(string startTime) => StartTime = startTime;
        private void SetEndTime(string endTime) => EndTime = endTime;


        // Public function to set availability details
        public static NurseAvailability SetAvailabilityDetails(int availabilityId, string nurseID, string date, string startTime, string endTime)
        {
            return new NurseAvailability
            {
                AvailabilityId = availabilityId,
                NurseID = nurseID,
                Date = date,
                StartTime = startTime,
                EndTime = endTime
            };
        }

        // Public function to retrieve details
        public Dictionary<string, object> GetAvailabilityDetails()
        {
            return new Dictionary<string, object>
            {
                { "availabilityId", AvailabilityId },
                { "nurseID", NurseID },
                { "date", Date },
                { "startTime", StartTime },
                { "endTime", EndTime }
            };
        }
    }
}
