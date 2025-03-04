using Google.Cloud.Firestore;
using System;
using System.Runtime.InteropServices;

public class FirebaseService
{
    private static FirestoreDb? _firestoreDb;

    public static FirestoreDb Initialize()
    {
        if (_firestoreDb == null)
        {
            // Determine the OS and set the file path accordingly
            string path;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // Path for MacBook
                path = @"Firebase/firebase-adminsdk.json";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Path for Windows
                path = @"C:\Firebase\firebase-adminsdk.json";
            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported operating system.");
            }

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            _firestoreDb = FirestoreDb.Create("ict2112");
            Console.WriteLine("Connected to Firestore!");
        }

        return _firestoreDb;
    }
}