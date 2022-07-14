using System;

namespace Core
{
    public class BaseCommand
    {
        #region Public Events

        public event Action<bool> Complete;

        #endregion

        #region Public Methods and Operators

        public virtual void DispatchComplete(bool success)
        {
            Complete?.Invoke(success);
        }

        public virtual void Execute()
        {
        }

        #endregion
    }
}