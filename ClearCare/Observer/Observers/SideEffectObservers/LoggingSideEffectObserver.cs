using ClearCare.Models;
using System;

namespace ClearCare.Observers
{
    public class LoggingSideEffectObserver : ISideEffectObserver
    {
        public void Update(SideEffectModel sideEffect, string eventType)
        {
            Console.WriteLine($"[LoggingObserver] Event: {eventType}, Drug Name: {sideEffect.DrugName}, Patient ID: {sideEffect.PatientId}");
        }
    }
}
