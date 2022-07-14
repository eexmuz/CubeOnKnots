using System;

namespace Core.Services
{
    public interface IDelayedCallService : IService
    {
        #region Public Methods and Operators

        void DelayedCall(float delay, Action callback, bool checkNull = false);
        void DoInMainThread(Action callback);

        void RemoveDelayedCallsTo(Action callback);

        #endregion
    }
}