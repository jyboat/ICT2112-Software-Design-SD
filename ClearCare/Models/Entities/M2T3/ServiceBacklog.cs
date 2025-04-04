using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities
{
    public class ServiceBacklog
    {
        private string BacklogId { get; set; }
        private string AppointmentId { get; set; }

        // private getters and setters
        private string getBacklogId() => BacklogId;
        private void setBacklogId(string backlogID) => BacklogId = backlogID;
        private string getServiceAppointmentId() => AppointmentId;
        private void setServiceAppointmentId(string appointmentID) => AppointmentId = appointmentID;

        // public constructor
        public ServiceBacklog() {}

        public ServiceBacklog(string appointmentID)
        {
            AppointmentId = appointmentID;
        }

        // public getters and setters
        public Dictionary<string, string> getBacklogInformation()
        {
            return new Dictionary<string, string>
            {
                { "backlogId", getBacklogId() },
                { "appointmentId", getServiceAppointmentId() },
            };
        }
        
        public void setBacklogInformation(string backlogID, string appointmentID)
        {
            setBacklogId(backlogID);
            setServiceAppointmentId(appointmentID);
        }
    }
}