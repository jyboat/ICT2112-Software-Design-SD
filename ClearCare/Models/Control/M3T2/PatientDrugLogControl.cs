using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Observer;
using ClearCare.DataSource.M3T2;
using ClearCare.Models.DTO.M3T2;
using ClearCare.Models.Entities.M3T2;

namespace ClearCare.Controls
{
    public class PatientDrugLogControl : ISubject<PatientDrugLog>
    {
        private readonly PatientDrugMapper _mapper;
        private readonly List<Observer.IObserver<PatientDrugLog>> _observers = new();

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
        public void Attach(Observer.IObserver<PatientDrugLog> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        /// <summary>
        ///   Detaches an observer from the subject, removing it from the list
        ///   of notification receivers.
        /// </summary>
        /// <param name="observer">The observer to detach.</param>
        public void Detach(Observer.IObserver<PatientDrugLog> observer)
        {
            if (_observers.Contains(observer))
                _observers.Remove(observer);
        }

        /// <summary>
        ///   Notifies all attached observers that a new drug log entry has been
        ///   created.
        /// </summary>
        /// <param name="drugInfo">The newly created drug log entry.</param>
        private void notifyCreated(PatientDrugLog drugInfo)
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
        public async Task<List<PatientDrugLog>> getDrugLogAsync()
        {
            return await _mapper.GetDrugLogAsync();
        }

        /// <summary>
        ///   Retrieves the drug log for all patients (Doctor's view).
        /// </summary>
        /// <returns>
        ///   A list of <see cref="PatientDrugLog"/> representing the drug
        ///   logs of all patients.
        /// </returns>
        //Html stuff
        public async Task<List<PatientDrugLog>> getAllDrugLogAsync()
        {
            return await _mapper.GetAllDrugLogAsync();
        }

        /// <summary>
        ///   Uploads new drug information to the patient's drug log.
        /// </summary>
        /// <param name="drugInfo">The drug information to upload.</param>
        public async Task uploadDrugInfo(PatientDrugLog drugInfo)
        {
            await _mapper.UploadDrugInfo(drugInfo);
            notifyCreated(drugInfo);
        }
    }
}
