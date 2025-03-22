using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;
using Google.Cloud.Location;
using Newtonsoft.Json;

namespace ClearCare.Models.Entities.M3T1
{
    public class FeedbackResponse
    {
        private string Id { get; set; }
        [FirestoreProperty]
        private string FeedbackId { get; set; }
        [FirestoreProperty]
        private string Response { get; set; }
        [FirestoreProperty]
        private string UserId { get; set; }
        [FirestoreProperty]
        private string DateResponded { get; set; }

        private string GetId() => Id;
        private string GetFeedbackId() => FeedbackId;
        private string GetResponse() => Response;
        private string GetUserId() => UserId;
        private string GetDateResponded() => DateResponded;

        private void SetId(string id) => Id = id;
        private void SetFeedbackId(string feedbackId) => FeedbackId = feedbackId;
        private void SetResponse(string response) => Response = response;
        private void SetUserId(string userId) => UserId = userId;
        private void SetDateResponded(string dateResponded) => DateResponded = dateResponded;

        public FeedbackResponse() { }

        public FeedbackResponse(string id, string feedbackId, string response, string userId, string dateResponded)
        {
            Id = id;
            FeedbackId = feedbackId;
            Response = response;
            UserId = userId;
            DateResponded = dateResponded;
        }

        public Dictionary<string, object> GetResponseDetails()
        {
            return new Dictionary<string, object>
            {
                { "Id", GetId() },
                { "FeedbackId", GetFeedbackId() },
                { "Response", GetResponse() },
                { "UserId", GetUserId() },
                { "DateResponded", GetDateResponded() }
            };
        }

        public void setResponseDetails(Dictionary<string, object> responseDetails)
        {
            if (responseDetails == null)
            {
                throw new ArgumentNullException(nameof(responseDetails));
            }

            if (responseDetails.ContainsKey("Id")) SetId(responseDetails["Id"]?.ToString() ?? string.Empty);
            if (responseDetails.ContainsKey("FeedbackId")) SetFeedbackId(responseDetails["FeedbackId"]?.ToString() ?? string.Empty);
            if (responseDetails.ContainsKey("Response")) SetResponse(responseDetails["Response"]?.ToString() ?? string.Empty);
            if (responseDetails.ContainsKey("UserId")) SetUserId(responseDetails["UserId"]?.ToString() ?? string.Empty);
            if (responseDetails.ContainsKey("DateResponded")) SetDateResponded(responseDetails["DateResponded"]?.ToString() ?? string.Empty);
        }
    }
}
