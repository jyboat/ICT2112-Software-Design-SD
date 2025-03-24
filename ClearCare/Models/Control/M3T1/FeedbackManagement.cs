using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.DataSource;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T1;


namespace ClearCare.Models.Control.M3T1
{
    public class FeedbackManager : IFeedbackReceive
    {
        private readonly IFeedbackSend _gateway;

        public FeedbackManager(IFeedbackSend gateway)
        {
            _gateway = gateway;
        }

        public Task receiveFeedbacks(List<Feedback> feedbacks)
        {
            if (feedbacks.Count > 0)
            {
                Console.WriteLine($"Received {feedbacks.Count} feedback");
            }
            else
            {
                Console.WriteLine("No feedback received");
            }
            return Task.CompletedTask;
        }

        public Task receiveFeedbacksByUserId(List<Feedback> feedbacks)
        {
            if (feedbacks.Count > 0)
            {
                Console.WriteLine($"Received {feedbacks.Count} feedback");
            }
            else
            {
                Console.WriteLine("No feedback received");
            }
            return Task.CompletedTask;
        }

        public Task receiveAddStatus(bool success)
        {
            if (success)
            {
                Console.WriteLine("Inserted feedback successfully");
            }
            else
            {
                Console.WriteLine("Failed to insert feedback");
            }
            return Task.CompletedTask;
        }

        public async Task<string> submitFeedback(string content, int rating, string patientId, string dateCreated)
        {
            return await _gateway.insertFeedback(content, rating, patientId, dateCreated);
        }

        public async Task<List<Feedback>> viewFeedback()
        {
            return await _gateway.fetchFeedbacks();
        }

        public async Task<List<Feedback>> viewFeedbackByUserId(string patientId)
        {
            return await _gateway.fetchFeedbacksByUserId(patientId);
        }

        // Response Notification for Patients, for FeedbackController, not implemented in any interfaces
        public bool ResponseNotification(string patientId)
        {
            if (PatientNotificationObserver.NotificationMap.ContainsKey(patientId))
            {
                PatientNotificationObserver.NotificationMap.Remove(patientId);
                return true;
            }
            return false;
        }

        // Combine feedbackList and responseList, for FeedbackController, not implemented in any interfaces
        public List<Dictionary<string, object>> CombineFeedbackResponse(
            List<Dictionary<string, object>> feedbackList, 
            List<Dictionary<string, object>> responseList)
        {
            // Create lookup dictionary for responses by FeedbackId
            var responseMap = responseList
                .Where(r => r.ContainsKey("FeedbackId"))
                .ToDictionary(r => r["FeedbackId"]?.ToString() ?? "", r => r);

            List<Dictionary<string, object>> combinedList = feedbackList.Select(fb =>
            {
                string feedbackId = fb["Id"]?.ToString() ?? "";
                responseMap.TryGetValue(feedbackId, out var response);

                fb["Response"] = response?["Response"] ?? "";
                fb["DateResponded"] = response?["DateResponded"] ?? "";
                fb["ResponseUserId"] = response?["UserId"] ?? "";
                fb["ResponseId"] = response?["Id"] ?? "";

                return fb;
            }).ToList();

            return combinedList;
        }
    }
}
