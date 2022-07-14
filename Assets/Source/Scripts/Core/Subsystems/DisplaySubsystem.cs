using System.Collections.Generic;
using Core.Services;

namespace Core.Subsystems
{
    /// <summary>
    ///     Responsible for displaying and controlling all user interface elements.
    ///     Will also handle management of game dialogs and user interface hooks.
    /// </summary>
    public class DisplaySubsystem : Subsystem
    {
        #region Fields

        private IDialogsService _dialogsService;
        private ILevelLoaderService _levelLoaderService;
        private ILocaleService _localeService;

        #endregion

        #region Methods

        /// <summary>
        ///     Used to initialize any variables or game state before the game starts.
        ///     Only once during the lifetime of the script instance.
        /// </summary>
        private void Awake()
        {
            _levelLoaderService = gameObject.AddComponent<LevelLoaderService>();
            _dialogsService = gameObject.AddComponent<DialogsService>();
            _localeService = gameObject.AddComponent<LocaleService>();

            Services = new List<IService>
            {
                _levelLoaderService,
                _dialogsService,
                _localeService
            };
        }

        #endregion
    }
}