using ClearCare.Models;

public class LoggingObserver : IEnquiryObserver
{
    public void Update(Enquiry enquiry, string eventType)
    {
        Console.WriteLine($"[LOG] Event: {eventType}, Enquiry ID: {enquiry.FirestoreId}, Name: {enquiry.Name}");
    }

 
}
