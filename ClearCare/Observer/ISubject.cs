namespace ClearCare.Observer
{
    /// <summary>
    ///   Generic Subject interface for managing observer subscriptions.
    /// </summary>
    /// <typeparam name="T">
    ///   The entity type observed (e.g., Enquiry).
    /// </typeparam>
    public interface ISubject<T>
    {
        /// <summary>
        ///   Attaches an observer to the subject for receiving notifications.
        /// </summary>
        /// <param name="observer">The observer to attach.</param>
        void Attach(IObserver<T> observer);

        /// <summary>
        ///   Detaches an observer from the subject, removing it from the list
        ///   of notification receivers.
        /// </summary>
        /// <param name="observer">The observer to detach.</param>
        void Detach(IObserver<T> observer);
    }
}
