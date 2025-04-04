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
        private bool HasResponded { get; set; }

        private string getId() => Id;
        private string getContent() => Content;
        private int getRating() => Rating;
        private string getUserId() => UserId;
        private string getDateCreated() => DateCreated;
        private bool getHasResponded() => HasResponded;

        private void setId(string id) => Id = id;
        private void setContent(string content) => Content = content;
        private void setRating(int rating) => Rating = rating;
        private void setUserId(string userId) => UserId = userId;
        private void setDateCreated(string dateCreated) => DateCreated = dateCreated;
        private void setHasResponded(bool hasResponded) => HasResponded = hasResponded;

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

        public Dictionary<string, object> getFeedbackDetails()
        {
            return new Dictionary<string, object>
            {
                { "Id", getId() },
                { "Content", getContent() },
                { "Rating", getRating() },
                { "UserId", getUserId() },
                { "DateCreated", getDateCreated() },
                { "HasResponded", getHasResponded() },
            };
        }

        public void setFeedbackDetails(Dictionary<string, object> feedbackDetails)
        {
            if (feedbackDetails == null)
            {
                throw new ArgumentNullException(nameof(feedbackDetails));
            }

            if (feedbackDetails.ContainsKey("Id")) setId(feedbackDetails["Id"]?.ToString() ?? string.Empty);
            if (feedbackDetails.ContainsKey("Content")) setContent(feedbackDetails["Content"]?.ToString() ?? string.Empty);
            if (feedbackDetails.ContainsKey("Rating")) setRating(Convert.ToInt32(feedbackDetails["Rating"] ?? 0));
            if (feedbackDetails.ContainsKey("UserId")) setUserId(feedbackDetails["UserId"]?.ToString() ?? string.Empty);
            if (feedbackDetails.ContainsKey("DateCreated")) setDateCreated(feedbackDetails["DateCreated"]?.ToString() ?? string.Empty);
            if (feedbackDetails.ContainsKey("HasResponded")) setHasResponded(Convert.ToBoolean(feedbackDetails["HasResponded"] ?? false));
        }
    }
}
