﻿using ClearCare.Models.Entities;

namespace ClearCare.Models.Interfaces
{
    public interface IFeedbackSend
    {
        Task<List<Feedback>> fetchFeedbacks();

        Task<Feedback> fetchFeedbackById(string id);

        Task<List<Feedback>> fetchFeedbacksByPatientId(string id);

        Task<string> insertFeedback(string content, int rating, string patientId, string dateCreated);

        Task<bool> updateFeedback(string id, string content, int rating, string dateCreated);

        Task<bool> insertResponse(string id, string response, string doctorId, string dateResponded);

        Task<bool> deleteFeedback(string id);
    }
}
