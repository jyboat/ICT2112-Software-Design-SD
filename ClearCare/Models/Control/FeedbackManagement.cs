using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.DataSource;
using ClearCare.Models.Entities;


namespace ClearCare.Models.Controls
{
    public class FeedbackManager
    {
        private readonly FeedbackGateway _gateway;

        public FeedbackManager()
        {
            _gateway = new FeedbackGateway();
        }

        public async Task<string> submitFeedback(string content, int rating, string patientId, string dateCreated)
        {
            return await _gateway.insertFeedback(content, rating, patientId, dateCreated);
        }

        public async Task<bool> respondToFeedback(string id, string response, string doctorId, string dateResponded)
        {
            return await _gateway.updateFeedback(id, response, doctorId, dateResponded);
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
    }
}
