using System;

namespace Core
{
    /// <summary>
    ///     Generic operation result
    /// </summary>
    public class OperationResult<T> : IOperationResult
    {
        #region Public Properties

        public Exception Error { get; set; }

        public bool Success => Error == null;

        #endregion

        #region Properties

        private T Data { get; set; }

        #endregion

        #region Public Methods and Operators

        public object GetData()
        {
            return Data;
        }

        public void Reset()
        {
            Data = default;
            Error = null;
        }

        public void SetData(object data)
        {
            Data = (T) data;
        }

        #endregion
    }

    /// <summary>
    ///     Operation result with no data
    /// </summary>
    public class OperationResult : IOperationResult
    {
        #region Public Properties

        public Exception Error { get; set; }

        public bool Success => Error == null;

        #endregion

        #region Public Methods and Operators

        public object GetData()
        {
            return null;
        }

        public void Reset()
        {
            Error = null;
        }

        public void SetData(object data)
        {
            // do nothing since this operaton result does not have data
        }

        #endregion
    }

    public interface IOperationResult
    {
        #region Public Properties

        Exception Error { get; set; }
        bool Success { get; }

        #endregion

        #region Public Methods and Operators

        object GetData();

        void Reset();

        void SetData(object data);

        #endregion
    }
}