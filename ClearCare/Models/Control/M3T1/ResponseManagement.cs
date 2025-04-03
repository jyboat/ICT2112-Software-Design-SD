﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.DataSource;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T1;


namespace ClearCare.Models.Control.M3T1
{
    public class ResponseManager : IResponseReceive
    {
        private readonly IResponseSend _gateway;
        public ResponseManager(IResponseSend gateway)
        {
            _gateway = gateway;
        }

        public Task receiveResponses(List<FeedbackResponse> responses)
        {
            if (responses.Count > 0)
            {
                Console.WriteLine($"Received {responses.Count} response");
            }
            else
            {
                Console.WriteLine("No response received");
            }
            return Task.CompletedTask;
        }

        public Task receiveResponseByFeedbackId(FeedbackResponse response)
        {
            if (response != null)
            {
                Console.WriteLine("Received response");
            }
            else
            {
                Console.WriteLine("No response received");
            }
            return Task.CompletedTask;
        }

        public Task receiveAddResponse(bool success)
        {
            if (success)
            {
                Console.WriteLine("Inserted response successfully");
            }
            else
            {
                Console.WriteLine("Failed to insert response");
            }
            return Task.CompletedTask;
        }

        public Task receiveUpdateResponse(bool success)
        {
            if (success)
            {
                Console.WriteLine("Updated response successfully");
            }
            else
            {
                Console.WriteLine("Failed to update response");
            }
            return Task.CompletedTask;
        }

        public Task receiveDeleteResponse(bool success)
        {
            if (success)
            {
                Console.WriteLine("Deleted response successfully");
            }
            else
            {
                Console.WriteLine("Failed to delete response");
            }
            return Task.CompletedTask;
        }

        public async Task<string> respondToFeedback(string feedbackId, string response, string userId, string dateResponded)
        {
            return await _gateway.insertResponse(feedbackId, response, userId, dateResponded);
        }

        public async Task<bool> updateResponse(string responseId, string response, string dateResponded, string userId)
        {
            return await _gateway.updateResponse(responseId, response, dateResponded, userId);
        }

        public async Task<List<FeedbackResponse>> viewResponse()
        {
            return await _gateway.fetchResponses();
        }

        public async Task<FeedbackResponse> viewResponseByFeedbackId(string feedbackId)
        {
            return await _gateway.fetchResponseByFeedbackId(feedbackId);
        }

        public async Task<bool> deleteResponse(string responseId)
        {
            return await _gateway.deleteResponse(responseId);
        }

        // Get responses for feedbackList, for FeedbackController, not implemented in any interfaces
        public async Task<List<Dictionary<string, object>>> getResponsesForFeedbackList(List<Dictionary<string, object>> feedbackList)
        {
            var responseList = new List<Dictionary<string, object>>();

            foreach (var feedback in feedbackList)
            {
                string? feedbackId = feedback["Id"]?.ToString();
                if (!string.IsNullOrEmpty(feedbackId))
                {
                    var response = await viewResponseByFeedbackId(feedbackId);
                    if (response != null)
                    {
                        responseList.Add(response.getResponseDetails());
                    }
                }
            }

            return responseList;
        }

    }
}
