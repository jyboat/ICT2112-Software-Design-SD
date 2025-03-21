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
        private string UserId { get; set; }
        [FirestoreProperty]
        private string DateCreated { get; set; }
        [FirestoreProperty]
        public bool HasResponded { get; set; }

        private string GetId() => Id;
        private string GetContent() => Content;
        private int GetRating() => Rating;
        private string GetUserId() => UserId;
        private string GetDateCreated() => DateCreated;
        private bool GetHasResponded() => HasResponded;

        private void SetId(string id) => Id = id;
        private void SetContent(string content) => Content = content;
        private void SetRating(int rating) => Rating = rating;
        private void SetUserId(string userId) => UserId = userId;
        private void SetDateCreated(string dateCreated) => DateCreated = dateCreated;
        private void SetHasResponded(bool hasResponded) => HasResponded = hasResponded;

        public Feedback() { }

        public Feedback(string id, string content, int rating, string userId, string dateCreated, bool hasResponded)
        {
            Id = id;
            Content = content;
            Rating = rating;
            UserId = userId;
            DateCreated = dateCreated;
            HasResponded = hasResponded;
        }

        public Dictionary<string, object> GetFeedbackDetails()
        {
            return new Dictionary<string, object>
            {
                { "Id", GetId() },
                { "Content", GetContent() },
                { "Rating", GetRating() },
                { "UserId", GetUserId() },
                { "DateCreated", GetDateCreated() },
                { "HasResponded", GetHasResponded() },
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
            if (feedbackDetails.ContainsKey("UserId")) SetUserId(feedbackDetails["UserId"]?.ToString() ?? string.Empty);
            if (feedbackDetails.ContainsKey("DateCreated")) SetDateCreated(feedbackDetails["DateCreated"]?.ToString() ?? string.Empty);
            if (feedbackDetails.ContainsKey("HasResponded")) SetHasResponded(Convert.ToBoolean(feedbackDetails["HasResponded"] ?? false));
        }
    }
}
