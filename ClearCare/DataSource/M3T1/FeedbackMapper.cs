using Google.Cloud.Firestore;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T1;

namespace ClearCare.DataSource.M3T1
{
    public class FeedbackMapper : AbstractFeedbackNotifier, IFeedbackSend, IResponseSend
    {
        private readonly FirestoreDb _db;
        private IFeedbackReceive _feedbackReceiver;
        private IResponseReceive _responseReceiver;

        public FeedbackMapper()
        {
            // Initialize Firebase
            _db = FirebaseService.Initialize();
        }

        public IFeedbackReceive feedbackReceiver
        {
            get { return _feedbackReceiver; }
            set { _feedbackReceiver = value; }
        }

        public IResponseReceive responseReceiver
        {
            get { return _responseReceiver; }
            set { _responseReceiver = value; }
        }

        // Insert New Feedback
        public async Task<string> insertFeedback(string content, int rating, string userId, string dateCreated)
        {
            DocumentReference docRef = _db.Collection("Feedback").Document();

            var feedback = new Dictionary<string, object>
            {
                { "Content",  content},
                { "Rating", rating },
                { "UserId", userId },
                { "DateCreated", dateCreated },
                { "HasResponded", false }
            };

            await docRef.SetAsync(feedback);
            await _feedbackReceiver.receiveAddStatus(true);

            return docRef.Id;
        }

        // Fetch All Feedbacks
        public async Task<List<Feedback>> fetchFeedbacks()
        {
            List<Feedback> feedbacks = new List<Feedback>();
            CollectionReference feedbackRef = _db.Collection("Feedback");
            QuerySnapshot snapshot = await feedbackRef.GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                if (doc.Exists)
                {
                    try
                    {
                        string id = doc.ContainsField("Id") ? doc.GetValue<string>("Id") : doc.Id;
                        string content = doc.ContainsField("Content") ? doc.GetValue<string>("Content") : "";
                        int rating = doc.ContainsField("Rating") ? doc.GetValue<int>("Rating") : 0;
                        string userId = doc.ContainsField("UserId") ? doc.GetValue<string>("UserId") : "";
                        string dateCreated = doc.ContainsField("DateCreated") ? doc.GetValue<string>("DateCreated") : "";
                        bool hasResponded = doc.ContainsField("HasResponded") ? doc.GetValue<bool>("HasResponded") : false;

                        Feedback feedback = new Feedback(id, content, rating, userId, dateCreated, hasResponded);
                        feedbacks.Add(feedback);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting feedback {doc.Id}: {ex.Message}");
                    }
                }
            }

            await _feedbackReceiver.receiveFeedbacks(feedbacks);
            return feedbacks;
        }

        // Fetch All Feedbacks by UserId
        public async Task<List<Feedback>> fetchFeedbacksByUserId(string userId)
        {
            List<Feedback> feedbacks = new List<Feedback>();
            CollectionReference feedbackRef = _db.Collection("Feedback");
            Query query = feedbackRef.WhereEqualTo("UserId", userId);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                if (doc.Exists)
                {
                    try
                    {
                        string id = doc.ContainsField("Id") ? doc.GetValue<string>("Id") : doc.Id;
                        string content = doc.ContainsField("Content") ? doc.GetValue<string>("Content") : "";
                        int rating = doc.ContainsField("Rating") ? doc.GetValue<int>("Rating") : 0;
                        string dateCreated = doc.ContainsField("DateCreated") ? doc.GetValue<string>("DateCreated") : "";
                        bool hasResponded = doc.ContainsField("HasResponded") ? doc.GetValue<bool>("HasResponded") : false;

                        Feedback feedback = new Feedback(id, content, rating, userId, dateCreated, hasResponded);
                        feedbacks.Add(feedback);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting feedback {doc.Id}: {ex.Message}");
                    }
                }
            }

            await _feedbackReceiver.receiveFeedbacksByUserId(feedbacks);
            return feedbacks;
        }

        // Insert New Response
        public async Task<string> insertResponse(string feedbackId, string response, string userId, string dateResponded)
        {
            DocumentReference docRef = _db.Collection("FeedbackResponse").Document();

            var responseData = new Dictionary<string, object>
            {
                { "FeedbackId", feedbackId },
                { "Response", response },
                { "UserId", userId },
                { "DateResponded", dateResponded }
            };

            await docRef.SetAsync(responseData);
            await _responseReceiver.receiveAddResponse(true);

            // Update Feedback HasResponded flag
            DocumentReference feedbackDoc = _db.Collection("Feedback").Document(feedbackId);
            await feedbackDoc.UpdateAsync(new Dictionary<string, object> { { "HasResponded", true } });

            // Notify Observer
            notify(feedbackId);

            return docRef.Id;
        }

        // Update Response
        public async Task<bool> updateResponse(string id, string response, string dateResponded)
        {
            DocumentReference docRef = _db.Collection("FeedbackResponse").Document(id);

            var updatedData = new Dictionary<string, object>
            {
                { "Response", response },
                { "DateResponded", dateResponded }
            };

            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                return false; // Document not found
            }

            await _responseReceiver.receiveUpdateResponse(true);
            await docRef.UpdateAsync(updatedData);
            return true;
        }

        // Fetch All Responses
        public async Task<List<FeedbackResponse>> fetchResponses()
        {
            List<FeedbackResponse> responses = new List<FeedbackResponse>();
            CollectionReference responseRef = _db.Collection("FeedbackResponse");
            QuerySnapshot snapshot = await responseRef.GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                if (doc.Exists)
                {
                    try
                    {
                        string id = doc.ContainsField("Id") ? doc.GetValue<string>("Id") : doc.Id;
                        string feedbackId = doc.ContainsField("FeedbackId") ? doc.GetValue<string>("FeedbackId") : "";
                        string response = doc.ContainsField("Response") ? doc.GetValue<string>("Response") : "";
                        string userId = doc.ContainsField("UserId") ? doc.GetValue<string>("UserId") : "";
                        string dateResponded = doc.ContainsField("DateResponded") ? doc.GetValue<string>("DateResponded") : "";

                        FeedbackResponse responseData = new FeedbackResponse(id, feedbackId, response, userId, dateResponded);
                        responses.Add(responseData);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting response {doc.Id}: {ex.Message}");
                    }
                }
            }

            await _responseReceiver.receiveResponses(responses);
            return responses;
        }

        // Fetch Response by FeedbackId
        public async Task<FeedbackResponse> fetchResponseByFeedbackId(string feedbackId)
        {
            CollectionReference responseRef = _db.Collection("FeedbackResponse");
            Query query = responseRef.WhereEqualTo("FeedbackId", feedbackId);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            if (snapshot.Documents.Count == 0)
            {
                Console.WriteLine($"No response found for FeedbackId {feedbackId}");
                return null;
            }

            DocumentSnapshot doc = snapshot.Documents[0];
            string id = doc.ContainsField("Id") ? doc.GetValue<string>("Id") : doc.Id;
            string response = doc.ContainsField("Response") ? doc.GetValue<string>("Response") : "";
            string userId = doc.ContainsField("UserId") ? doc.GetValue<string>("UserId") : "";
            string dateResponded = doc.ContainsField("DateResponded") ? doc.GetValue<string>("DateResponded") : "";

            FeedbackResponse responseData = new FeedbackResponse(id, feedbackId, response, userId, dateResponded);

            await _responseReceiver.receiveResponseByFeedbackId(responseData);
            return responseData;
        }

        // Delete Response
        public async Task<bool> deleteResponse(string id)
        {
            DocumentReference docRef = _db.Collection("FeedbackResponse").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                return false; // Document not found
            }

            // Get FeedbackId before deleting the response
            string feedbackId = snapshot.ContainsField("FeedbackId") ? snapshot.GetValue<string>("FeedbackId") : "";

            await docRef.DeleteAsync();
            await _responseReceiver.receiveDeleteResponse(true);

            // Reset Feedback HasResponded flag to false
            if (!string.IsNullOrEmpty(feedbackId))
            {
                DocumentReference feedbackDoc = _db.Collection("Feedback").Document(feedbackId);
                await feedbackDoc.UpdateAsync(new Dictionary<string, object> { { "HasResponded", false } });
            }

            return true;
        }
    }
}