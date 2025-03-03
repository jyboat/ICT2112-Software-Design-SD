using System;
using System.Collections.Generic;
using ClearCare.Models;

public class EnquirySubject
{
    private readonly List<IEnquiryObserver> _observers = new();

    // Add an observer
    public void Attach(IEnquiryObserver observer)
    {
        if (observer == null)
        {
            Console.WriteLine("[EnquirySubject] Attempted to attach a null observer.");
            return;
        }

        _observers.Add(observer);
        Console.WriteLine($"[EnquirySubject] Observer attached. Total observers: {_observers.Count}");
    }

    // Remove an observer
    public void Detach(IEnquiryObserver observer)
    {
        if (observer == null)
        {
            Console.WriteLine("[EnquirySubject] Attempted to detach a null observer.");
            return;
        }

        if (_observers.Remove(observer))
        {
            Console.WriteLine($"[EnquirySubject] Observer detached. Total observers: {_observers.Count}");
        }
        else
        {
            Console.WriteLine("[EnquirySubject] Attempted to detach an observer that was not attached.");
        }
    }

    // Notify all observers about an event
    public void Notify(Enquiry enquiry, string eventType)
    {
        if (enquiry == null)
        {
            Console.WriteLine("[EnquirySubject] Attempted to notify observers with a null enquiry.");
            return;
        }

        Console.WriteLine($"[EnquirySubject] Notifying observers about event: {eventType}, Enquiry ID: {enquiry.FirestoreId}");

        foreach (var observer in _observers)
        {
            try
            {
                observer.Update(enquiry, eventType);
                Console.WriteLine($"[EnquirySubject] Observer notified successfully for event: {eventType}, Enquiry ID: {enquiry.FirestoreId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EnquirySubject] Error notifying observer: {ex.Message}");
            }
        }
    }
}
