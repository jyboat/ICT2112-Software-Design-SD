using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ClearCare.Models
{
    public class NurseAvailability
    {
        [Key]
        [JsonProperty("availabilityId")]
        private int AvailabilityId { get; set; }

        [JsonProperty("nurseID")]
        private string NurseID { get; set; }

        [JsonProperty("date")]
        private string Date { get; set; }

        [JsonProperty("startTime")]
        private string StartTime { get; set; }

        [JsonProperty("endTime")]
        private string EndTime { get; set; }

        // Constructor
        public NurseAvailability() 
        { 
            NurseID = "USR003"; 
        }

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
