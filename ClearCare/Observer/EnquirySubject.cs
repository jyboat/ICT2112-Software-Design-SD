using System.Collections.Generic;
using ClearCare.Models;

public class EnquirySubject
{
    private readonly List<IEnquiryObserver> _observers = new();

    // Add an observer
    public void Attach(IEnquiryObserver observer)
    {
        _observers.Add(observer);
    }

    // Remove an observer
    public void Detach(IEnquiryObserver observer)
    {
        _observers.Remove(observer);
    }

    // Notify all observers about an event
    public void Notify(Enquiry enquiry, string eventType)
    {
        foreach (var observer in _observers)
        {
            observer.Update(enquiry, eventType);
        }
    }
}
