using System.Collections.Generic;
using System.Linq;
using Aig.Client.Integration.Runtime.Analytics;
using Aig.Client.Integration.Runtime.Subsystem;
using Core;
using Core.Attributes;
using Core.Notifications;
using Core.Services;
using Core.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : DIBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasGroup;
    
    [SerializeField]
    private TMP_Text _targetBlockNumber;

    [SerializeField]
    private TMP_Text _levelNumber;

    [SerializeField]
    private TMP_Text _moves;

    [SerializeField]
    private GameObject[] _stars;

    [SerializeField]
    private SettingsDropdown _dropdown;
    
    [SerializeField]
    private CustomToggle _soundToggle;

    [SerializeField]
    private CustomToggle _vibrationToggle;

    [SerializeField]
    private GameObject _blur;

    [Inject]
    private GameSettings _gameSettings;

    [Inject]
    private IGameService _gameService;

    [Inject] 
    private ISoundService _soundService;

    [Inject]
    private IGameOptionsService _gameOptionsService;

    protected override void OnAppInitialized()
    {
        IntegrationSubsystem.Instance.AdsService.ShowBanner();
        
        Subscribe(NotificationType.LevelLoaded, OnLevelLoaded);
        Subscribe(NotificationType.MovesCounterChanged, OnPlayerMove);
        Subscribe(NotificationType.BlurGame, OnBlurGame);
        Subscribe(NotificationType.PlayerReachedTargetNumber, OnPlayerReachedTargetNumber);
        
        _soundToggle.SetToggleWithoutNotification(_gameOptionsService.Sound);
        _vibrationToggle.SetToggleWithoutNotification(_gameOptionsService.Vibration);
        _soundToggle.AddListener(() => _soundService.PlaySound(Sounds.Click));
        _vibrationToggle.AddListener(() => _soundService.PlaySound(Sounds.Click));

        foreach (var button in GetComponentsInChildren<Button>())
        {
            button.onClick.AddListener(() => _soundService.PlaySound(Sounds.Click));
        }
    }

    private void OnPlayerReachedTargetNumber(NotificationType notificationType, NotificationParams notificationParams)
    {
        _canvasGroup.blocksRaycasts = false;
    }

    private void OnBlurGame(NotificationType notificationType, NotificationParams notificationParams)
    {
        _blur.SetActive((bool) notificationParams.Data);
    }

    private void OnPlayerMove(NotificationType notificationType, NotificationParams notificationParams)
    {
        int moves = (int) notificationParams.Data;
        _moves.text = "MOVES: " + moves;
        for (int i = 0; i < _stars.Length; i++)
        {
            _stars[i].SetActive(moves < _gameService.CurrentLevel.StarMoves[i]);
        }
    }

    private void OnLevelLoaded(NotificationType notificationType, NotificationParams notificationParams)
    {
        LevelConfig loadedLevel = (LevelConfig) notificationParams.Data;
        _levelNumber.text = "LEVEL " + (loadedLevel.LevelIndex + 1);
        _targetBlockNumber.text = loadedLevel.TargetValue.Number.ToString();
        foreach (var star in _stars)
        {
            star.SetActive(true);
        }
        
        _canvasGroup.blocksRaycasts = true;
    }

    public void OnRestartButtonClick()
    {
        Dispatch(NotificationType.RestartLevel);
    }

    public void OnSettingsButtonClick()
    {
        _dropdown.Toggle();
    }
    
    public void OnSoundToggleChanged(bool value)
    {
        IntegrationSubsystem.Instance.AnalyticsService.CustomEvent("pause_button", new Dictionary<string, object>()
        {
            {"action_name", value ? "sound_on" : "sound_off"}
        }, false, AnalyticType.AppMetrica);

        _gameOptionsService.Sound = value;
    }

    public void OnVibrationToggleChanged(bool value)
    {
        IntegrationSubsystem.Instance.AnalyticsService.CustomEvent("pause_button", new Dictionary<string, object>()
        {
            {"action_name", value ? "vibration_on" : "vibration_off"}
        }, false, AnalyticType.AppMetrica);

        _gameOptionsService.Vibration = value;
    }
}
