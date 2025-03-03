using ClearCare.Models;

public class NotificationObserver : IEnquiryObserver
{
    public void Update(Enquiry enquiry, string eventType)
    {
        if (eventType == "Created")
        {
            Console.WriteLine($"[NOTIFICATION] New enquiry created by {enquiry.Name}.");
        }
        else if (eventType == "Replied")
        {
            Console.WriteLine($"[NOTIFICATION] A reply was added to the enquiry by {enquiry.Name}.");
        }
    }

   
}
