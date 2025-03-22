using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.DataSource;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T1;

public class PatientNotificationObserver : IResponseObserver
{
    public void Update(string userId, string feedbackId)
    {
        Console.WriteLine($"Response to Feedback {feedbackId} by {userId} has been modified.");
    }
}

