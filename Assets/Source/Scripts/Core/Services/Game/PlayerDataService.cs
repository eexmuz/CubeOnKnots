using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using Core.Settings;
using Core.Attributes;
using Data;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace Core.Services
{
    [InjectionAlias(typeof(IPlayerDataService))]
    public class PlayerDataService : Service, IPlayerDataService
    {
        [Serializable]
        public class JsonArrayWrapper
        {
            public List<LevelStatus> Items;
        }

        #region Fields

        [Inject]
        private IPlayerPrefsService _playerPrefsService;

        [Inject]
        private GameSettings _gameSettings;
        
        private List<LevelStatus> _levelStatusList;

        #endregion

        #region Public Properties

        public int SessionTime { get; set; }
        public int PlayTime { get; set; }
        
        public bool NoAds 
        {
            get => _playerPrefsService.GetBool(PlayerPrefsKeys.NO_ADS);
            set => _playerPrefsService.SetBool(PlayerPrefsKeys.NO_ADS, value);
        }

        public bool IsFirstLaunch
        {
            get => !_playerPrefsService.GetBool(PlayerPrefsKeys.IS_FIRST_LAUNCH);
            set => _playerPrefsService.SetBool(PlayerPrefsKeys.IS_FIRST_LAUNCH, !value);
        }

        public int FirstLaunchTimeStamp
        {
            get => _playerPrefsService.GetInt(PlayerPrefsKeys.FIRST_LAUNCH_TIMESTAMP);
            set => _playerPrefsService.SetInt(PlayerPrefsKeys.FIRST_LAUNCH_TIMESTAMP, value);
        }

        public int ShowRateUsTimeStamp
        {
            get => _playerPrefsService.GetInt(PlayerPrefsKeys.SHOW_RATE_US_TIMESTAMP);
            set => _playerPrefsService.SetInt(PlayerPrefsKeys.SHOW_RATE_US_TIMESTAMP, value);
        }

        public bool IsNeedToShowRateUsDialog
        {
            get => _playerPrefsService.GetBool(PlayerPrefsKeys.IS_SHOW_RATE_DIALOG);
            set => _playerPrefsService.SetBool(PlayerPrefsKeys.IS_SHOW_RATE_DIALOG, value);
        }

        public bool IsGameRated
        {
            get => _playerPrefsService.GetBool(PlayerPrefsKeys.IS_RATE_GAME);
            set => _playerPrefsService.SetBool(PlayerPrefsKeys.IS_RATE_GAME, value);
        }

        public bool IsGdprApplied
        {
            get => _playerPrefsService.GetBool(PlayerPrefsKeys.IS_GDPR_APPLIED);
            set => _playerPrefsService.SetBool(PlayerPrefsKeys.IS_GDPR_APPLIED, value);
        }
        
        public int SessionCount
        {
            get => _playerPrefsService.GetInt(PlayerPrefsKeys.SESSIONS_COUNT);
            set => _playerPrefsService.SetInt(PlayerPrefsKeys.SESSIONS_COUNT, value);
        }
        
        public int LevelsCount
        {
            get => _playerPrefsService.GetInt(PlayerPrefsKeys.LEVELS_COUNT);
            set => _playerPrefsService.SetInt(PlayerPrefsKeys.LEVELS_COUNT, value);
        }

        public int LastLevel
        {
            get => _playerPrefsService.GetInt(PlayerPrefsKeys.LAST_LEVEL);
            set => _playerPrefsService.SetInt(PlayerPrefsKeys.LAST_LEVEL, value);
        }

        public LevelData LevelData
        {
            get => LoadLevelData();
            set => SaveLevelData(value);
        }

        public int HighestOpenedLevel
        {
            get
            {
                int highestOpenedLevel = 0;
                for (int i = 0; i < LevelStatusList.Count; i++)
                {
                    if (LevelStatusList[i].Unlocked)
                    {
                        highestOpenedLevel = i;
                    }
                }

                return highestOpenedLevel;
            }
        }

        public List<LevelStatus> LevelStatusList => _levelStatusList;

        #endregion

        #region Public Methods and Operators

        public override void Run()
        {
            LoadPlayerData();

            // for (int i = 13; i < _levelStatusList.Count; i++)
            // {
            //     _levelStatusList[i].Unlocked = false;
            //     _levelStatusList[i].Complete = false;
            //     _levelStatusList[i].Stars = 0;
            // }
            //
            // SaveLevelsProgress();
            
            base.Run();
        }

        public bool CanShowRateUs()
        {
            return IsGameRated == false &&
                   TimeUtils.GetTimestamp() - ShowRateUsTimeStamp > 24 * 60 * 60 &&
                   LevelStatusList.Count(l => l.Complete) >= 3;
        }

        public bool IsLevelComplete(int level)
        {
            return _levelStatusList[level].Complete;
        }

        public void CompleteLevel(int levelIndex, int stars)
        {
            if (levelIndex < 0 || levelIndex > _levelStatusList.Count - 1)
            {
                return;
            }
            
            _levelStatusList[levelIndex].Complete = true;
            
            if (stars < _levelStatusList[levelIndex].Stars)
            {
                return;
            }
            
            _levelStatusList[levelIndex].Stars = stars;
            _levelStatusList[Mathf.Min(LastLevel + 1, _levelStatusList.Count - 1)].Unlocked = true;

            SaveLevelsProgress();
        }

        #endregion
        
        private void LoadPlayerData()
        {
            LoadLevelsProgress();
        }

        private LevelData LoadLevelData()
        {
            string json = _playerPrefsService.GetString(PlayerPrefsKeys.LEVEL_DATA);
            return string.IsNullOrEmpty(json) ? null : JsonUtility.FromJson<LevelData>(json);
        }

        private void SaveLevelData(LevelData levelData)
        {
            string json = JsonUtility.ToJson(levelData);
            _playerPrefsService.SetString(PlayerPrefsKeys.LEVEL_DATA, json);
        }

        private void LoadLevelsProgress()
        {
            string json = _playerPrefsService.GetString(PlayerPrefsKeys.LEVELS_PROGRESS);
            JsonArrayWrapper wrapper = JsonUtility.FromJson<JsonArrayWrapper>(json);
            if (wrapper != null)
            {
                _levelStatusList = wrapper.Items;
                if (_levelStatusList.Count < _gameSettings.Levels.Count)
                {
                    for (int i = _levelStatusList.Count; i < _gameSettings.Levels.Count; i++)
                    {
                        _levelStatusList.Add(new LevelStatus
                        {
                            Stars = _gameSettings.Levels[i].DefaultLevelStatus.Stars,
                            Unlocked = _gameSettings.Levels[i].DefaultLevelStatus.Unlocked,
                        });
                    }
                }
            }
            else
            {
                _levelStatusList = new List<LevelStatus>(_gameSettings.Levels.Count);
                
                for (int i = 0; i < _gameSettings.Levels.Count; i++)
                {
                    _levelStatusList.Add(new LevelStatus
                    {
                        Stars = _gameSettings.Levels[i].DefaultLevelStatus.Stars,
                        Unlocked = _gameSettings.Levels[i].DefaultLevelStatus.Unlocked,
                    });
                }
            }
        }

        private void SaveLevelsProgress()
        {
            string json = JsonUtility.ToJson(new JsonArrayWrapper{ Items = _levelStatusList });
            _playerPrefsService.SetString(PlayerPrefsKeys.LEVELS_PROGRESS, json);
        }
    }
}