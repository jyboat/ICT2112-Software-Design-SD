using ClearCare.Interfaces;

namespace ClearCare.DataSource
{
    // Base class representing a subject in the Observer pattern.
    public abstract class Subject
    {
        public abstract void attachObserver(IDatabaseObserver observer);
        public abstract void detachObserver(IDatabaseObserver observer);
        public abstract void notifyObservers(object data);
    }

    // Abstract class implementing the subject behavior.
    public abstract class AbstractDatabaseSubject : Subject
    {
        protected List<IDatabaseObserver> _observers = new List<IDatabaseObserver>();

        public override void attachObserver(IDatabaseObserver observer)
        {
            _observers.Add(observer);
        }

        public override void detachObserver(IDatabaseObserver observer)
        {
            _observers.Remove(observer);
        }

        public override void notifyObservers(object data)
        {
            foreach (var observer in _observers)
            {
                observer.update(this, data);
            }
        }
    }
}