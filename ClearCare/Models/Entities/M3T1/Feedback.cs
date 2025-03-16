using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;
using Google.Cloud.Location;
using Newtonsoft.Json;

namespace ClearCare.Models.Entities.M3T1
{
    public class Feedback
    {
        private string Id { get; set; }
        [FirestoreProperty]
        private string Content { get; set; }
        [FirestoreProperty]
        private int Rating { get; set; }
        [FirestoreProperty]
        private string Response { get; set; }
        [FirestoreProperty]
        private string PatientId { get; set; }
        [FirestoreProperty]
        private string DateCreated { get; set; }
        [FirestoreProperty]
        private string DoctorId { get; set; }
        [FirestoreProperty]
        private string DateResponded { get; set; }

        private string GetId() => Id;
        private string GetContent() => Content;
        private int GetRating() => Rating;
        private string GetResponse() => Response;
        private string GetPatientId() => PatientId;
        private string GetDateCreated() => DateCreated;
        private string GetDoctorId() => DoctorId;
        private string GetDateResponded() => DateResponded;

        private void SetId(string id) => Id = id;
        private void SetContent(string content) => Content = content;
        private void SetRating(int rating) => Rating = rating;
        private void SetResponse(string response) => Response = response;
        private void SetPatientId(string patientId) => PatientId = patientId;
        private void SetDateCreated(string dateCreated) => DateCreated = dateCreated;
        private void SetDoctorId(string doctorId) => DoctorId = doctorId;
        private void SetDateResponded(string dateResponded) => DateResponded = dateResponded;

        public Feedback() { }

        public Feedback(string id, string content, int rating, string response, string patientId, string dateCreated, string doctorId, string dateResponded)
        {
            Id = id;
            Content = content;
            Rating = rating;
            Response = response;
            PatientId = patientId;
            DateCreated = dateCreated;
            DoctorId = doctorId;
            DateResponded = dateResponded;
        }

        public Dictionary<string, object> GetFeedbackDetails()
        {
            return new Dictionary<string, object>
            {
                { "Id", GetId() },
                { "Content", GetContent() },
                { "Rating", GetRating() },
                { "Response", GetResponse() },
                { "PatientId", GetPatientId() },
                { "DateCreated", GetDateCreated() },
                { "DoctorId", GetDoctorId() },
                { "DateResponded", GetDateResponded() }
            };
        }

        public void setFeedbackDetails(Dictionary<string, object> feedbackDetails)
        {
            if (feedbackDetails == null)
            {
                throw new ArgumentNullException(nameof(feedbackDetails));
            }

            if (feedbackDetails.ContainsKey("Id")) SetId(feedbackDetails["Id"]?.ToString() ?? string.Empty);
            if (feedbackDetails.ContainsKey("Content")) SetContent(feedbackDetails["Content"]?.ToString() ?? string.Empty);
            if (feedbackDetails.ContainsKey("Rating")) SetRating(Convert.ToInt32(feedbackDetails["Rating"] ?? 0));
            if (feedbackDetails.ContainsKey("Response")) SetResponse(feedbackDetails["Response"]?.ToString() ?? string.Empty);
            if (feedbackDetails.ContainsKey("PatientId")) SetPatientId(feedbackDetails["PatientId"]?.ToString() ?? string.Empty);
            if (feedbackDetails.ContainsKey("DateCreated")) SetDateCreated(feedbackDetails["DateCreated"]?.ToString() ?? string.Empty);
            if (feedbackDetails.ContainsKey("DoctorId")) SetPatientId(feedbackDetails["DoctorId"]?.ToString() ?? string.Empty);
            if (feedbackDetails.ContainsKey("DateResponded")) SetDateCreated(feedbackDetails["DateResponded"]?.ToString() ?? string.Empty);
        }

        public void setResponseDetails(Dictionary<string, object> feedbackDetails)
        {
            if (feedbackDetails == null)
            {
                throw new ArgumentNullException(nameof(feedbackDetails));
            }

            if (feedbackDetails.ContainsKey("Response")) SetResponse(feedbackDetails["Response"]?.ToString() ?? string.Empty);
            if (feedbackDetails.ContainsKey("DoctorId")) SetDoctorId(feedbackDetails["DoctorId"]?.ToString() ?? string.Empty);
            if (feedbackDetails.ContainsKey("DateResponded")) SetDateResponded(feedbackDetails["DateResponded"]?.ToString() ?? string.Empty);
        }
    }
}
