namespace ClearCare.Models.Interface
{
    public interface IAuditSubject
    {
        void AddObserver(IAuditObserver observer);
        void RemoveObserver(IAuditObserver observer);
        Task NotifyObservers(); // ✅ Ensure it's asynchronous, like your medical record system
        
        Task<string> InsertAuditLog(string action, string performedBy);
    }
}
