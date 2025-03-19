using ClearCare.DataSource;

namespace ClearCare.Interfaces
{
    public interface IDatabaseObserver
    {
        void update(Subject subject, object data);
    }
}
