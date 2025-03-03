namespace ClearCare.Observer
{
    public interface IObserver<T>
    {
        void OnCreated(T entity);
        void OnUpdated(T entity);
        void OnDeleted(string entityId);
    }
}
