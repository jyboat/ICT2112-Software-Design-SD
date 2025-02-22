using System.IO;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using ClearCare.Models.Entities;

namespace ClearCare.DataSource
{
    public class MedicalRecordGateway
    {
        private FirestoreDb db;

        public MedicalRecordGateway()
        {
            // Initialize Firebase
            db = FirebaseService.Initialize();
        }

        public async Task<MedicalRecord> InsertMedicalRecord(string doctorNote, string patientID)
        {
            try
            {
                CollectionReference medicalRecordsRef = db.Collection("MedicalRecords");

                // Check if patientID exists in User collection
                CollectionReference usersRef = db.Collection("User");
                QuerySnapshot userSnapshot = await usersRef
                    .WhereEqualTo("UserID", patientID)
                    .Limit(1)
                    .GetSnapshotAsync();

                if (userSnapshot.Documents.Count == 0)
                {
                    Console.WriteLine($"Error: No user found with UserID: {patientID}");
                    return null;
                }

                // Check if the role of the found user is "Patient"
                DocumentSnapshot userDoc = userSnapshot.Documents[0];
                string role = userDoc.GetValue<string>("Role");

                if (!role.Equals("Patient", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Error: The user with UserID: {patientID} is not a Patient.");
                    return null;
                }

                // Find the highest existing MDx number
                QuerySnapshot allRecordsSnapshot = await medicalRecordsRef.GetSnapshotAsync();
                int highestID = 0;

                foreach (var doc in allRecordsSnapshot.Documents)
                {
                    string docID = doc.Id; // Example: "MD3"
                    if (docID.StartsWith("MD") && int.TryParse(docID.Substring(2), out int id))
                    {
                        highestID = Math.Max(highestID, id);
                    }
                }

                int newMedicalRecordID = highestID + 1;
                string newRecordID = $"MD{newMedicalRecordID}";

                // Generate current timestamp
                Timestamp currentTimestamp = Timestamp.FromDateTime(DateTime.UtcNow);

                // Create a new medical record
                MedicalRecord record = new MedicalRecord(doctorNote, currentTimestamp, patientID);

                // Add record to Firestore with unique document ID
                DocumentReference docRef = medicalRecordsRef.Document(newRecordID);
                await docRef.SetAsync(record);

                Console.WriteLine($"Medical record inserted successfully with Firestore ID: {newRecordID}");

                return record;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inserting medical record: " + ex.Message);
                return null;
            }
        }





    }
}