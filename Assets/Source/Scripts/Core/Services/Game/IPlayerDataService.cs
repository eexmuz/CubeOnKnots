using System.Collections.Generic;

namespace Core.Services
{
    public interface IPlayerDataService : IService
    {
        #region Public Properties

        bool NoAds { get; set; }
        bool IsNeedToShowRateUsDialog { get; set; }
        bool IsGameRated { get; set; }
        bool IsFirstLaunch { get; set; }
        int FirstLaunchTimeStamp { get; set; }
        int ShowRateUsTimeStamp { get; set; }
        bool IsGdprApplied { get; set; }
        int SessionCount { get; set; }
        int LevelsCount { get; set; }
        
        int LastLevel { get; set; }
        int HighestOpenedLevel { get; }
        List<LevelStatus> LevelStatusList { get; }
        LevelData LevelData { get; set; }
        

        /// <summary>
        /// Resets after closing game.
        /// </summary>
        int SessionTime { get; set; }
        
        /// <summary>
        /// Resets after closing game. Pauses when popup appears.
        /// </summary>
        int PlayTime { get; set; }
        
        #endregion

        #region Public Methods and Operators
        
        bool CanShowRateUs();

        bool IsLevelComplete(int level);
        void CompleteLevel(int levelIndex, int stars);

        #endregion
    }
}