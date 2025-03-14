namespace ClearCare.Models.Interface {   
    public interface IMedicalRecordSubject
    {
        void addObserver(IMedicalRecordObserver observer);
        void removeObserver(IMedicalRecordObserver observer);
        Task notifyObservers();
    }
}
