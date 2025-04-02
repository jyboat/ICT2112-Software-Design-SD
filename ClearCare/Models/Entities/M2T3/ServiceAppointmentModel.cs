using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Google.Cloud.Location;
using Newtonsoft.Json;
using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities
{
    [FirestoreData]
    public class ServiceAppointment
    {
        // string.Empty = set empty default or else it will throw error FCK U ASP.NET
        // [FirestoreProperty]
        private string AppointmentId { get; set; } = string.Empty;

        [FirestoreProperty]
        private string PatientId { get; set; } = string.Empty;

        [FirestoreProperty]
        private string NurseId { get; set; } = string.Empty;

        [FirestoreProperty]
        private string DoctorId { get; set; } = string.Empty;

        [FirestoreProperty]
        private string Service { get; set; } = string.Empty;

        [FirestoreProperty]
        private string Status { get; set; } = string.Empty;

        [FirestoreProperty]
        private DateTime DateTime { get; set; }

        [FirestoreProperty]
        private int Slot { get; set; } = 0; 

        [FirestoreProperty]
        private string Location { get; set; } = string.Empty;

        // Getter and Setter
        private string GetAppointmentID() => AppointmentId;
        private string GetPatientID() => PatientId;
        private string GetNurseID() => NurseId;
        private string GetDoctorID() => DoctorId;
        private string GetServiceType() => Service;
        private string GetStatus() => Status;
        private DateTime GetDateTime() => DateTime; 
        private int GetSlot() => Slot;
        private string GetLocation() => Location;

        // private void SetAppointmentID(string appointmentId) => AppointmentId = appointmentId;
        private void SetPatientID(string patientId) => PatientId = patientId;
        private void SetNurseID(string nurseId) => NurseId = nurseId;
        private void SetDoctorID(string doctorId) => DoctorId = doctorId;
        private void SetServiceId(string Service) => Service = Service;
        private void SetStatus(string status) => Status = status;
        public void SetDateTime(DateTime dateTime) => DateTime = dateTime;
        private void SetSlot(int slot) => Slot = slot;
        private void SetLocation(string location) => Location = location;

        public void appointNurseToPatient(string nurseId, int slot){
            SetNurseID(nurseId);
            SetSlot(slot);
        }

        public string GetAttribute(string attributeName)
        {
            return attributeName switch
            {
                "AppointmentId" => GetAppointmentID(),
                "PatientId" => GetPatientID(),
                "NurseId" => GetNurseID(),
                "DoctorId" => GetDoctorID(),
                "Service" => GetServiceType(),
                "Status" => GetStatus(),
                "Datetime" => GetDateTime().ToString(),
                "Slot" => GetSlot().ToString(),
                "Location" => GetLocation(),
                _ => throw new ArgumentException("Invalid attribute name")
            };
        }

        public int GetIntAttribute(string attributeName)
        {
            return attributeName switch
            {
                "Slot" => GetSlot(),
                _ => throw new ArgumentException("Invalid integer attribute name")
            };
        }

        public DateTime GetAppointmentDateTime (ServiceAppointment appointment) {
            // Ensure DateTime is properly converted
            // const string DateTimeFormat = "d/M/yyyy h:mm:ss tt";

            // DateTime localTime = DateTime.ParseExact(
            //     appointment.GetAttribute("Datetime"),  // Ensure correct key
            //     DateTimeFormat,
            //     System.Globalization.CultureInfo.InvariantCulture,
            //     System.Globalization.DateTimeStyles.AdjustToUniversal // Ensures UTC conversion
            // );

            // return localTime;
            return GetDateTime();

        }

        public ServiceAppointment updateServiceAppointementById (ServiceAppointment appointment, string patientId, string nurseId,
            string doctorId, string Service, string status, DateTime dateTime, int slot, string location) {
                appointment.SetPatientID(patientId);
                appointment.SetNurseID(nurseId);
                appointment.SetDoctorID(doctorId);
                appointment.SetServiceId(Service);
                appointment.SetStatus(status);
                appointment.SetDateTime(dateTime);
                appointment.SetSlot(slot);
                appointment.SetLocation(location);
            return appointment;
        }
        
        public static ServiceAppointment setApptDetails(string patientId, string nurseId,
            string doctorId, string Service, string status, DateTime dateTime, int slot, string location)
        {
            return new ServiceAppointment
            {
                // AppointmentId = appointmentId,
                PatientId = patientId,
                NurseId = nurseId,
                DoctorId = doctorId,
                Service = Service,
                Status = status,
                DateTime = dateTime,
                Slot = slot,
                Location = location
            };
        }

        public Dictionary<string, object> GetApptDetails()
        {
            return new Dictionary<string, object>
            {
                { "PatientID", GetPatientID() },
                // { "AppointmentID", GetAppointmentID() },
                { "NurseID", GetNurseID() },
                { "DoctorID", GetDoctorID() },
                { "ServiceType", GetServiceType() },
                { "Status", GetStatus() },
                { "DateTime", GetDateTime() },
                { "Slot", GetSlot() },
                { "Location", GetLocation() }
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
            // Check if the DateTime field is already a DateTime or a Firestore Timestamp
            object dateTimeValue = data["DateTime"];
            DateTime appointmentDateTime;

            if (dateTimeValue is DateTime)
            {
                // If it's already a DateTime, use it directly
                appointmentDateTime = (DateTime)dateTimeValue;
            }
            else if (dateTimeValue is Google.Cloud.Firestore.Timestamp)
            {
                // If it's a Firestore Timestamp, convert it to DateTime
                appointmentDateTime = ((Google.Cloud.Firestore.Timestamp)dateTimeValue).ToDateTime();
            }
            else
            {
                // Handle the case where DateTime is missing or has an unexpected type
                throw new InvalidCastException("Unexpected DateTime type in Firestore data.");
            }

            return new ServiceAppointment
            {
                AppointmentId = appointmentId,
                PatientId = data["PatientId"].ToString() ?? "",
                NurseId = data.ContainsKey("NurseId") ? data["NurseId"].ToString() ?? "" : "" ,
                DoctorId = data["DoctorId"].ToString() ?? "",
                Service = data["Service"].ToString() ?? "",
                Status = data["Status"].ToString() ?? "",
                DateTime = appointmentDateTime,
                Slot = data.ContainsKey("Slot") ? Convert.ToInt32(data["Slot"]) : 0,
                Location = data["Location"].ToString()  ?? ""
            };
        }

        // Convert to Firestore Dictionary format for insertion
        // Acts as a getter for all attributes 
        public Dictionary<string, object> ToFirestoreDictionary()
        {
            return new Dictionary<string, object>
            {
                // { "AppointmentId", AppointmentId },
                { "PatientId", PatientId },
                { "NurseId", NurseId },
                { "DoctorId", DoctorId },
                { "Service", Service },
                { "Status", Status },
                { "DateTime", DateTime },
                { "Slot", Slot },
                { "Location", Location }
            };
        }
        public void UpdateStatus(string newStatus)
        {
            if (!string.IsNullOrWhiteSpace(newStatus))
            {
                Status = newStatus;
            }
        }

        // public bool CheckAndMarkAsMissed()
        // {
         
        //     if (Status != "Completed" && DateTime < DateTime.Now && Status != "Missed")
        //     {
        //         UpdateStatus("Missed");
        //         return true; 
        //     }
        //     return false;
        // }

    }


}
