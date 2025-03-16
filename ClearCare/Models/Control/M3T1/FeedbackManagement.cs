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

        public Task receiveFeedbacksByPatientId(List<Feedback> feedbacks)
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

        public Task receiveFeedback(Feedback feedback)
        {
            if (feedback != null)
            {
                Console.WriteLine("Received feedback");
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

        public Task receiveUpdateStatus(bool success)
        {
            if (success)
            {
                Console.WriteLine("Updated feedback successfully");
            }
            else
            {
                Console.WriteLine("Failed to update feedback");
            }
            return Task.CompletedTask;
        }

        public Task receiveResponseStatus(bool success)
        {
            if (success)
            {
                Console.WriteLine("Added response successfully");
            }
            else
            {
                Console.WriteLine("Failed to add response");
            }
            return Task.CompletedTask;
        }

        public Task receiveDeleteStatus(bool success)
        {
            if (success)
            {
                Console.WriteLine("Deleted feedback successfully");
            }
            else
            {
                Console.WriteLine("Failed to delete feedback");
            }
            return Task.CompletedTask;
        }

        public async Task<string> submitFeedback(string content, int rating, string patientId, string dateCreated)
        {
            return await _gateway.insertFeedback(content, rating, patientId, dateCreated);
        }

        public async Task<bool> updateFeedback(string feedbackId, string content, int rating, string dateCreated)
        {
            return await _gateway.updateFeedback(feedbackId, content, rating, dateCreated);
        }

        public async Task<bool> respondToFeedback(string feedbackId, string response, string doctorId, string dateResponded)
        {
            return await _gateway.insertResponse(feedbackId, response, doctorId, dateResponded);
        }

        public async Task<List<Feedback>> viewFeedback()
        {
            return await _gateway.fetchFeedbacks();
        }

        public async Task<List<Feedback>> viewFeedbackByPatientId(string patientId)
        {
            return await _gateway.fetchFeedbacksByPatientId(patientId);
        }

        public async Task<Feedback> getFeedback(string feedbackId)
        {
            return await _gateway.fetchFeedbackById(feedbackId);
        }

        public async Task<bool> deleteFeedback(string feedbackId)
        {
            return await _gateway.deleteFeedback(feedbackId);
        }
    }
}
