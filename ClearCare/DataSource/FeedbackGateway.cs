﻿using Google.Cloud.Firestore;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using ClearCare.Models.Entities;
using ClearCare.Models.Interfaces;

namespace ClearCare.DataSource
{
    public class FeedbackGateway : IFeedbackSend
    {
        private readonly FirestoreDb _db;
        private IFeedbackReceive _receiver;

        public FeedbackGateway()
        {
            // Initialize Firebase
            _db = FirebaseService.Initialize();
        }

        public IFeedbackReceive receiver
        {
            get { return _receiver; }
            set { _receiver = value; }
        }

        public async Task<string> insertFeedback(string content, int rating, string patientId, string dateCreated)
        {
            DocumentReference docRef = _db.Collection("Feedback").Document();

            var feedback = new Dictionary<string, object>
            {
                { "Content",  content},
                { "Rating", rating },
                { "PatientId", patientId },
                { "DateCreated", dateCreated }
            };

            await docRef.SetAsync(feedback);
            await _receiver.receiveAddStatus(true);

            return docRef.Id;
        }

        // Feedback Update
        public async Task<bool> updateFeedback(string id, string content, int rating, string dateCreated)
        {
            DocumentReference docRef = _db.Collection("Feedback").Document(id);

            var updatedData = new Dictionary<string, object>
            {
                { "Content",  content},
                { "Rating", rating },
                { "DateCreated", dateCreated }
            };

            // Check if document exists before updating
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                return false; // Document not found
            }

            await docRef.UpdateAsync(updatedData);
            await _receiver.receiveUpdateStatus(true);

            return true;
        }

        public async Task<bool> insertResponse(string id, string response, string doctorId, string dateResponded)
        {
            DocumentReference docRef = _db.Collection("Feedback").Document(id);

            var updatedData = new Dictionary<string, object>
            {
                { "Response", response },
                { "DoctorId", doctorId },
                { "DateResponded", dateResponded }
            };

            // Check if document exists before updating
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                return false; // Document not found
            }

            await docRef.UpdateAsync(updatedData);
            await _receiver.receiveResponseStatus(true);

            return true;
        }

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
                        string response = doc.ContainsField("Response") ? doc.GetValue<string>("Response") : "";
                        string patientId = doc.ContainsField("PatientId") ? doc.GetValue<string>("PatientId") : "";
                        string dateCreated = doc.ContainsField("DateCreated") ? doc.GetValue<string>("DateCreated") : "";
                        string doctorId = doc.ContainsField("DoctorId") ? doc.GetValue<string>("DoctorId") : "";
                        string dateResponded = doc.ContainsField("DateResponded") ? doc.GetValue<string>("DateResponded") : "";

                        Feedback feedback = new Feedback(id, content, rating, response, patientId, dateCreated, doctorId, dateResponded);
                        feedbacks.Add(feedback);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting feedback {doc.Id}: {ex.Message}");
                    }
                }
            }

            await _receiver.receiveFeedbacks(feedbacks);
            return feedbacks;
        }

        public async Task<List<Feedback>> fetchFeedbacksByPatientId(string patientId)
        {
            List<Feedback> feedbacks = new List<Feedback>();
            CollectionReference feedbackRef = _db.Collection("Feedback");
            Query query = feedbackRef.WhereEqualTo("PatientId", patientId);
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
                        string response = doc.ContainsField("Response") ? doc.GetValue<string>("Response") : "";
                        string dateCreated = doc.ContainsField("DateCreated") ? doc.GetValue<string>("DateCreated") : "";
                        string doctorId = doc.ContainsField("DoctorId") ? doc.GetValue<string>("DoctorId") : "";
                        string dateResponded = doc.ContainsField("DateResponded") ? doc.GetValue<string>("DateResponded") : "";

                        Feedback feedback = new Feedback(id, content, rating, response, patientId, dateCreated, doctorId, dateResponded);
                        feedbacks.Add(feedback);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting feedback {doc.Id}: {ex.Message}");
                    }
                }
            }

            await _receiver.receiveFeedbacksByPatientId(feedbacks);
            return feedbacks;
        }

        public async Task<Feedback> fetchFeedbackById(string id)
        {
            DocumentReference docRef = _db.Collection("Feedback").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Console.WriteLine($"No feedback found {snapshot.Id}");
                return null;
            }

            string content = snapshot.ContainsField("Content") ? snapshot.GetValue<string>("Content") : "";
            int rating = snapshot.ContainsField("Rating") ? snapshot.GetValue<int>("Rating") : 0;
            string response = snapshot.ContainsField("Response") ? snapshot.GetValue<string>("Response") : "";
            string patientId = snapshot.ContainsField("PatientId") ? snapshot.GetValue<string>("PatientId") : "";
            string dateCreated = snapshot.ContainsField("DateCreated") ? snapshot.GetValue<string>("DateCreated") : "";
            string doctorId = snapshot.ContainsField("DoctorId") ? snapshot.GetValue<string>("DoctorId") : "";
            string dateResponded = snapshot.ContainsField("DateResponded") ? snapshot.GetValue<string>("DateResponded") : "";

            Feedback feedback = new Feedback(id, content, rating, response, patientId, dateCreated, doctorId, dateResponded);
            await _receiver.receiveFeedback(feedback);

            return feedback;
        }

        public async Task<bool> deleteFeedback(string id)
        {
            DocumentReference docRef = _db.Collection("Feedback").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                return false; // Document not found
            }

            await docRef.DeleteAsync();
            await _receiver.receiveDeleteStatus(true);

            return true;
        }
    }
}