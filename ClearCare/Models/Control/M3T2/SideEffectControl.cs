using ClearCare.Observer;   // <--- Add this
using ClearCare.DataSource.M3T2;
using ClearCare.Models.Entities.M3T2;
using ClearCare.Models.Interfaces.M3T2;

namespace ClearCare.Models.Control.M3T2
{
    // Implements ISubject<SideEffectModel>
    public class SideEffectControl : ISubject<SideEffectModel>
    {
        private readonly SideEffectsMapper _sideEffectsMapper;
    private readonly IFetchPrescriptions _prescriptionFetcher;

        // A list to keep all observers
        private readonly List<Observer.IObserver<SideEffectModel>> _observers = new();

        public SideEffectControl(SideEffectsMapper sideEffectsMapper,IFetchPrescriptions prescriptionFetcher)
        {
            _sideEffectsMapper = sideEffectsMapper;
            _prescriptionFetcher = prescriptionFetcher;
        }

        //=============================
        // ISubject<SideEffectModel> Implementation
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

        //=============================
        // Private Notification Helper
        //=============================
        private void notifyCreated(SideEffectModel sideEffect)
        {
            foreach (var obs in _observers)
            {
                obs.OnCreated(sideEffect);
            }
        }

        //=============================
        // Public Methods
        //=============================
        public async Task<List<SideEffectModel>> getSideEffectsAsync(string userRole, string userUUID)
        {
            var allSideEffects = await _sideEffectsMapper.getAllSideEffectsAsync();

            if (userRole == "Patient")
            {
                return allSideEffects.Where(se => se.PatientId == userUUID).ToList();
            }
            
            return allSideEffects;
        }

        public async Task addSideEffectAsync(SideEffectModel sideEffect)
        {
            await _sideEffectsMapper.addSideEffectAsync(sideEffect);
            notifyCreated(sideEffect);
        }

        public async Task<List<DrugDosage>> getPatientMedications(string userRole, string userUUID)
        {
            if (userRole != "Patient")
                return new List<DrugDosage>();

            // Get all prescriptions and filter client-side
            var allPrescriptions = await _prescriptionFetcher.fetchPrescriptions();

            return allPrescriptions
                .Where(p => p.PatientId == userUUID)
                .SelectMany(p => p.Medications)
                .GroupBy(m => m.DrugName) 
                .Select(g => new DrugDosage
                {
                    DrugName = g.Key,
                    Dosage = ""
                })
                .ToList();
        }
    }
}
