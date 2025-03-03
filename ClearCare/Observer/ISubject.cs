namespace ClearCare.Observer
{
    /// <summary>
    /// Generic Subject interface for managing observer subscriptions.
    /// </summary>
    /// <typeparam name="T">The entity type observed (e.g., Enquiry).</typeparam>
    public interface ISubject<T>
    {
        void Attach(IObserver<T> observer);
        void Detach(IObserver<T> observer);
    }
}
