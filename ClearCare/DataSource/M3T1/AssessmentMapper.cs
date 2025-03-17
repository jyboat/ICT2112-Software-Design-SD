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

        // Fetch all assessments
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
                        string riskLevel = doc.ContainsField("RiskLevel") ? doc.GetValue<string>("RiskLevel") : "";
                        string recommendation = doc.ContainsField("Recommendation") ? doc.GetValue<string>("Recommendation") : "";
                        DateTime createdAt = doc.ContainsField("CreatedAt") ? doc.GetValue<DateTime>("CreatedAt") : DateTime.Now;
                        string doctorId = doc.ContainsField("DoctorID") ? doc.GetValue<string>("DoctorID") : "";
                        string patientId = doc.ContainsField("PatientID") ? doc.GetValue<string>("PatientID") : "";
                        List<string> imagePath = doc.ContainsField("ImagePath") ? doc.GetValue<List<string>>("ImagePath") : new List<string>();

                        Assessment assessment = new Assessment(id, riskLevel, recommendation, createdAt, doctorId, patientId, imagePath);
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

        // Fetch assessment by ID
        public async Task<Assessment> fetchAssessmentById(string id)
        {
            DocumentReference docRef = _db.Collection("Assessment").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Console.WriteLine($"No assessment found with ID {id}");
                return null;
            }

            string riskLevel = snapshot.GetValue<string>("RiskLevel");
            string recommendation = snapshot.GetValue<string>("Recommendation");
            DateTime createdAt = snapshot.GetValue<DateTime>("CreatedAt");
            string doctorId = snapshot.GetValue<string>("DoctorID");
            string patientId = snapshot.GetValue<string>("PatientID");
            List<string> imagePath = snapshot.GetValue<List<string>>("ImagePath");

            return new Assessment(id, riskLevel, recommendation, createdAt, doctorId, patientId, imagePath);
        }

        // Insert assessment
        public async Task<string> insertAssessment(string riskLevel, string recommendation, string createdAt, string patientId,string doctorId, List<string> imagePath)
        {
            // Parse the createdAt string into a DateTime object
            if (!DateTime.TryParse(createdAt, out DateTime parsedDate))
            {
                throw new ArgumentException("Invalid date format.");
            }

            // Convert the DateTime object to a Firestore Timestamp
            var timestamp = Timestamp.FromDateTime(parsedDate.ToUniversalTime());

            DocumentReference docRef = _db.Collection("Assessment").Document();

            var assessmentData = new Dictionary<string, object>
            {
                { "RiskLevel", riskLevel },
                { "Recommendation", recommendation },
                { "CreatedAt", timestamp }, // Store as Firestore Timestamp
                { "DoctorID", doctorId},
                { "PatientID", patientId },
                { "ImagePath", imagePath }
            };

            await docRef.SetAsync(assessmentData);
            return docRef.Id;
        }
        // Update assessment
        public async Task<bool> updateAssessment(string id, string riskLevel, string recommendation, string createdAt, List<string> imagePath)
        {
            DocumentReference docRef = _db.Collection("Assessment").Document(id);

            var updatedData = new Dictionary<string, object>
            {
                { "RiskLevel", riskLevel },
                { "Recommendation", recommendation },
                { "CreatedAt", createdAt },
                { "ImagePath", imagePath }
            };

            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                return false;
            }

            await docRef.UpdateAsync(updatedData);
            return true;
        }

        // Delete assessment
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
    }
}