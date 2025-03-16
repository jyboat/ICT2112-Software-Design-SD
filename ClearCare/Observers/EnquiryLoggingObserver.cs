using ClearCare.Models;
using ClearCare.Observer;
using System;

namespace ClearCare.Observers
{
    /// <summary>
    /// Example observer that logs whenever an Enquiry is created/updated/deleted.
    /// </summary>
    public class EnquiryLoggingObserver : Observer.IObserver<Enquiry>
    {
        public void OnCreated(Enquiry enquiry)
        {
            Console.WriteLine($"[EnquiryLoggingObserver] Created:{enquiry.Name}");
        }

        public void OnUpdated(Enquiry enquiry)
        {
            Console.WriteLine($"[EnquiryLoggingObserver] Updated: {enquiry.Name}");
        }

        public void OnDeleted(string enquiryId)
        {
            Console.WriteLine($"[EnquiryLoggingObserver] Deleted: {enquiryId}");
        }
    }
}
