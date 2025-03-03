using ClearCare.Models;

public interface IEnquiryObserver
{
    void Update(Enquiry enquiry, string eventType);
}
