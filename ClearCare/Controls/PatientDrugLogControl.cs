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

        //Observer 
        public void Attach(Observer.IObserver<PatientDrugLogModel> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void Detach(Observer.IObserver<PatientDrugLogModel> observer)
        {
            if (_observers.Contains(observer))
                _observers.Remove(observer);
        }

        private void notifyCreated(PatientDrugLogModel drugInfo)
        {
            foreach (var obs in _observers)
            {
                obs.OnCreated(drugInfo);
            }
        }

        //Mapper
        public PatientDrugLogControl(PatientDrugMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<List<PatientDrugLogModel>> getDrugLogAsync() {
            return await _mapper.getDrugLogAsync();
        }

        //Html stuff
        public async Task<List<PatientDrugLogModel>> getAllDrugLogAsync() {
            return await _mapper.getAllDrugLogAsync();
        }

        public async Task uploadDrugInfo(PatientDrugLogModel drugInfo) {
            await _mapper.uploadDrugInfo(drugInfo);
            notifyCreated(drugInfo);
        }
    }
}
