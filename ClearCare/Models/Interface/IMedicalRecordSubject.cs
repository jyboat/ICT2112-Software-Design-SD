namespace ClearCare.Models.Interface {   
    public interface IMedicalRecordSubject
    {
        void AddObserver(IMedicalRecordObserver observer);
        void RemoveObserver(IMedicalRecordObserver observer);
        Task NotifyObservers();
    }
}
