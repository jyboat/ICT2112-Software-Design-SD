using Google.Cloud.Firestore;
using System;

public class FirebaseService
{
    private static FirestoreDb? _firestoreDb;

    public static FirestoreDb Initialize()
    {
        if (_firestoreDb == null)
        {
            // Firebase Admin SDK JSON file
            string path = @"Firebase\firebase-adminsdk.json"; 
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            _firestoreDb = FirestoreDb.Create("sd-m2t5");
            Console.WriteLine("Connected to Firestore!");
        }

        return _firestoreDb;
    }
}
