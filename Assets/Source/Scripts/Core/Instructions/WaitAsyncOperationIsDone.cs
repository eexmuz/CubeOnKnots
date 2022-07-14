using UnityEngine;

namespace Core.Instructions
{
    /// <summary>
    ///     Wait Async Operation IsDone Status
    /// </summary>
    public class WaitAsyncOperationIsDone : CustomYieldInstruction
    {
        #region Fields

        private readonly AsyncOperation _asyncOperation;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public WaitAsyncOperationIsDone(AsyncOperation asyncOperation)
        {
            _asyncOperation = asyncOperation;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Returns true while the coroutine should block
        /// </summary>
        public override bool keepWaiting => _asyncOperation.isDone == false;

        #endregion
    }
}