using System;
using System.Collections.Generic;
using Aig.Client.Integration.Runtime.Analytics;
using Aig.Client.Integration.Runtime.Subsystem;
using Core;
using Core.Attributes;
using Core.Notifications;
using Core.Services;
using Core.Settings;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class PauseDialog : BaseViewController
{
    [Inject]
    private IPlayerDataService _playerDataService;

    [Inject]
    private IGameOptionsService _gameOptionsService;

    [Inject]
    private ISocialService _socialService;

    [Inject]
    private GameSettings _gameSettings;
    
    [Inject]
    private IInAppPurchaseService _inAppPurchaseService;
    
    [SerializeField]
    private CustomToggle soundToggle;

    [SerializeField]
    private CustomToggle musicToggle;

    [SerializeField]
    private CustomToggle vibrationToggle;

    [SerializeField]
    private GameObject _noAdsButton;

    
    private bool _isInited;

    private Action<bool> _confirmRestartHandler;

    protected override void OnAppInitialized()
    {
        Subscribe(NotificationType.PurchaseNoAdsComplete, (a, b) => _noAdsButton.SetActive(_playerDataService.NoAds == false));
    }

    public override void InitWithData(object data)
    {
        base.InitWithData(data);
        
        soundToggle.SetToggle(_gameOptionsService.Sound);
        musicToggle.SetToggle(_gameOptionsService.Music);
        vibrationToggle.SetToggle(_gameOptionsService.Vibration);

        _noAdsButton.SetActive(_playerDataService.NoAds == false);

        _confirmRestartHandler = ConfirmRestartHandler;
        _isInited = true;
    }
    
    public void OnBackButtonClick()
    {
        CloseDialog();
    }

    public void OnSoundToggleChanged(bool value)
    {
        if(_isInited == false)
            return;

        IntegrationSubsystem.Instance.AnalyticsService.CustomEvent("pause_button", new Dictionary<string, object>()
        {
            {"action_name", value ? "sound_on" : "sound_off"}
        }, false, AnalyticType.AppMetrica);

        _gameOptionsService.Sound = value;
    }

    public void OnMusicToggleChanged(bool value)
    {
        if(_isInited == false)
            return;

        IntegrationSubsystem.Instance.AnalyticsService.CustomEvent("pause_button", new Dictionary<string, object>()
        {
            {"action_name", value ? "music_on" : "music_off"}
        }, false, AnalyticType.AppMetrica);

        _gameOptionsService.Music = value;
    }

    public void OnVibrationToggleChanged(bool value)
    {
        if(_isInited == false)
            return;

        IntegrationSubsystem.Instance.AnalyticsService.CustomEvent("pause_button", new Dictionary<string, object>()
        {
            {"action_name", value ? "vibration_on" : "vibration_off"}
        }, false, AnalyticType.AppMetrica);

        _gameOptionsService.Vibration = value;
    }

    public void OnRestartButtonClick()
    {
        Dispatch(NotificationType.ShowView, ShowViewNotificationParams.Get(ViewName.ConfirmDialog, ViewCreationOptions.None, _confirmRestartHandler));
    }

    private void ConfirmRestartHandler(bool result)
    {
        if (result == false)
        {
            return;
        }
        
        IntegrationSubsystem.Instance.AnalyticsService.CustomEvent("pause_button", new Dictionary<string, object>()
        {
            {"action_name", "restart_level"}
        }, false, AnalyticType.AppMetrica);
        
        CloseDialog();
        Dispatch(NotificationType.RestartLevel);
    }

    public void OnLeaderboardButtonClick()
    {
        IntegrationSubsystem.Instance.AnalyticsService.CustomEvent("pause_button", new Dictionary<string, object>()
        {
            {"action_name", "leaders_board"}
        }, false, AnalyticType.AppMetrica);
        
        _socialService.ShowLeaderBoard();
    }

    public void OnNoAdsButtonClick()
    {
        _inAppPurchaseService.BuyProduct(InAppPurchaseService.PRODUCT_NO_ADS);
    }

    public void OnRestorePurchasesButtonClick()
    {
        IntegrationSubsystem.Instance.AnalyticsService.CustomEvent("pause_button", new Dictionary<string, object>()
        {
            {"action_name", "restore_purchase"}
        }, false, AnalyticType.AppMetrica);
    }

    public void OnPolicyButtonClick()
    {
        IntegrationSubsystem.Instance.AnalyticsService.CustomEvent("pause_button", new Dictionary<string, object>()
        {
            {"action_name", "privacy_policy"}
        }, false, AnalyticType.AppMetrica);

        Application.OpenURL("https://devgame.me/policy");
    }

    public void OnMediationDebuggerClick()
    {
        MaxSdk.ShowMediationDebugger();
    }
}
