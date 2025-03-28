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

        private string getId() => Id;
        private string getFeedbackId() => FeedbackId;
        private string getResponse() => Response;
        private string getUserId() => UserId;
        private string getDateResponded() => DateResponded;

        private void setId(string id) => Id = id;
        private void setFeedbackId(string feedbackId) => FeedbackId = feedbackId;
        private void setResponse(string response) => Response = response;
        private void setUserId(string userId) => UserId = userId;
        private void setDateResponded(string dateResponded) => DateResponded = dateResponded;

        public FeedbackResponse() { }

        public FeedbackResponse(string id, string feedbackId, string response, string userId, string dateResponded)
        {
            Id = id;
            FeedbackId = feedbackId;
            Response = response;
            UserId = userId;
            DateResponded = dateResponded;
        }

        public Dictionary<string, object> getResponseDetails()
        {
            return new Dictionary<string, object>
            {
                { "Id", getId() },
                { "FeedbackId", getFeedbackId() },
                { "Response", getResponse() },
                { "UserId", getUserId() },
                { "DateResponded", getDateResponded() }
            };
        }

        public void setResponseDetails(Dictionary<string, object> responseDetails)
        {
            if (responseDetails == null)
            {
                throw new ArgumentNullException(nameof(responseDetails));
            }

            if (responseDetails.ContainsKey("Id")) setId(responseDetails["Id"]?.ToString() ?? string.Empty);
            if (responseDetails.ContainsKey("FeedbackId")) setFeedbackId(responseDetails["FeedbackId"]?.ToString() ?? string.Empty);
            if (responseDetails.ContainsKey("Response")) setResponse(responseDetails["Response"]?.ToString() ?? string.Empty);
            if (responseDetails.ContainsKey("UserId")) setUserId(responseDetails["UserId"]?.ToString() ?? string.Empty);
            if (responseDetails.ContainsKey("DateResponded")) setDateResponded(responseDetails["DateResponded"]?.ToString() ?? string.Empty);
        }
    }
}
