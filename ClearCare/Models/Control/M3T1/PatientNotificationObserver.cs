using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.DataSource;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T1;
using Google.Cloud.Firestore;

public class PatientNotificationObserver : IFeedbackObserver
{
    // Static dict to keep track of feedback responses per patient (UserId, List of FeedbackIds)
    // Used for temp notif storage until patient visits feedback page
    private static Dictionary<string, List<string>> NotificationMap = new Dictionary<string, List<string>>();

    public void update(string feedbackId)
    {
        FirestoreDb db = FirebaseService.Initialize();
        var feedbackDoc = db.Collection("Feedback").Document(feedbackId).GetSnapshotAsync().Result;
        string userId = feedbackDoc.GetValue<string>("UserId");

        // Initialize list if they dont have pending notifs
        if (!NotificationMap.ContainsKey(userId))
        {
            NotificationMap[userId] = new List<string>();
        }

        // Add feedbackId to notif list
        // Indicates that feedback has been responded to and patient should be notified
        NotificationMap[userId].Add(feedbackId);
    }

    // Checks if user has any pending feedback response notifications, and clears it if true.
    public static bool checkAndClearNotification(string userId)
    {
        if (NotificationMap.ContainsKey(userId))
        {
            NotificationMap.Remove(userId);
            return true;
        }
        return false;
    }
}

