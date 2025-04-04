namespace ClearCare.Observer
{
    /// <summary>
    ///   Generic Observer interface that can watch changes to any entity type
    ///   T.
    /// </summary>
    /// <typeparam name="T">
    ///   The entity type (e.g., Enquiry, SideEffect, etc.)
    /// </typeparam>
    public interface IObserver<T>
    {
        /// <summary>
        ///   Called when a new entity of type T is created.
        /// </summary>
        /// <param name="entity">The newly created entity.</param>
        void OnCreated(T entity);

        /// <summary>
        ///   Called when an existing entity of type T is updated.
        /// </summary>
        /// <param name="entity">The updated entity.</param>
        void OnUpdated(T entity);

        /// <summary>
        ///   Called when an entity of type T is deleted.
        /// </summary>
        /// <param name="entityId">The ID of the deleted entity.</param>
        void OnDeleted(string entityId);
    }
}
