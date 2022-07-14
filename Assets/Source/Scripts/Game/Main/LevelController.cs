using System.Collections;
using System.Collections.ObjectModel;
using Core;
using Core.Attributes;
using Core.Notifications;
using Core.Services;
using Core.Settings;
using UnityEngine;

public enum Direction
{
    Up,
    Right,
    Down,
    Left
}

public class LevelController : DIBehaviour
{
    [SerializeField]
    private float _victoryDelay = .5f;
    
    [SerializeField]
    private CameraController _camera;
    
    [SerializeField]
    private Board _board;

    [SerializeField]
    private ParticleSystem _victoryVFX;

    [Inject]
    private GameSettings _gameSettings;

    [Inject]
    private IPlayerDataService _playerDataService;

    [Inject]
    private IGameService _gameService;

    [Inject]
    private IDelayedCallService _delayedCallService;

    private int _movesCounter;
    private bool _playing;
    private bool _logoFaded;

    public ObjectPool<Block> BlocksPool { get; private set; }

    public void OnSwipeN() => OnSwipe(Direction.Up);
    public void OnSwipeE() => OnSwipe(Direction.Right);
    public void OnSwipeS() => OnSwipe(Direction.Down);
    public void OnSwipeW() => OnSwipe(Direction.Left);

    protected override void OnAppInitialized()
    {
        Subscribe(NotificationType.LoadNewLevel, OnLoadNewLevel);
        Subscribe(NotificationType.LoadSavedLevel, OnLoadSavedLevel);
        Subscribe(NotificationType.BlocksMerge, OnBlocksMerge);
        Subscribe(NotificationType.BoardSetupComplete, OnBoardSetupComplete);
        Subscribe(NotificationType.LoadingLogoFaded, OnLoadingLogoFaded);
        BlocksPool = new ObjectPool<Block>(_gameSettings.BlockPrefab, 36, transform);
    }

    private void OnBoardSetupComplete(NotificationType notificationType, NotificationParams notificationParams)
    {
        _playing = true;
    }
    
    private void OnLoadingLogoFaded(NotificationType notificationType, NotificationParams notificationParams)
    {
        _logoFaded = true;
    }

    private LevelData GetLevelData()
    {
        return new LevelData
        {
            BlocksData = _board.GetBlocksData(),
            MovesCount = _movesCounter,
            LevelIndex = _gameService.CurrentLevel.LevelIndex,
        };
    }

    private void OnBlocksMerge(NotificationType notificationType, NotificationParams notificationParams)
    {
        int mergedPOT = (int) notificationParams.Data;
        if (mergedPOT >= _gameService.CurrentLevel.TargetValue.POT)
        {
            StartCoroutine(OnTargetBlockMerged_co());
        }
    }

    private IEnumerator OnTargetBlockMerged_co()
    {
        yield return new WaitForEndOfFrame();
        
        int stars = _gameService.CurrentLevel.CalculateStars(_movesCounter);
        _playing = false;
        
        _victoryVFX.Play();
        
        Dispatch(NotificationType.PlayerReachedTargetNumber);
        _playerDataService.CompleteLevel(_gameService.CurrentLevel.LevelIndex, stars);

        _delayedCallService.DelayedCall(_victoryDelay, () =>
        {
            Dispatch(NotificationType.BlurGame, NotificationParams.Get(true));
            Dispatch(NotificationType.ShowView,
                ShowViewNotificationParams.Get(ViewName.VictoryDialog, ViewCreationOptions.None,
                    (_movesCounter, stars, _gameService.CurrentLevel.StarMoves.z - 1)));
        });
    }

    private void OnLoadNewLevel(NotificationType notificationType, NotificationParams notificationParams)
    {
        int levelIndex = Mathf.Min((int) notificationParams.Data, _gameSettings.Levels.Count - 1);
        LevelConfig level = _gameSettings.Levels[levelIndex];
        level.LevelIndex = levelIndex;
        _playerDataService.LastLevel = levelIndex;
        
        LoadLevel(level);
    }
    
    private void OnLoadSavedLevel(NotificationType notificationType, NotificationParams notificationParams)
    {
        LevelData savedData = (LevelData) notificationParams.Data;
        LevelConfig level = _gameSettings.Levels[savedData.LevelIndex];
        level.LevelIndex = savedData.LevelIndex;
        _playerDataService.LastLevel = savedData.LevelIndex;
        
        LoadLevel(level, savedData);
    }

    private void LoadLevel(LevelConfig levelConfig, LevelData savedData = null)
    {
        _playing = false;

        _victoryVFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _camera.SetupCamera(levelConfig);

        StartCoroutine(SetupBoard(levelConfig, savedData));

        _movesCounter = savedData?.MovesCount ?? 0;

        Dispatch(NotificationType.LevelLoaded, NotificationParams.Get(levelConfig));
        Dispatch(NotificationType.MovesCounterChanged, NotificationParams.Get(_movesCounter));
    }

    private IEnumerator SetupBoard(LevelConfig levelConfig, LevelData savedData)
    {
        while (_logoFaded == false)
        {
            yield return null;
        }
        
        _board.SetupBoard(levelConfig, savedData);
    }

    private void OnSwipe(Direction direction)
    {
        if (_playing == false)
        {
            return;
        }
        
        bool anyChanges = _board.Swipe(direction);
        if (anyChanges)
        {
            _movesCounter++;
            Dispatch(NotificationType.MovesCounterChanged, NotificationParams.Get(_movesCounter));
        }
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused == false)
        {
            return;
        }

        Debug.Log("Saving level data . . .");
        _playerDataService.LevelData = _playing == false ? null : GetLevelData();
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Saving level data . . .");
        _playerDataService.LevelData = _playing == false ? null : GetLevelData();
    }
}