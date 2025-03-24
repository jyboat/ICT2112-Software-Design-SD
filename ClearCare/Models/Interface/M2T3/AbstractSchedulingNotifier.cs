using System.Diagnostics;
using System.Net.Sockets;
using ClearCare.Interfaces;

namespace ClearCare.Models.Interface.M2T3
{
    public abstract class AbstractSchedulingNotifier
    {
        private List<ISchedulingListener> listeners = new List<ISchedulingListener>();

        public void attach(ISchedulingListener listener)
        {
            listeners.Add(listener);
        }
        public void detach(ISchedulingListener listener)
        {
            listeners.Remove(listener);
        }
        protected void notify(string appointmentID, string eventType)
        {
            foreach (var listener in listeners)
            {
                listener.update(appointmentID, eventType);
            }
        }
    }
}