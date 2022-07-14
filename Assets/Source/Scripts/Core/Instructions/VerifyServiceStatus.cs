using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Instructions
{
    /// <summary>
    ///     Blocks a coroutine until the status of all services is reached.
    /// </summary>
    public class VerifyServiceStatus : CustomYieldInstruction
    {
        #region Fields

        /// <summary>
        ///     List of services to monitor
        /// </summary>
        private readonly IEnumerable<IService> _services;

        /// <summary>
        ///     The status to wait for
        /// </summary>
        private readonly ServiceStatus _status;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public VerifyServiceStatus(ServiceStatus status, IEnumerable<IService> services)
        {
            _status = status;
            _services = services;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Returns true while the coroutine should block
        /// </summary>
        public override bool keepWaiting
        {
            get { return _services.Any(service => service.GetStatus() != _status); }
        }

        #endregion
    }
}