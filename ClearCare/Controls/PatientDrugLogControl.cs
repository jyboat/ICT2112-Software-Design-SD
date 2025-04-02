using ClearCare.Interfaces;
using ClearCare.Models;
using ClearCare.Gateways;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Observer;

namespace ClearCare.Controls
{
    public class PatientDrugLogControl : ISubject<PatientDrugLogModel>
    {
        private readonly PatientDrugMapper _mapper;
        private readonly List<Observer.IObserver<PatientDrugLogModel>> _observers = new();

        /// <summary>
        ///   Initializes a new instance of the
        ///   <see cref="PatientDrugLogControl"/> class.
        /// </summary>
        /// <param name="mapper">The PatientDrugMapper instance for data
        ///   access.</param>
        public PatientDrugLogControl(PatientDrugMapper mapper)
        {
            _mapper = mapper;
        }

        //Observer
        /// <summary>
        ///   Attaches an observer to the subject for receiving drug log
        ///   notifications.
        /// </summary>
        /// <param name="observer">The observer to attach.</param>
        public void Attach(Observer.IObserver<PatientDrugLogModel> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        /// <summary>
        ///   Detaches an observer from the subject, removing it from the list
        ///   of notification receivers.
        /// </summary>
        /// <param name="observer">The observer to detach.</param>
        public void Detach(Observer.IObserver<PatientDrugLogModel> observer)
        {
            if (_observers.Contains(observer))
                _observers.Remove(observer);
        }

        /// <summary>
        ///   Notifies all attached observers that a new drug log entry has been
        ///   created.
        /// </summary>
        /// <param name="drugInfo">The newly created drug log entry.</param>
        private void notifyCreated(PatientDrugLogModel drugInfo)
        {
            foreach (var obs in _observers)
            {
                obs.OnCreated(drugInfo);
            }
        }

        /// <summary>
        ///   Retrieves the drug log for the current patient.
        /// </summary>
        /// <returns>
        ///   A list of <see cref="PatientDrugLogModel"/> representing the
        ///   patient's drug log.
        /// </returns>
        public async Task<List<PatientDrugLogModel>> getDrugLogAsync()
        {
            return await _mapper.GetDrugLogAsync();
        }

        /// <summary>
        ///   Retrieves the drug log for all patients (Doctor's view).
        /// </summary>
        /// <returns>
        ///   A list of <see cref="PatientDrugLogModel"/> representing the drug
        ///   logs of all patients.
        /// </returns>
        //Html stuff
        public async Task<List<PatientDrugLogModel>> getAllDrugLogAsync()
        {
            return await _mapper.GetAllDrugLogAsync();
        }

        /// <summary>
        ///   Uploads new drug information to the patient's drug log.
        /// </summary>
        /// <param name="drugInfo">The drug information to upload.</param>
        public async Task uploadDrugInfo(PatientDrugLogModel drugInfo)
        {
            await _mapper.UploadDrugInfo(drugInfo);
            notifyCreated(drugInfo);
        }
    }
}
