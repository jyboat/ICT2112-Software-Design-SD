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
        public bool responseNotification(string patientId)
        {
            return PatientNotificationObserver.CheckAndClearNotification(patientId);
        }

        // Combine feedbackList and responseList, for FeedbackController, not implemented in any interfaces
        public List<Dictionary<string, object>> combineFeedbackResponse(
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

        // Search Filter, for FeedbackController, not implemented in any interfaces
        public List<Dictionary<string, object>> applySearchFilter(List<Dictionary<string, object>> list, string search)
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                return list.Where(fb =>
                    (fb["Content"]?.ToString()?.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (fb["Response"]?.ToString()?.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                ).ToList();
            }

            return list;
        }

        // Response Filter, for FeedbackController, not implemented in any interfaces
        public List<Dictionary<string, object>> applyResponseFilter(List<Dictionary<string, object>> list, string responseFilter)
        {
            if (responseFilter == "Responded")
            {
                return list.Where(fb => Convert.ToBoolean(fb["HasResponded"])).ToList();
            }
            else if (responseFilter == "Unresponded")
            {
                return list.Where(fb => !Convert.ToBoolean(fb["HasResponded"])).ToList();
            }

            return list;
        }

        // Rating Filter, for FeedbackController, not implemented in any interfaces
        public List<Dictionary<string, object>> applyRatingFilter(List<Dictionary<string, object>> list, int ratingFilter)
        {
            if (ratingFilter >= 1 && ratingFilter <= 5)
            {
                return list.Where(fb => Convert.ToInt32(fb["Rating"]) == ratingFilter).ToList();
            }

            return list;
        }

        // Pagination, for FeedbackController, not implemented in any interfaces
        public List<Dictionary<string, object>> applyPagination(List<Dictionary<string, object>> list, int page, int pageSize)
        {
            var paginatedList = list
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (paginatedList);
        }

    }
}
