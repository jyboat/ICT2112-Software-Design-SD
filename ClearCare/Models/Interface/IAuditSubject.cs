namespace ClearCare.Models.Interface
{
    public interface IAuditSubject
    {
        void addObserver(IAuditObserver observer);
        void removeObserver(IAuditObserver observer);
        Task notifyObservers(); // ✅ Ensure it's asynchronous, like your medical record system
        
        Task<string> insertAuditLog(string action, string performedBy);
    }
}
