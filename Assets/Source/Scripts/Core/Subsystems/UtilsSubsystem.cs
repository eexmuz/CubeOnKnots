using System.Collections.Generic;
using Core.Services;

namespace Core.Subsystems
{
    /// <summary>
    ///     Responsible for displaying and controlling all user interface elements.
    ///     Will also handle management of game dialogs and user interface hooks.
    /// </summary>
    public class UtilsSubsystem : Subsystem
    {
        #region Fields

        private IDelayedCallService _delayedCallService;

        #endregion

        #region Methods

        /// <summary>
        ///     Used to initialize any variables or game state before the game starts.
        ///     Only once during the lifetime of the script instance.
        /// </summary>
        private void Awake()
        {
            _delayedCallService = gameObject.AddComponent<DelayedCallService>();

            Services = new List<IService>
            {
                _delayedCallService
            };
        }

        #endregion
    }
}