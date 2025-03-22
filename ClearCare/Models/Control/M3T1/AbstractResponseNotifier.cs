using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.DataSource;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T1;

public abstract class AbstractResponseNotifier
{
    private readonly List<IResponseObserver> _observers = new List<IResponseObserver>();

    public void Attach(IResponseObserver observer)
    {
        _observers.Add(observer);
    }

    public void Detach(IResponseObserver observer)
    {
        _observers.Remove(observer);
    }

    protected void Notify(string userId, string feedbackId)
    {
        foreach (var observer in _observers)
        {
            observer.Update(userId, feedbackId);
        }
    }
}

