using UnityEngine;

namespace Core.Settings
{
    /// <summary>
    ///     Contains all settings that are applicable globally
    /// </summary>
    [CreateAssetMenu(menuName = "Settings/Application Settings")]
    public class ApplicationSettings : ScriptableObject, ISettings
    {
        #region Fields

        public string androidBundle;

        public string androidLeaderBoardRatingName;
        public string appLink;

        public string appMetricApiKey;

        /// <summary>
        ///     The current application build version
        /// </summary>
        public string appVersion = "0.0.0";

        public string appVersionCode;
        public string appVersionPun;
        public string deployHash;

        public string deployHashPrefix;
        public string deployVersion;


        public string iosAppleId;

        /// <summary>
        ///     Production flag
        /// </summary>
        public bool production;

        public string supportEmail;

        #endregion
    }
}