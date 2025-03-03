using ClearCare.Models;
using ClearCare.Observer;
using ClearCare.Gateways;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ClearCare.Controls
{
    public class SideEffectControl : ISubject<SideEffect>
    {
        private readonly ILogger<SideEffectControl> _logger;
        private readonly SideEffectGateway _gateway;  // <--- inject your new gateway

        private static readonly List<SideEffect> _sideEffects = new();
        private readonly List<Observer.IObserver<SideEffect>> _observers = new();

        public SideEffectControl(ILogger<SideEffectControl> logger, SideEffectGateway gateway)
        {
            _logger = logger;
            _gateway = gateway;
        }

        // Attach/Detach for the observer pattern
        public void Attach(Observer.IObserver<SideEffect> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }
        public void Detach(Observer.IObserver<SideEffect> observer)
        {
            if (_observers.Contains(observer))
            {
                _observers.Remove(observer);
            }
        }

        private void NotifyCreated(SideEffect sideEffect)
        {
            foreach (var obs in _observers)
            {
                obs.OnCreated(sideEffect);
            }
        }

        // ... NotifyUpdated, NotifyDeleted if needed ...

        public async Task CreateSideEffectAsync(SideEffect sideEffect)
        {
            _logger.LogInformation($"Creating side effect: {sideEffect.Name}");
            
            // If you want an in-memory ID, or rely on Firestore doc ID
            sideEffect.Id = Guid.NewGuid().ToString();

            _sideEffects.Add(sideEffect);
            await _gateway.AddSideEffectAsync(sideEffect);

            NotifyCreated(sideEffect);
        }

        public async Task<List<SideEffect>> FetchAllSideEffectsAsync()
        {
            // Also fetch from Firestore
            var dbSideEffects = await _gateway.GetAllSideEffectsAsync();

            // If you want to keep an in-memory list in sync, you could do so here.
            // Or you could rely solely on Firestore data.

            return dbSideEffects;  // or merge with _sideEffects if you prefer
        }
    }
}
