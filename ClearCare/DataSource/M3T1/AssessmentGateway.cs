using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T1;

namespace ClearCare.DataSource.M3T1
{
    public class AssessmentGateway : IAssessmentSend, IAssessment
    {
        private readonly FirestoreDb _db;

        public AssessmentGateway()
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
                        string id = doc.ContainsField("Id") ? doc.GetValue<string>("Id") : doc.Id;
                        string hazardType = doc.ContainsField("HazardType") ? doc.GetValue<string>("HazardType") : "";
                        string riskLevel = doc.ContainsField("RiskLevel") ? doc.GetValue<string>("RiskLevel") : "";
                        string recommendation = doc.ContainsField("Recommendation") ? doc.GetValue<string>("Recommendation") : "";

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
                        string patientId = doc.TryGetValue<string>("PatientId", out var patId) ? patId : string.Empty;
                        string imagePath = doc.TryGetValue<string>("ImagePath", out var paths) ? paths : "";
                        Dictionary<string, bool> checklist = doc.TryGetValue<Dictionary<string, bool>>("HomeAssessmentChecklist", out var chklist) ? chklist : new Dictionary<string, bool>();

                        Assessment assessment = new Assessment(
                            id: id,
                            hazardType: hazardType,
                            riskLevel: riskLevel,
                            recommendation: recommendation,
                            createdAt: createdAt,
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

        public async Task<Assessment> fetchAssessmentById(string id)
        {
            DocumentReference docRef = _db.Collection("Assessment").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Console.WriteLine($"No assessment found with ID {id}");
                return null;
            }

            string hazardType = snapshot.ContainsField("HazardType") ? snapshot.GetValue<string>("HazardType") : "";
            string riskLevel = snapshot.ContainsField("RiskLevel") ? snapshot.GetValue<string>("RiskLevel") : "";
            string recommendation = snapshot.ContainsField("Recommendation") ? snapshot.GetValue<string>("Recommendation") : "";

            DateTime createdAt;
            if (snapshot.GetValue<object>("CreatedAt") is Timestamp timestamp)
            {
                createdAt = timestamp.ToDateTime();
            }
            else
            {
                string dateString = snapshot.GetValue<string>("CreatedAt");
                createdAt = DateTime.TryParse(dateString, out var date) ? date : DateTime.Now;
            }

            string patientId = snapshot.TryGetValue<string>("PatientId", out var pid) ? pid : "DEFAULT_PATIENT";
            string imagePath = snapshot.TryGetValue<string>("ImagePath", out var paths) ? paths : "";
            Dictionary<string, bool> checklist = snapshot.TryGetValue<Dictionary<string, bool>>("HomeAssessmentChecklist", out var chklist) ? chklist : new Dictionary<string, bool>();

            Assessment assessment = new Assessment(
                id: id,
                hazardType: hazardType,  // Now passing hazardType
                riskLevel: riskLevel,
                recommendation: recommendation,
                createdAt: createdAt,
                patientId: patientId,
                imagePath: imagePath,
                homeAssessmentChecklist: checklist
            );

            return assessment;
        }

        public async Task<Assessment> fetchAssessmentByPatientId(string patientId)
        {
            CollectionReference assessmentRef = _db.Collection("Assessment");
            QuerySnapshot query = await assessmentRef
                .WhereEqualTo("PatientId", patientId)
                .OrderByDescending("CreatedAt") 
                .Limit(1) 
                .GetSnapshotAsync();

            if (!query.Documents.Any())
            {
                Console.WriteLine($"No assessment found with patient ID {patientId}");
                return null;
            }
            DocumentSnapshot snapshot = query.Documents.FirstOrDefault();


            string id = snapshot.ContainsField("Id") ? snapshot.GetValue<string>("Id") : snapshot.Id;
            string hazardType = snapshot.ContainsField("HazardType") ? snapshot.GetValue<string>("HazardType") : "";
            string riskLevel = snapshot.ContainsField("RiskLevel") ? snapshot.GetValue<string>("RiskLevel") : "";
            string recommendation = snapshot.ContainsField("Recommendation") ? snapshot.GetValue<string>("Recommendation") : "";

            DateTime createdAt;
            if (snapshot.GetValue<object>("CreatedAt") is Timestamp timestamp)
            {
                createdAt = timestamp.ToDateTime();
            }
            else
            {
                string dateString = snapshot.GetValue<string>("CreatedAt");
                createdAt = DateTime.TryParse(dateString, out var date) ? date : DateTime.Now;
            }

            string imagePath = snapshot.TryGetValue<string>("ImagePath", out var paths) ? paths : "";
            Dictionary<string, bool> checklist = snapshot.TryGetValue<Dictionary<string, bool>>("HomeAssessmentChecklist", out var chklist) ? chklist : new Dictionary<string, bool>();

            Assessment assessment = new Assessment(
                id: id,
                hazardType: hazardType,  // Now passing hazardType
                riskLevel: riskLevel,
                recommendation: recommendation,
                createdAt: createdAt,
                patientId: patientId,
                imagePath: imagePath,
                homeAssessmentChecklist: checklist
            );

            return assessment;
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

        public async Task<string> insertAssessment(string imagePath, string riskLevel, string recommendation, string createdAt, string patientId)
        {
            if (!DateTime.TryParse(createdAt, out DateTime parsedDate))
            {
                throw new ArgumentException("Invalid date format.");
            }

            var timestamp = Timestamp.FromDateTime(parsedDate.ToUniversalTime());
            DocumentReference docRef = _db.Collection("Assessment").Document();

            var assessmentData = new Dictionary<string, object>
            {
                { "ImagePath", imagePath },
                { "RiskLevel", riskLevel },
                { "Recommendation", recommendation },
                { "CreatedAt", timestamp },
                { "HomeAssessmentChecklist", new Dictionary<string, bool>() },
                { "PatientId", patientId }
            };

            await docRef.SetAsync(assessmentData);
            return docRef.Id;
        }

        public async Task<bool> updateAssessment(string id, string riskLevel, string recommendation, string hazardType, Dictionary<string, bool> checklist = null)
        {
            DocumentReference docRef = _db.Collection("Assessment").Document(id);
            
            var updatedData = new Dictionary<string, object>
            {
                { "RiskLevel", riskLevel },
                { "HazardType", hazardType },
                { "Recommendation", recommendation }
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