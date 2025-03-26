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

        public async Task<List<Erratum>> retrieveAllErratums()
        {
            List<Erratum> errata = new List<Erratum>();
            CollectionReference erratumRef = db.Collection("Erratum");
            QuerySnapshot snapshot = await erratumRef.GetSnapshotAsync();

            if (snapshot.Documents.Count == 0)
            {
                Console.WriteLine("No erratums found in Firestore.");
                return errata;
            }

            foreach (var doc in snapshot.Documents)
            {
                // Retrieve data from Firestore document
                string erratumID = doc.Id;  // Firestore document ID as the ErratumID
                string medicalRecordID = doc.GetValue<string>("MedicalRecordID");
                Timestamp date = doc.GetValue<Timestamp>("Date");
                string erratumDetails = doc.GetValue<string>("ErratumDetails");
                string doctorID = doc.GetValue<string>("DoctorID");
                byte[] attachment = doc.GetValue<byte[]>("ErratumAttachment");
                string attachmentName = doc.GetValue<string>("ErratumAttachmentName");

                // Create a new Erratum object with the Firestore data
                Erratum erratum = new Erratum(erratumID, medicalRecordID, date, erratumDetails, doctorID, attachment, attachmentName);

                // Add the newly created erratum to the list
                errata.Add(erratum);
            }

            return errata;
        }

        // Retrieve a medical record by ID
        public async Task<Erratum?> getErratumByID(string erratumID)
        {
            DocumentReference docRef = db.Collection("Erratum").Document(erratumID);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Console.WriteLine($"Erratum {erratumID} not found in Firestore.");
                return null;
            }

            // Assign values from Firestore document
            string medicalRecordID = snapshot.GetValue<string>("MedicalRecordID");
            Timestamp date = snapshot.GetValue<Timestamp>("Date");
            string erratumDetails = snapshot.GetValue<string>("ErratumDetails");
            string doctorID = snapshot.GetValue<string>("DoctorID");
            byte[] attachment = snapshot.GetValue<byte[]>("ErratumAttachment");
            string attachmentName = snapshot.GetValue<string>("ErratumAttachmentName");

            // Return record
            return new Erratum(erratumID, medicalRecordID, date, erratumDetails, doctorID, attachment, attachmentName);
        }

        public async Task<Erratum?> insertErratum(string medicalRecordID, string erratumDetails, string doctorID, byte[] fileBytes, string fileName)
        {
            try
            {
                CollectionReference erratumRef = db.Collection("Erratum");

                // Fetch medical record document from Firestore
                DocumentReference medDocRef = db.Collection("MedicalRecords").Document(medicalRecordID);
                DocumentSnapshot medSnapshot = await medDocRef.GetSnapshotAsync();

                if (!medSnapshot.Exists)
                {
                    Console.WriteLine($"Error: No Medical Record found with MedicalRecordID: {medicalRecordID}");
                    return null;
                }

                // Generate current timestamp
                Timestamp currentTimestamp = Timestamp.FromDateTime(DateTime.UtcNow);

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

                // Create Erratum Document in Firestore with unique erratum ID
                DocumentReference docRef = erratumRef.Document(erratumID);

                // Prepare the data to insert into Firestore
                var erratumData = new Dictionary<string, object>
                {
                    { "MedicalRecordID", medicalRecordID },
                    { "Date", currentTimestamp },
                    { "ErratumDetails", erratumDetails },
                    { "DoctorID", doctorID },
                    { "ErratumAttachment", fileBytes },
                    { "ErratumAttachmentName", fileName }
                };

                // Insert new Erratum record into Firestore 
                await docRef.SetAsync(erratumData);

                Console.WriteLine($"Erratum ID: {erratumID} inserted successfully.");

                // Return the record with the correct MedicalRecordID (Firestore document ID)
                return new Erratum(erratumID, medicalRecordID, currentTimestamp, erratumDetails, doctorID, fileBytes, fileName);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return null;
            }
        }
    }
}