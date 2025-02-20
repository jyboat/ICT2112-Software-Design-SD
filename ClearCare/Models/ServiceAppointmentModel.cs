using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Google.Cloud.Location;
using Newtonsoft.Json;

namespace ClearCare.Models
{
    public class ServiceAppointment
    {
        // Create does not work when properties are private
        // I think it is due to the pass to gateway??? idk..
        [Key]
        [JsonProperty("AppointmentId")]
        public string AppointmentId { get; set; }

        [JsonProperty("PatientId")]
        public string PatientId { get; set; }

        [JsonProperty("NurseId")]
        public string NurseId { get; set; }

        [JsonProperty("DoctorId")]
        public string DoctorId { get; set; }

        [JsonProperty("ServiceTypeId")]
        public string ServiceTypeId { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("DateTime")]
        public DateTime DateTime { get; set; }

        [JsonProperty("Slot")]
        public int Slot { get; set; }

        [JsonProperty("Location")]
        public string Location { get; set; }

        // Private constructor
        public ServiceAppointment() { }

        // Constructor
        public static ServiceAppointment setApptDetails(string appointmentId, string patientId, string nurseId,
            string doctorId, string serviceTypeId, string status, DateTime dateTime, int slot, string location)
        {
            return new ServiceAppointment
            {
                AppointmentId = appointmentId,
                PatientId = patientId,
                NurseId = nurseId,
                DoctorId = doctorId,
                ServiceTypeId = serviceTypeId,
                Status = status,
                DateTime = dateTime,
                Slot = slot,
                Location = location
            };
        }

        // Data Normalization
        // Convert firebase key-value pair into ServiceAppointment Structure so it can be used directly
        // No more key-value but return the object
        // Extracts values from { "PatientId": "USR010", "NurseId": "USR001", .... }
        // and maps them into the ServiceAppointment model
        // ServiceAppointment { PatientId = "USR010", NurseId = "USR001", ... }
        // Simple Domain Model Mapping
        public static ServiceAppointment FromFirestoreData(string appointmentId, Dictionary<string, object> data)
        {
            return new ServiceAppointment
            {
                AppointmentId = appointmentId,
                PatientId = data["PatientId"].ToString(),
                NurseId = data.ContainsKey("NurseId") ? data["NurseId"].ToString() : "",
                DoctorId = data["DoctorId"].ToString(),
                ServiceTypeId = data["ServiceTypeId"].ToString(),
                Status = data["Status"].ToString(),
                DateTime = ((Google.Cloud.Firestore.Timestamp)data["DateTime"]).ToDateTime(),
                Slot = data.ContainsKey("Slot") ? Convert.ToInt32(data["Slot"]) : 0,
                Location = data["Location"].ToString()
            };
        }

        // Convert to Firestore Dictionary format for insertion
        // Acts as a getter for all attributes 
        public Dictionary<string, object> ToFirestoreDictionary()
        {
            return new Dictionary<string, object>
            {
                { "AppointmentId", AppointmentId },
                { "PatientId", PatientId },
                { "NurseId", NurseId },
                { "DoctorId", DoctorId },
                { "ServiceTypeId", ServiceTypeId },
                { "Status", Status },
                { "DateTime", DateTime },
                { "Slot", Slot },
                { "Location", Location }
            };
        }

    }
}
