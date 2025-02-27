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
        
        // Retrieve all medical records
        public async Task<List<MedicalRecord>> RetrieveAllMedicalRecords()
        {
            List<MedicalRecord> recordsList = new List<MedicalRecord>();
            QuerySnapshot snapshot = await db.Collection("MedicalRecords").GetSnapshotAsync();

            if (snapshot.Documents.Count == 0)
            {
                Console.WriteLine("No medical records found in Firestore.");
                return recordsList;
            }

            foreach (var doc in snapshot.Documents)
            {
                // Retrieve data from Firestore document
                string recordID = doc.Id;
                string doctorNote = doc.GetValue<string>("DoctorNote");
                Timestamp date = doc.GetValue<Timestamp>("Date");
                string patientID = doc.GetValue<string>("PatientID");
                byte[] attachment = doc.GetValue<byte[]>("Attachment");
                string attachmentName = doc.GetValue<string>("AttachmentName");
                string doctorID = doc.GetValue<string>("DoctorID");

                // Create a new MedicalRecord object with the Firestore data
                MedicalRecord record = new MedicalRecord(recordID, doctorNote, date, patientID, attachment, attachmentName, doctorID);

                // Add the newly created record to the list
                recordsList.Add(record);
            }
            return recordsList;
        }

        // Retrieve a medical record by ID
        public async Task<MedicalRecord> RetrieveMedicalRecordById(string recordID)
        {
            DocumentReference docRef = db.Collection("MedicalRecords").Document(recordID);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Console.WriteLine($"Medical record {recordID} not found in Firestore.");
                return null;
            }

            // Assign values from Firestore document
            string doctorNote = snapshot.GetValue<string>("DoctorNote");
            Timestamp date = snapshot.GetValue<Timestamp>("Date");
            string patientID = snapshot.GetValue<string>("PatientID");
            byte[] attachment = snapshot.GetValue<byte[]>("Attachment");
            string attachmentName = snapshot.GetValue<string>("AttachmentName");
            string doctorID = snapshot.GetValue<string>("DoctorID");

            // Return record
            return new MedicalRecord(recordID, doctorNote, date, patientID, attachment, attachmentName, doctorID);
        }

        // insert a medical record
        public async Task<MedicalRecord> InsertMedicalRecord(string doctorNote, string patientID,  byte[] fileBytes, string fileName, string doctorID)
        {
            try
            {
                CollectionReference medicalRecordsRef = db.Collection("MedicalRecords");

                // Fetch user document from Firestore
                DocumentReference userDocRef = db.Collection("User").Document(patientID);
                DocumentSnapshot userSnapshot = await userDocRef.GetSnapshotAsync();

                if (!userSnapshot.Exists)
                {
                    Console.WriteLine($"Error: No user found with UserID: {patientID}");
                    return null;
                }

                // Check if the role of the found user is "Patient"
                string role = userSnapshot.GetValue<string>("Role");

                if (!role.Equals("Patient", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Error: The user with UserID: {patientID} is not a Patient.");
                    return null;
                }

                // Generate current timestamp
                Timestamp currentTimestamp = Timestamp.FromDateTime(DateTime.UtcNow);

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

                // Add record to Firestore with unique document ID
                DocumentReference docRef = medicalRecordsRef.Document(newRecordID);

                // Prepare the data to insert into Firestore
                var medicalRecordData = new Dictionary<string, object>
                {
                    { "DoctorNote", doctorNote },
                    { "Date", currentTimestamp },
                    { "PatientID", patientID },
                    { "Attachment", fileBytes },
                    { "AttachmentName", fileName },
                    { "DoctorID", doctorID }
                };

                await docRef.SetAsync(medicalRecordData);

                Console.WriteLine($"Medical record inserted successfully with Firestore ID: {newRecordID}");

                // Return the record with the correct MedicalRecordID (Firestore document ID)
                return new MedicalRecord(newRecordID, doctorNote, currentTimestamp, patientID, fileBytes, fileName, doctorID);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inserting medical record: " + ex.Message);
                return null;
            }
        }

    }
}