using ClearCare.Models;
using ClearCare.Observer;
using System;

namespace ClearCare.Observers
{
    /// <summary>
    /// Example observer that logs side effect changes to the console.
    /// </summary>
    public class SideEffectLoggingObserver : Observer.IObserver<SideEffect>
    {
        public void OnCreated(SideEffect entity)
        {
            Console.WriteLine($"[SideEffectLoggingObserver] Created: {entity.Id} - {entity.Name}");
        }

        public void OnUpdated(SideEffect entity)
        {
            Console.WriteLine($"[SideEffectLoggingObserver] Updated: {entity.Id} - {entity.Name}");
        }

        public void OnDeleted(string entityId)
        {
            Console.WriteLine($"[SideEffectLoggingObserver] Deleted: {entityId}");
        }
    }
}
