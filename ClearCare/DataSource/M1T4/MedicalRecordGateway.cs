using System.IO;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using ClearCare.Models.Entities;
using Google.Protobuf;

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

        // userID is patientID
        public async Task<List<MedicalRecord>> findMedicalRecordsByUserID(string userID)
        {
            if (db == null)
            {
                throw new Exception("Database connection is not initialized.");
            }

            List<MedicalRecord> recordsList = new List<MedicalRecord>();
            Query query = db.Collection("MedicalRecords").WhereEqualTo("PatientID", userID);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            if (snapshot.Documents.Count == 0)
            {
                return recordsList;
            }

            foreach (var doc in snapshot.Documents)
            {
                string recordID = doc.Id;
                string doctorNote = doc.GetValue<string>("DoctorNote");
                Timestamp timestamp = doc.GetValue<Timestamp>("Date");
                DateTime date = timestamp.ToDateTime();

                byte[] attachment = new byte[0]; // Default empty array

                if (doc.ContainsField("Attachment") && doc.GetValue<object>("Attachment") != null)
                {
                    var blobData = doc.GetValue<ByteString>("Attachment"); 

                    if (blobData != null)
                    {
                        attachment = blobData.ToByteArray(); 
                    }
                }

                string attachmentName = doc.ContainsField("AttachmentName")
                                ? doc.GetValue<string>("AttachmentName")
                                : string.Empty;

                string doctorID = doc.GetValue<string>("DoctorID");

                Timestamp firestoreTimestamp = Timestamp.FromDateTime(date.ToUniversalTime());

                MedicalRecord record = new MedicalRecord(recordID, doctorNote, firestoreTimestamp, userID, attachment, attachmentName, doctorID);
                recordsList.Add(record);
            }

            return recordsList;
        }

        // Retrieve all medical records
        public async Task<List<MedicalRecord>> retrieveAllMedicalRecords()
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
        public async Task<MedicalRecord> retrieveMedicalRecordById(string recordID)
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
        public async Task<MedicalRecord> insertMedicalRecord(string doctorNote, string patientID, byte[] fileBytes, string fileName, string doctorID)
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

                // Generate Medical Record ID
                string MedicalRecordID = $"MD{highestID + 1}";

                // Create MedRecord Document in Firestore with unique MedRecord ID
                DocumentReference docRef = medicalRecordsRef.Document(MedicalRecordID);

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

                Console.WriteLine($"Medical record inserted successfully with Firestore ID: {MedicalRecordID}");

                // Return the record with the correct MedicalRecordID (Firestore document ID)
                return new MedicalRecord(MedicalRecordID, doctorNote, currentTimestamp, patientID, fileBytes, fileName, doctorID);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inserting medical record: " + ex.Message);
                return null;
            }
        }

    }
}