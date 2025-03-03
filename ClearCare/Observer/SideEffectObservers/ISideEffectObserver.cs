using ClearCare.Models;

namespace ClearCare.Observers
{
    public interface ISideEffectObserver
    {
        void Update(SideEffectModel sideEffect, string eventType);
    }
}
