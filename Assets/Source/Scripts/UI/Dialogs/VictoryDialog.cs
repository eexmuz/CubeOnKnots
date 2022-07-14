using System.Collections;
using Core;
using Core.Attributes;
using Core.Notifications;
using Core.Services;
using DG.Tweening;
using TMPro;
using UI;
using UnityEngine;

public class VictoryDialog : BaseViewController
{
    [SerializeField]
    private TextMeshProUGUI _levelCounter;

    [SerializeField]
    private GameObject[] _stars;

    [SerializeField]
    private TextMeshProUGUI _moves;

    [SerializeField]
    private TextMeshProUGUI _targetMoves;

    [Inject]
    private IPlayerDataService _playerDataService;

    [Inject]
    private IGameService _gameService;

    private bool _nextButtonClicked;

    public override void InitWithData(object data)
    {
        base.InitWithData(data);
        
        _nextButtonClicked = false;

        (int moves, int stars, int targetMoves) result = ((int, int, int)) data;

        _levelCounter.text = $"Уровень {(_gameService.CurrentLevel.LevelIndex + 1)} пройден";
        _moves.text = result.moves.ToString();
        _targetMoves.text = $"ЦЕЛЬ: {result.targetMoves}";
        for (int i = 0; i < _stars.Length; i++)
        {
            _stars[i].SetActive(i < result.stars);
        }
    }

    public override void OnShroudClicked() { }

    public void OnNextButtonClick()
    {
        if (_nextButtonClicked)
        {
            return;
        }

        _nextButtonClicked = true;
        CloseDialog();
        Dispatch(NotificationType.LoadNextLevel);
        if (_playerDataService.CanShowRateUs())
        {
            Dispatch(NotificationType.ShowView, ShowViewNotificationParams.Get(ViewName.RateUsDialog));
        }
        else
        {
            Dispatch(NotificationType.BlurGame, NotificationParams.Get(false));
        }
    }
}