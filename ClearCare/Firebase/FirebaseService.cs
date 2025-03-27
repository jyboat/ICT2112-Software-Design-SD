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
                path = @"Firebase/ict2112-firebase-adminsdk-fbsvc-168ada7053.json";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Path for Windows
                path = @"Firebase\ict2112-firebase-adminsdk-fbsvc-168ada7053.json"; // not to make path absolute with "C: "
                // path = @"Firebase\ict2112-firebase-adminsdk-fbsvc-168ada7053"; // not to make path absolute with "C: "

            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported operating system.");
            }

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            _firestoreDb = FirestoreDb.Create("ict2112");
            // _firestoreDb = FirestoreDb.Create("sd-m2t5");
            // _firestoreDb = FirestoreDb.Create("hello-world-f8882");
            Console.WriteLine("Connected to Firestore!");
        }

        return _firestoreDb;
    }
}
