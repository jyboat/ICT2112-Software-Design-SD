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
        [FirestoreProperty]
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
        private string getAppointmentID() => AppointmentId;
        private string getPatientID() => PatientId;
        private string getNurseID() => NurseId;
        private string getDoctorID() => DoctorId;
        private string getServiceType() => Service;
        private string getStatus() => Status;
        private DateTime getDateTime() => DateTime; 
        private int getSlot() => Slot;
        private string getLocation() => Location;

        private void SetAppointmentID(string appointmentId) => AppointmentId = appointmentId;
        private void setPatientID(string patientId) => PatientId = patientId;
        private void setNurseID(string nurseId) => NurseId = nurseId;
        private void setDoctorID(string doctorId) => DoctorId = doctorId;
        private void setServiceId(string Service) => Service = Service;
        private void setStatus(string status) => Status = status;
        public void setDateTime(DateTime dateTime) => DateTime = dateTime;
        private void setSlot(int slot) => Slot = slot;
        private void setLocation(string location) => Location = location;

        public void appointNurseToPatient(string nurseId, int slot){
            setNurseID(nurseId);
            setSlot(slot);
        }

        public string getAttribute(string attributeName)
        {
            return attributeName switch
            {
                "AppointmentId" => getAppointmentID(),
                "PatientId" => getPatientID(),
                "NurseId" => getNurseID(),
                "DoctorId" => getDoctorID(),
                "Service" => getServiceType(),
                "Status" => getStatus(),
                "Datetime" => getDateTime().ToString(),
                "Slot" => getSlot().ToString(),
                "Location" => getLocation(),
                _ => throw new ArgumentException("Invalid attribute name")
            };
        }

        public int getIntAttribute(string attributeName)
        {
            return attributeName switch
            {
                "Slot" => getSlot(),
                _ => throw new ArgumentException("Invalid integer attribute name")
            };
        }

        public DateTime getAppointmentDateTime (ServiceAppointment appointment) {
           
            return getDateTime();

        }

        public ServiceAppointment updateServiceAppointementById (ServiceAppointment appointment, string patientId, string nurseId,
            string doctorId, string Service, string status, DateTime dateTime, int slot, string location) {
                appointment.setPatientID(patientId);
                appointment.setNurseID(nurseId);
                appointment.setDoctorID(doctorId);
                appointment.setServiceId(Service);
                appointment.setStatus(status);
                appointment.setDateTime(dateTime);
                appointment.setSlot(slot);
                appointment.setLocation(location);
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

        public Dictionary<string, object> getApptDetails()
        {
            return new Dictionary<string, object>
            {
                { "PatientID", getPatientID() },
                // { "AppointmentID", GetAppointmentID() },
                { "NurseID", getNurseID() },
                { "DoctorID", getDoctorID() },
                { "ServiceType", getServiceType() },
                { "Status", getStatus() },
                { "DateTime", getDateTime() },
                { "Slot", getSlot() },
                { "Location", getLocation() }
            };
        }

        // Data Normalization
        // Convert firebase key-value pair into ServiceAppointment Structure so it can be used directly
        // No more key-value but return the object
        // Extracts values from { "PatientId": "USR010", "NurseId": "USR001", .... }
        // and maps them into the ServiceAppointment model
        // ServiceAppointment { PatientId = "USR010", NurseId = "USR001", ... }
        // Simple Domain Model Mapping
        public static ServiceAppointment fromFirestoreData(string appointmentId, Dictionary<string, object> data)
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
        public Dictionary<string, object> toFirestoreDictionary()
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
        public void updateStatus(string newStatus)
        {
            if (!string.IsNullOrWhiteSpace(newStatus))
            {
                Status = newStatus;
            }
        }

    }


}
