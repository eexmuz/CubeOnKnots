using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Instructions
{
    /// <summary>
    ///     Blocks a coroutine until the status of all system components is reached.
    /// </summary>
    public class VerifySubsystemStatus : CustomYieldInstruction
    {
        #region Fields

        /// <summary>
        ///     The status to wait for
        /// </summary>
        private readonly SubsystemStatus _status;

        /// <summary>
        ///     List of subsystems to monitor
        /// </summary>
        private readonly IEnumerable<ISubsystem> _subsystems;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public VerifySubsystemStatus(SubsystemStatus status, IEnumerable<ISubsystem> subsystems)
        {
            _status = status;
            _subsystems = subsystems;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Returns true while the coroutine should block
        /// </summary>
        public override bool keepWaiting
        {
            get { return _subsystems.Any(subsystem => subsystem.GetStatus() != _status); }
        }

        #endregion
    }
}