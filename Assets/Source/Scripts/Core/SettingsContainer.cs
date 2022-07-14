using System.Collections.Generic;
using System.Linq;
using Core.Settings;
using UnityEngine;

namespace Core
{
    /// <summary>
    ///     Contains all the settings for the application and its subsystems.
    /// </summary>
    public class SettingsContainer : MonoBehaviour
    {
        #region Fields

        /// <summary>
        ///     List of all settings objects in the system
        /// </summary>
        [SerializeField]
        private List<ScriptableObject> allSettings;

        #endregion

        #region Public Properties

        /// <summary>
        ///     The current collection of all active settings
        /// </summary>
        public List<ISettings> ActiveSettings { get; set; }

        /// <summary>
        ///     Application settings object
        /// </summary>
        public ApplicationSettings AppSettings { get; private set; }

        public GameAudioSettings GameAudioSettings { get; private set; }

        /// <summary>
        ///     Communication settings object
        /// </summary>
        /// <summary>
        ///     Gets the lobby settings data.
        /// </summary>
        /// <summary>
        ///     Gets the game settings data
        /// </summary>

        public GameSettings GameSettings { get; private set; }

        public IapProductsSettings IapProductsSettings { get; private set; }

        /// <summary>
        ///     UI System Settings
        /// </summary>
        public UiSystemSettings UiSystemSettings { get; private set; }
        
        public SoundSettings SoundSettings { get; private set; }
        
        public LocalNotificationSettings LocalNotificationSettings { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Will check all settings objects to see if they are present.
        ///     If a settings object is NULL it will generate a default instance.
        /// </summary>
        public void FinalizeSettings()
        {
            if (allSettings == null)
            {
                CreateDefault();
                return;
            }

            var appSettings = (ApplicationSettings) allSettings.FirstOrDefault(o => o.GetType() == typeof(ApplicationSettings));
            var gameSettingsAsset = (GameSettings) allSettings.FirstOrDefault(o => o.GetType() == typeof(GameSettings));
            var uiSystemSettingsAsset = (UiSystemSettingsAsset) allSettings.FirstOrDefault(o => o.GetType() == typeof(UiSystemSettingsAsset));
            var iapProductsSettings = (IapProductsSettings) allSettings.FirstOrDefault(o => o.GetType() == typeof(IapProductsSettings));
            var audioSettings = (GameAudioSettings) allSettings.FirstOrDefault(o => o.GetType() == typeof(GameAudioSettings));
            var localNotificationSettings = (LocalNotificationSettings) allSettings.FirstOrDefault(o => o.GetType() == typeof(LocalNotificationSettings));
            var soundSettings = (SoundSettings) allSettings.FirstOrDefault(o => o.GetType() == typeof(SoundSettings));

            AppSettings = appSettings == null ? ScriptableObject.CreateInstance<ApplicationSettings>() : appSettings;
            GameSettings = gameSettingsAsset == null ? ScriptableObject.CreateInstance<GameSettings>() : gameSettingsAsset;
            UiSystemSettings = uiSystemSettingsAsset == null ? new UiSystemSettings() : uiSystemSettingsAsset.data;
            IapProductsSettings = iapProductsSettings == null ? ScriptableObject.CreateInstance<IapProductsSettings>() : iapProductsSettings;
            GameAudioSettings = audioSettings == null ? ScriptableObject.CreateInstance<GameAudioSettings>() : audioSettings;
            LocalNotificationSettings = localNotificationSettings == null ? ScriptableObject.CreateInstance<LocalNotificationSettings>() : localNotificationSettings;
            SoundSettings = soundSettings == null ? ScriptableObject.CreateInstance<SoundSettings>() : soundSettings;

            ActiveSettings = new List<ISettings>
            {
                AppSettings,
                GameSettings,
                UiSystemSettings,
                IapProductsSettings,
                GameAudioSettings,
                LocalNotificationSettings,
                SoundSettings,
            };
        }

        #endregion

        #region Methods

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        ///     create all default settings objects
        /// </summary>
        private void CreateDefault()
        {
            AppSettings = ScriptableObject.CreateInstance<ApplicationSettings>();
            GameSettings = ScriptableObject.CreateInstance<GameSettings>();
            UiSystemSettings = new UiSystemSettings();
            IapProductsSettings = ScriptableObject.CreateInstance<IapProductsSettings>();
            GameAudioSettings = ScriptableObject.CreateInstance<GameAudioSettings>();
            LocalNotificationSettings = ScriptableObject.CreateInstance<LocalNotificationSettings>();
            SoundSettings = ScriptableObject.CreateInstance<SoundSettings>();

            ActiveSettings = new List<ISettings>
            {
                AppSettings,
                GameSettings,
                UiSystemSettings,
                IapProductsSettings,
                GameAudioSettings,
                LocalNotificationSettings,
                SoundSettings,
            };
        }

        #endregion
    }
}