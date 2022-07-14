using System.Collections.Generic;
using Core.Services;

namespace Core.Subsystems
{
    /// <summary>
    ///     Responsible for controlling, launching, and running all game operations
    /// </summary>
    public class GameSubsystem : Subsystem
    {
        #region Fields

        private IGameOptionsService _gameOptionsService;
        private IPlayerDataService _playerDataService;
        private ISocialService _socialService;
        private ITimeService _timeService;
        private IGameService _gameService;

        #endregion

        #region Methods

        /// <summary>
        ///     Used to initialize any variables or game state before the game starts.
        ///     Only once during the lifetime of the script instance.
        /// </summary>
        private void Awake()
        {
            _gameOptionsService = gameObject.AddComponent<GameOptionsService>();
            _playerDataService = gameObject.AddComponent<PlayerDataService>();
            _socialService = gameObject.AddComponent<SocialService>();
            _timeService = gameObject.AddComponent<TimeService>();
            _gameService = gameObject.AddComponent<GameService>();

            Services = new List<IService>
            {
                _gameOptionsService,
                _playerDataService,
                _socialService,
                _timeService,
                _gameService,
            };
        }

        #endregion
    }
}