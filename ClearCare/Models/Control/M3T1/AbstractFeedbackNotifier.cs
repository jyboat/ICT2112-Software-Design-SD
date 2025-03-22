using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.DataSource;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T1;

public abstract class AbstractFeedbackNotifier
{
    private readonly List<IFeedbackObserver> _observers = new List<IFeedbackObserver>();

    public void Attach(IFeedbackObserver observer)
    {
        _observers.Add(observer);
    }

    public void Detach(IFeedbackObserver observer)
    {
        _observers.Remove(observer);
    }

    protected void Notify(string feedbackId)
    {
        foreach (var observer in _observers)
        {
            observer.Update(feedbackId);
        }
    }
}

