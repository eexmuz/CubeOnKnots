using Aig.Client.Integration.Runtime.Subsystem;
using Core;
using Core.Attributes;
using Core.Services;
using UnityEngine;
using Utility;

public class GameController : DIBehaviour
{
    [Inject]
    private IPlayerDataService _playerDataService;

    private bool _uiBlockedByAd;

    protected override void OnAppInitialized()
    {
        base.OnAppInitialized();

        IntegrationSubsystem.Instance.AnalyticsService.Technical("06_StartGameMainMenu",
            _playerDataService.IsFirstLaunch);
        
        _playerDataService.IsNeedToShowRateUsDialog = true;

        IntegrationSubsystem.Instance.AdsService.NoAdFunc = () => _playerDataService.NoAds;

        bool firstLaunch = _playerDataService.IsFirstLaunch;

        if (firstLaunch)
        {
            _playerDataService.IsFirstLaunch = false;
            _playerDataService.FirstLaunchTimeStamp = TimeUtils.GetTimestamp();
        }

        _playerDataService.SessionCount++;
        
        
        IntegrationSubsystem.Instance.AnalyticsService.Technical("07_EndGameMainMenu",
            firstLaunch);
        
        Subscribe(NotificationType.LoadingLogoFaded, OnLoadingLogoFaded);
        Subscribe(NotificationType.OpenLevelFromMenu, OnOpenLevelFromMenu);
        Subscribe(NotificationType.LoadNextLevel, OnLoadNextLevel);
        Subscribe(NotificationType.RestartLevel, OnRestartLevel);
        
        IntegrationSubsystem.Instance.AdsService.OnVideoAdsStarted += (s, s1, arg3) => BlockUIWhilePlayingAd();
        IntegrationSubsystem.Instance.AdsService.OnVideoAdsWatch += (s, s1, arg3, arg4) => UnlockUIAfterPlayingAd();
    }

    private void Start()
    {
        LevelData savedData = _playerDataService.LevelData;
        if (savedData == null)
        {
            Dispatch(NotificationType.LoadNewLevel, NotificationParams.Get(_playerDataService.HighestOpenedLevel));
        }
        else
        {
            Dispatch(NotificationType.LoadSavedLevel, NotificationParams.Get(savedData));
        }
    }

    private void OnLoadingLogoFaded(NotificationType notificationType, NotificationParams notificationParams)
    {
        Dispatch(NotificationType.UiBlockingOperationEnd);
    }
    
    private void OnOpenLevelFromMenu(NotificationType notificationType, NotificationParams notificationParams)
    {
        int level = notificationParams == null ? _playerDataService.LastLevel : (int) notificationParams.Data;
        LoadLevel(level);
    }
    
    private void OnLoadNextLevel(NotificationType notificationType, NotificationParams notificationParams)
    {
        LoadLevel(_playerDataService.LastLevel + 1);
    }
    
    private void OnRestartLevel(NotificationType notificationType, NotificationParams notificationParams)
    {
        LoadLevel(_playerDataService.LastLevel);
    }

    private void LoadLevel(int level)
    {
        Dispatch(NotificationType.LevelEnd);
        Dispatch(NotificationType.LoadNewLevel, NotificationParams.Get(level));
    }

    private void BlockUIWhilePlayingAd()
    {
        _uiBlockedByAd = true;
        Dispatch(NotificationType.UiBlockingOperationStart);
    }

    private void UnlockUIAfterPlayingAd()
    {
        if (_uiBlockedByAd)
        {
            _uiBlockedByAd = false;
            Dispatch(NotificationType.UiBlockingOperationEnd);
        }
    }
}