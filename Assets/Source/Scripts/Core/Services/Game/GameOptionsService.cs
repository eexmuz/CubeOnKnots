using Core.Settings;
using Core.Attributes;
using Data;
using UnityEngine;

namespace Core.Services
{
    public class GameOptions
    {
        #region Fields

        public bool Sound = true;
        public bool Music = true;
        public bool Vibration = true;

        #endregion
    }

    [InjectionAlias(typeof(IGameOptionsService))]
    public class GameOptionsService : Service, IGameOptionsService
    {
        #region Fields

        [Inject]
        private GameAudioSettings _gameAudioSettings;

        [Inject]
        private IPlayerPrefsService _playerPrefsService;

        private GameOptions _gameOptions;

        #endregion

        #region Public Properties

        public bool Sound
        {
            get => GameOptionsObject.Sound;
            set
            {
                GameOptionsObject.Sound = value;
                Dispatch(NotificationType.ToggleSoundState, NotificationParams.Get(value));
                SaveSettings();
            }
        }

        public bool Music
        {
            get => GameOptionsObject.Music;
            set
            {
                GameOptionsObject.Music = value;
                Dispatch(NotificationType.ToggleMusicState, NotificationParams.Get(value));
                SaveSettings();
            }
        }

        public bool Vibration
        {
            get => GameOptionsObject.Vibration;

            set
            {
                GameOptionsObject.Vibration = value;
                SaveSettings();
            }
        }

        #endregion

        #region Properties

        private GameOptions GameOptionsObject
        {
            get
            {
                if (_gameOptions == null)
                {
                    _gameOptions = new GameOptions();
                }

                return _gameOptions;
            }

            set => _gameOptions = value;
        }

        #endregion

        #region Public Methods and Operators

        public override void Run()
        {
            base.Run();

            LoadSettings();

            Sound = _gameOptions.Sound;
            Music = _gameOptions.Music;
            Vibration = _gameOptions.Vibration;
        }

        private void SaveSettings()
        {
            var jsonString = JsonUtility.ToJson(GameOptionsObject);
            _playerPrefsService.SetString(PlayerPrefsKeys.GAME_OPTIONS, jsonString);
            _playerPrefsService.Save();
        }

        #endregion

        #region Methods

        private float LinearToDecibel(float linear)
        {
            float dB;

            if (linear > 0)
                dB = 20.0f * Mathf.Log10(linear);
            else
                dB = -144.0f;

            return dB;
        }

        private void LoadSettings()
        {
            if (_playerPrefsService.HasKey(PlayerPrefsKeys.GAME_OPTIONS))
            {
                GameOptionsObject =
                    JsonUtility.FromJson<GameOptions>(_playerPrefsService.GetString(PlayerPrefsKeys.GAME_OPTIONS));
            }
            else
            {
                GameOptionsObject = new GameOptions();
                SaveSettings();
            }
        }

        #endregion
    }
}