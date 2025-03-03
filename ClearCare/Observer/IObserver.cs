namespace ClearCare.Observer
{
    /// <summary>
    /// Generic Observer interface that can watch changes to any entity type T.
    /// </summary>
    /// <typeparam name="T">The entity type (e.g., Enquiry, SideEffect, etc.)</typeparam>
    public interface IObserver<T>
    {
        void OnCreated(T entity);
        void OnUpdated(T entity);
        void OnDeleted(string entityId);
    }
}
