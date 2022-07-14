using System.Collections.Generic;
using Core.Services;
using JetBrains.Annotations;

namespace Core.Subsystems
{
    /// <summary>
    ///     Responsible for displaying and controlling all user interface elements.
    ///     Will also handle management of game dialogs and user interface hooks.
    /// </summary>
    public class AudioSubsystem : Subsystem
    {
        #region Fields

        private IMusicService _musicService;

        private ISoundService _soundService;

        #endregion

        #region Methods

        /// <summary>
        ///     Used to initialize any variables or game state before the game starts.
        ///     Only once during the lifetime of the script instance.
        /// </summary>
        [UsedImplicitly]
        private void Awake()
        {
            _soundService = gameObject.AddComponent<SoundService>();
            _musicService = gameObject.AddComponent<MusicService>();

            Services = new List<IService>
            {
                _soundService,
                _musicService
            };
        }

        #endregion
    }
}