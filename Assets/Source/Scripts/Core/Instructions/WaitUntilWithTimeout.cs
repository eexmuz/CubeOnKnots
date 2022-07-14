using System;
using UnityEngine;

namespace Core.Instructions
{
    /// <summary>
    ///     Wait until the predicate returns true or time out
    /// </summary>
    public class WaitUntilWithTimeout : CustomYieldInstruction
    {
        #region Fields

        private readonly Func<bool> _predicate;
        private readonly OperationResult _result;
        private readonly float _timeOutTime;

        #endregion

        #region Constructors and Destructors

        public WaitUntilWithTimeout(Func<bool> predicate, float timeOutS, OperationResult result = null)
        {
            if (result != null)
            {
                _result = result;
                _result.Reset();
            }

            _predicate = predicate;
            _timeOutTime = Time.time + timeOutS;
        }

        #endregion

        #region Public Properties

        public override bool keepWaiting
        {
            get
            {
                if (Time.time >= _timeOutTime)
                {
                    if (_result != null) _result.Error = new Exception("Operation timed out");
                    return false;
                }

                return _predicate();
            }
        }

        #endregion
    }
}