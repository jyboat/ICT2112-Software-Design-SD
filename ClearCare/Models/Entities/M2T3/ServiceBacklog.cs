using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities
{
    [FirestoreData]
    public class ServiceBacklog
    {
        private string backlogId { get; set; }

        private string appointmentId { get; set; }

        // private getters and setters
        private string getBacklogId() => backlogId;
        private string getServiceAppointmentId() => appointmentId;

        private void setBacklogId(string backlogID) => backlogId = backlogID;
        private void setServiceAppointmentId(string appointmentID) => appointmentId = appointmentID;

        // public constructor
        public ServiceBacklog(string appointmentID)
        {
            appointmentId = appointmentID;
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
