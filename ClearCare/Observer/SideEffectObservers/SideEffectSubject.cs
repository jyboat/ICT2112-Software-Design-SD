using System;
using System.Collections.Generic;
using ClearCare.Models;

namespace ClearCare.Observers
{
    public class SideEffectSubject
    {
        private readonly List<ISideEffectObserver> _observers = new();

        // Add an observer
        public void Attach(ISideEffectObserver observer)
        {
            _observers.Add(observer);
            Console.WriteLine($"[SideEffectSubject] Observer attached. Total observers: {_observers.Count}");
        }

        // Remove an observer
        public void Detach(ISideEffectObserver observer)
        {
            _observers.Remove(observer);
            Console.WriteLine($"[SideEffectSubject] Observer detached. Total observers: {_observers.Count}");
        }

        // Notify all observers about an event
        public void Notify(SideEffectModel sideEffect, string eventType)
        {
            Console.WriteLine($"[SideEffectSubject] Notifying observers about event: {eventType}, Drug Name: {sideEffect.DrugName}");

            foreach (var observer in _observers)
            {
                try
                {
                    observer.Update(sideEffect, eventType);
                    Console.WriteLine($"[SideEffectSubject] Observer notified successfully for event: {eventType}, Drug Name: {sideEffect.DrugName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SideEffectSubject] Error notifying observer: {ex.Message}");
                }
            }
        }
    }
}
