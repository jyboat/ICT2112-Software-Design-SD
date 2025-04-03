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

        /// <summary>
        ///   Initializes a new instance of the <see cref="SideEffectControl"/>
        ///   class.
        /// </summary>
        /// <param name="sideEffectsMapper">The SideEffectsMapper instance for
        ///   data access.</param>
        /// <param name="prescriptionFetcher">The IFetchPrescriptions instance
        ///   for fetching prescription data.</param>
        public SideEffectControl(
            SideEffectsMapper sideEffectsMapper,
            IFetchPrescriptions prescriptionFetcher
        )
        {
            _sideEffectsMapper = sideEffectsMapper;
            _prescriptionFetcher = prescriptionFetcher;
        }

        //=============================
        // ISubject<SideEffectModel> Implementation
        //=============================
        /// <summary>
        ///   Attaches an observer to the subject for receiving side effect
        ///   notifications.
        /// </summary>
        /// <param name="observer">The observer to attach.</param>
        public void Attach(Observer.IObserver<SideEffectModel> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        /// <summary>
        ///   Detaches an observer from the subject, removing it from the list
        ///   of notification receivers.
        /// </summary>
        /// <param name="observer">The observer to detach.</param>
        public void Detach(Observer.IObserver<SideEffectModel> observer)
        {
            if (_observers.Contains(observer))
                _observers.Remove(observer);
        }

        //=============================
        // Private Notification Helper
        //=============================
        /// <summary>
        ///   Notifies all attached observers that a new side effect has been
        ///   created.
        /// </summary>
        /// <param name="sideEffect">The newly created side effect.</param>
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
        /// <summary>
        ///   Retrieves all side effects, filtered by user role and UUID.
        /// </summary>
        /// <param name="userRole">The role of the user (e.g., "Patient").</param>
        /// <param name="userUUID">The UUID of the user.</param>
        /// <returns>
        ///   A list of <see cref="SideEffectModel"/> representing the side
        ///   effects for the specified user.
        /// </returns>
        public async Task<List<SideEffectModel>> getSideEffectsAsync(
            string userRole,
            string userUUID
        )
        {
            var allSideEffects = await _sideEffectsMapper.getAllSideEffectsAsync();

            if (userRole == "Patient")
            {
                return allSideEffects.Where(se => se.PatientId == userUUID).ToList();
            }

            return allSideEffects;
        }

        /// <summary>
        ///   Adds a new side effect.
        /// </summary>
        /// <param name="sideEffect">The <see cref="SideEffectModel"/>
        ///   containing the side effect data.</param>
        public async Task addSideEffectAsync(SideEffectModel sideEffect)
        {
            await _sideEffectsMapper.addSideEffectAsync(sideEffect);
            notifyCreated(sideEffect);
        }

        /// <summary>
        ///   Retrieves a list of medications for a given patient.
        /// </summary>
        /// <param name="userRole">The role of the user (must be
        ///   "Patient").</param>
        /// <param name="userUUID">The UUID of the user.</param>
        /// <returns>
        ///   A list of <see cref="DrugDosage"/> representing the patient's
        ///   medications. Returns an empty list if the user is not a
        ///   patient.
        /// </returns>
        public async Task<List<DrugDosage>> getPatientMedications(
            string userRole,
            string userUUID
        )
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
