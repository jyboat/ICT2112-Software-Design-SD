using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T1;

namespace ClearCare.DataSource.M3T1
{
    public class AssessmentMapper : IAssessmentSend
    {
        private readonly FirestoreDb _db;

        public AssessmentMapper()
        {
            _db = FirebaseService.Initialize();
        }

            public async Task<List<Assessment>> fetchAssessments()
        {
            List<Assessment> assessments = new List<Assessment>();
            CollectionReference assessmentRef = _db.Collection("Assessment");
            QuerySnapshot snapshot = await assessmentRef.GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                if (doc.Exists)
                {
                    try
                    {
                        string id = doc.Id;
                        string hazardType = doc.GetValue<string>("HazardType");
                        string riskLevel = doc.GetValue<string>("RiskLevel");
                        string recommendation = doc.GetValue<string>("Recommendation");

                        // Handle CreatedAt
                        DateTime createdAt;
                        if (doc.GetValue<object>("CreatedAt") is Timestamp timestamp)
                        {
                            createdAt = timestamp.ToDateTime();
                        }
                        else
                        {
                            string dateString = doc.GetValue<string>("CreatedAt");
                            if (!DateTime.TryParse(dateString, out createdAt))
                            {
                                createdAt = DateTime.Now;
                            }
                        }

                        // Handle optional fields
                        string doctorId = doc.TryGetValue<string>("DoctorID", out var docId) ? docId : string.Empty;
                        string patientId = doc.TryGetValue<string>("PatientID", out var patId) ? patId : string.Empty;
                        List<string> imagePath = doc.TryGetValue<List<string>>("ImagePath", out var paths) ? paths : new List<string>();
                        Dictionary<string, bool> checklist = doc.TryGetValue<Dictionary<string, bool>>("HomeAssessmentChecklist", out var chklist) ? chklist : new Dictionary<string, bool>();

                        Assessment assessment = new Assessment(
                            id: id,
                            hazardType: hazardType,
                            riskLevel: riskLevel,
                            recommendation: recommendation,
                            createdAt: createdAt,
                            doctorId: doctorId,
                            patientId: patientId,
                            imagePath: imagePath,
                            homeAssessmentChecklist: checklist
                        );

                        assessments.Add(assessment);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting assessment {doc.Id}: {ex.Message}");
                    }
                }
            }

            return assessments;
        }

        // In AssessmentMapper.cs
        private async Task<Assessment> ConvertToAssessment(DocumentSnapshot doc)
        {
            string id = doc.Id;
            string hazardType = doc.GetValue<string>("HazardType");
            string riskLevel = doc.GetValue<string>("RiskLevel");
            string recommendation = doc.GetValue<string>("Recommendation");

            DateTime createdAt;
            if (doc.GetValue<object>("CreatedAt") is Timestamp timestamp)
            {
                createdAt = timestamp.ToDateTime();
            }
            else
            {
                string dateString = doc.GetValue<string>("CreatedAt");
                createdAt = DateTime.TryParse(dateString, out var date) ? date : DateTime.Now;
            }

            string doctorId = doc.GetValue<string>("DoctorID");
            string patientId = doc.TryGetValue<string>("PatientID", out var pid) ? pid : "DEFAULT_PATIENT";
            List<string> imagePath = doc.TryGetValue<List<string>>("ImagePath", out var paths) ? paths : new List<string>();
            Dictionary<string, bool> checklist = doc.TryGetValue<Dictionary<string, bool>>("HomeAssessmentChecklist", out var chklist) ? chklist : new Dictionary<string, bool>();

            return new Assessment(
                id: id,
                hazardType: hazardType,  // Now passing hazardType
                riskLevel: riskLevel,
                recommendation: recommendation,
                createdAt: createdAt,
                doctorId: doctorId,
                patientId: patientId,
                imagePath: imagePath,
                homeAssessmentChecklist: checklist
            );
        }

        public async Task<Assessment> fetchAssessmentById(string id)
        {
            DocumentReference docRef = _db.Collection("Assessment").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Console.WriteLine($"No assessment found with ID {id}");
                return null;
            }

            return await ConvertToAssessment(snapshot);
        }
        

        public async Task<bool> deleteAssessment(string id)
        {
            DocumentReference docRef = _db.Collection("Assessment").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                return false;
            }

            await docRef.DeleteAsync();
            return true;
        }

        public async Task<string> insertAssessment(string hazardType, string doctorId, List<string> imagePath, string riskLevel, string recommendation, string createdAt)
        {
            if (!DateTime.TryParse(createdAt, out DateTime parsedDate))
            {
                throw new ArgumentException("Invalid date format.");
            }

            var timestamp = Timestamp.FromDateTime(parsedDate.ToUniversalTime());
            DocumentReference docRef = _db.Collection("Assessment").Document();

            var assessmentData = new Dictionary<string, object>
            {
                { "HazardType", hazardType },
                { "DoctorID", doctorId },
                { "ImagePath", imagePath },
                { "RiskLevel", riskLevel },
                { "Recommendation", recommendation },
                { "CreatedAt", timestamp },
                { "HomeAssessmentChecklist", new Dictionary<string, bool>() }
            };

            await docRef.SetAsync(assessmentData);
            return docRef.Id;
        }

        public async Task<bool> updateAssessment(string id, string doctorId, List<string> imagePath, string riskLevel, string recommendation, string createdAt, Dictionary<string, bool> checklist = null)
        {
            DocumentReference docRef = _db.Collection("Assessment").Document(id);
            
            var updatedData = new Dictionary<string, object>
            {
                { "DoctorID", doctorId },
                { "ImagePath", imagePath },
                { "RiskLevel", riskLevel },
                { "Recommendation", recommendation },
                { "CreatedAt", createdAt }
            };

            if (checklist != null)
            {
                updatedData["HomeAssessmentChecklist"] = checklist;
            }

            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists) return false;

            await docRef.UpdateAsync(updatedData);
            return true;
        }
    }
}