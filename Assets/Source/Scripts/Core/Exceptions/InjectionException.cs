using System;

namespace Core.Exceptions
{
    /// <summary>
    ///     Exceptions thrown when there is an issue with object injection
    /// </summary>
    public class InjectionException : Exception
    {
        #region Constructors and Destructors

        public InjectionException(string message) : base(message)
        {
        }

        public InjectionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion
    }
}