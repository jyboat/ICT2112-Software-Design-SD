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
    }
}
