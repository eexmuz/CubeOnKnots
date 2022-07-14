using System;
using System.Collections.Generic;
using Core.Attributes;
using UnityEngine;

namespace Core.Services
{
    [InjectionAlias(typeof(IDelayedCallService))]
    public class DelayedCallService : Service, IDelayedCallService
    {
        #region Fields

        public int DalayedCallsCount;

        private readonly List<DelayedCallbackItem> delayedCalls = new List<DelayedCallbackItem>();

        #endregion

        #region Public Methods and Operators

        public void DelayedCall(float delay, Action callback, bool checkNull = false)
        {
            delayedCalls.Add(new DelayedCallbackItem(delay + Time.realtimeSinceStartup, callback, checkNull));
            DalayedCallsCount = delayedCalls.Count;
        }

        //feature for windows platform
        public void DoInMainThread(Action callback)
        {
            delayedCalls.Add(new DelayedCallbackItem(0, callback));
            DalayedCallsCount = delayedCalls.Count;
        }

        public void RemoveDelayedCallsTo(Action callback)
        {
            for (var i = 0; i < delayedCalls.Count; i++)
                //if (delayedCalls[i].DelCallback.GetType() == callback.GetType() && delayedCalls[i].Id == id)
                if (delayedCalls[i].DelCallback == callback)
                {
                    delayedCalls.RemoveAt(i);
                    i--;
                }
        }

        #endregion

        #region Methods

        private static bool IsNull(object aObj)
        {
            return aObj == null || aObj.Equals(null);
        }

        private void Update()
        {
            Action delayedCall;
            bool checkNull;
            if (delayedCalls == null)
                return;

            for (var i = 0; i < delayedCalls.Count; i++)
                if (delayedCalls[i].Delay <= Time.realtimeSinceStartup)
                {
                    delayedCall = delayedCalls[i].DelCallback;
                    checkNull = delayedCalls[i].CheckNull;
                    //              delayedCalls[i].DelCallback();
                    if (delayedCalls.Count > i)
                    {
                        delayedCalls.RemoveAt(i);
                        i--;
                    }

                    if (delayedCall == null || checkNull && IsNull(delayedCall.Target))
                        Debug.Log(
                            "sorry delaycall listener are unavalible at the moment, please send sms or call again later");
                    else
                        delayedCall();
                }
            DalayedCallsCount = delayedCalls.Count;
        }

        #endregion

        private class DelayedCallbackItem
        {
            #region Fields

            public readonly bool CheckNull;
            public readonly float Delay;
            public readonly Action DelCallback;

            #endregion

            #region Constructors and Destructors

            //public string Id;

            public DelayedCallbackItem(float delay, Action callback, bool checkNull = false)
            {
                //Id = id;
                Delay = delay;
                DelCallback = callback;
                CheckNull = checkNull;
            }

            #endregion
        }
    }
}