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
        private string NurseID { get; set; } = string.Empty;
        [FirestoreProperty]
        private string Date { get; set; } = string.Empty;
        [FirestoreProperty]
        private string StartTime { get; set; } = "08:00:00";
        [FirestoreProperty]
        private string EndTime { get; set; } = "16:00:00";

        // 🔹 Private Getters and Setters (Encapsulation)
        private int getAvailabilityId() => AvailabilityId;
        private void setAvailabilityId(int availabilityId) => AvailabilityId = availabilityId;
        private string getNurseId() => NurseID;
        private void setNurseId(string nurseId) => NurseID = nurseId;
        private string getDate() => Date;
        private void setDate(string date) => Date = date;
        private string getStartTime() => StartTime;
        private void setStartTime(string startTime) => StartTime = startTime;
        private string getEndTime() => EndTime;
        private void setEndTime(string endTime) => EndTime = endTime;

        // Public function to set availability details
        public static NurseAvailability setAvailabilityDetails(int availabilityId, string nurseID, string date, string startTime, string endTime)
        {
            NurseAvailability availability = new NurseAvailability();
            availability.setAvailabilityId(availabilityId);
            availability.setNurseId(nurseID);
            availability.setDate(date);
            availability.setStartTime(startTime);
            availability.setEndTime(endTime);

            return availability;
        }

        // Public function to retrieve details
        public Dictionary<string, object> getAvailabilityDetails()
        {
            return new Dictionary<string, object>
            {
                { "availabilityId", getAvailabilityId() },
                { "nurseID", getNurseId() },
                { "date", getDate() },
                { "startTime", getStartTime() },
                { "endTime", getEndTime() }
            };
        }

        public static NurseAvailability FromFirestoreData(Dictionary<string, object> data)
        {
            NurseAvailability availability = new NurseAvailability();

            availability.AvailabilityId = data.ContainsKey("availabilityId") ? Convert.ToInt32(data["availabilityId"]) : 0;
            availability.NurseID = data.ContainsKey("nurseID") ? data["nurseID"].ToString() : "";
            availability.Date = data.ContainsKey("date") ? data["date"].ToString() : "";
            availability.StartTime = data.ContainsKey("startTime") ? data["startTime"].ToString() : "08:00:00";
            availability.EndTime = data.ContainsKey("endTime") ? data["endTime"].ToString() : "16:00:00";

            return availability;
        }
    }
}