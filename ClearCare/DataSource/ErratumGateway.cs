using System.IO;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using ClearCare.Models.Entities;

namespace ClearCare.DataSource
{
    public class ErratumGateway
    {
        private FirestoreDb db;

        public ErratumGateway()
        {
            // Initialize Firebase
            db = FirebaseService.Initialize();
        }

        // Retrieve all erratums
        public async Task<List<Erratum>> FindErratum()
        {
            CollectionReference erratumRef = db.Collection("Erratum");
            QuerySnapshot snapshot = await erratumRef.GetSnapshotAsync();

            List<Erratum> errata = new List<Erratum>();
            foreach (var doc in snapshot.Documents)
            {
                Erratum erratum = doc.ConvertTo<Erratum>();
                errata.Add(erratum);
            }
            return errata;
        }

        public async Task<Erratum?> InsertErratum(string medicalRecordID, string erratumDetails, string doctorID)
        {
            try
            {
                CollectionReference erratumRef = db.Collection("Erratum");

                // Check if medicalRecordID exists in MedicalRecord collection
                CollectionReference medicalRecordsRef = db.Collection("MedicalRecords");
                QuerySnapshot medicalRecordSnapshot = await medicalRecordsRef
                    .WhereEqualTo("MedicalRecordID", medicalRecordID)
                    .Limit(1)
                    .GetSnapshotAsync();

                if (medicalRecordSnapshot.Documents.Count == 0)
                {
                    Console.WriteLine($"Error: No Medical Record found with MedicalRecordID: {medicalRecordID}");
                    return null;
                }

                // Find the highest existing ERx number
                QuerySnapshot allErratumSnapshot = await erratumRef.GetSnapshotAsync();
                int highestID = 0;

                foreach (var doc in allErratumSnapshot.Documents)
                {
                    string docID = doc.Id; // Example: "ER3"
                    if (docID.StartsWith("ER") && int.TryParse(docID.Substring(2), out int id))
                    {
                        highestID = Math.Max(highestID, id);
                    }
                }

                // Generate Erratum ID
                string erratumID = $"ER{highestID + 1}";

                // Generate current date
                Timestamp date = Timestamp.FromDateTime(DateTime.UtcNow);

                // Create new Erratum record
                Erratum erratum = new Erratum(erratumID, medicalRecordID, date, erratumDetails, doctorID);

                // Insert new Erratum record into Firestore with unique erratum ID
                DocumentReference docRef = erratumRef.Document(erratumID);
                await docRef.SetAsync(erratum);

                Console.WriteLine($"Erratum ID: {erratumID} inserted successfully.");

                return erratum;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return null;
            }
        }
    }
}