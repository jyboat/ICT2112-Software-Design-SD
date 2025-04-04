using ClearCare.Models.Entities;

namespace ClearCare.Interfaces
{
    public interface ISchedulingListener
    {
        public Task update(string appointmentID, string eventType);
    }
}