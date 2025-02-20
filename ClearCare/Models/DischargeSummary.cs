using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Google.Cloud.Location;
using Newtonsoft.Json;

namespace ClearCare.Models
{
    public class DischargeSummary
    {
        private DischargeSummary() {}
        private string _id { get; set; }
        private string _details { get; set; }
        private string _instructions { get; set; }
        private DateTime _createdAt { get; set; }
        private string _patientId { get; set; }

        public void setId(string id)
        {
            _id = id;
        }

        public static DischargeSummary setDetails(string id, string details, string instructions, DateTime createdAt, string patientId){
            return new DischargeSummary
            {
                _id = id,
                _details = details,
                _instructions = instructions,
                _createdAt = createdAt,
                _patientId = patientId
            };
        }

        public Dictionary<string, object> getDetails(){
            return new Dictionary<string, object>{
                {"id", _id},
                {"patientId", _patientId},
                {"details", _details},
                {"instructions", _instructions},
                {"createdAt", _createdAt}
            };
        }

        public static DischargeSummary FromFirestoreData(string id, Dictionary<string, object> data)
        {
            return new DischargeSummary
            {
                _id = id,
                _patientId = data["patientId"].ToString() ?? "",
                _details = data["details"].ToString() ?? "",
                _instructions = data["instructions"].ToString() ?? "",
                _createdAt = ((Google.Cloud.Firestore.Timestamp)data["createdAt"]).ToDateTime()
            };
        }
    }
}