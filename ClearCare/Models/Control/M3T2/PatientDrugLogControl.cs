using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Observer;
using ClearCare.DataSource.M3T2;
using ClearCare.Models.DTO.M3T2;

namespace ClearCare.Controls
{
    public class PatientDrugLogControl : ISubject<PatientDrugLogDTO>
    {
        private readonly PatientDrugMapper _mapper;
        private readonly List<Observer.IObserver<PatientDrugLogDTO>> _observers = new();

        //Observer 
        public void Attach(Observer.IObserver<PatientDrugLogDTO> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void Detach(Observer.IObserver<PatientDrugLogDTO> observer)
        {
            if (_observers.Contains(observer))
                _observers.Remove(observer);
        }

        private void notifyCreated(PatientDrugLogDTO drugInfo)
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

        public async Task<List<PatientDrugLogDTO>> getDrugLogAsync() {
            return await _mapper.GetDrugLogAsync();
        }

        //Html stuff
        public async Task<List<PatientDrugLogDTO>> getAllDrugLogAsync() {
            return await _mapper.GetAllDrugLogAsync();
        }

        public async Task uploadDrugInfo(PatientDrugLogDTO drugInfo) {
            await _mapper.UploadDrugInfo(drugInfo);
            notifyCreated(drugInfo);
        }
    }
}
