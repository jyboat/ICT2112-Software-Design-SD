using ClearCare.Models;
using ClearCare.Gateways;
using ClearCare.Observer;   // <--- Add this


namespace ClearCare.Controls
{
    // Implement ISubject<SideEffectModel>
    public class SideEffectControl : ISubject<SideEffectModel>
    {
        private readonly SideEffectsMapper _sideEffectsMapper;

        // A list to keep all observers
        private readonly List<Observer.IObserver<SideEffectModel>> _observers = new();

        public SideEffectControl(SideEffectsMapper sideEffectsMapper)
        {
            _sideEffectsMapper = sideEffectsMapper;
        }

        //=============================
        // ISubject<SideEffectModel>
        //=============================
        public void Attach(Observer.IObserver<SideEffectModel> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void Detach(Observer.IObserver<SideEffectModel> observer)
        {
            if (_observers.Contains(observer))
                _observers.Remove(observer);
        }

        // A private helper to notify all observers a side effect was created
        private void NotifyCreated(SideEffectModel sideEffect)
        {
            foreach (var obs in _observers)
            {
                obs.OnCreated(sideEffect);
            }
        }

        //=============================
        // Existing Methods
        //=============================
        public async Task<List<SideEffectModel>> GetSideEffectsAsync()
        {
            return await _sideEffectsMapper.GetAllSideEffectsAsync();
        }

        public async Task AddSideEffectAsync(SideEffectModel sideEffect)
        {
            // 1. Persist to Firestore
            await _sideEffectsMapper.AddSideEffectAsync(sideEffect);

            // 2. Notify all observers that a new side effect was created
            NotifyCreated(sideEffect);
        }
    }
}
