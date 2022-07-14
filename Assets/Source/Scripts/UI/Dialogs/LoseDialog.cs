using System.Collections;
using Aig.Client.Integration.Runtime.Subsystem;
using Core;
using Core.Attributes;
using Core.Services;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class LoseDialog : BaseViewController
{
    [SerializeField]
    private float _timeToDecide;

    [SerializeField]
    private TMP_Text _timerText;

    [SerializeField]
    private Image _circleTimer;

    [SerializeField]
    private RectTransform _dynamicCorner;

    [SerializeField]
    private GameObject _staticCorner;

    [SerializeField]
    private GameObject _revivePanel;

    [SerializeField]
    private GameObject _restartPanel;

    [Inject]
    private IPlayerDataService _playerDataService;
    
    public override void InitWithData(object data)
    {
        base.InitWithData(data);

        bool revived = (bool) data;
        if (revived)
        {
            ShowPanel(false);
        }
        else
        {
            ShowPanel(true);
            StartCoroutine(Timer_co());
        }
    }

    public override void OnShroudClicked() { }

    public void OnReviveButtonClick()
    {
        IntegrationSubsystem.Instance.AdsService.ShowVideo(true, "revive", OnRewardReceived);
    }

    public void OnNoButtonClick()
    {
        CloseDialog();
        Dispatch(NotificationType.LevelFailed);
        Dispatch(NotificationType.LoadNewLevel);
    }

    private void OnRewardReceived()
    {
        CloseDialog();
        //Dispatch(NotificationType.OnRevive);
    }

    private void OnTimerEnd()
    {
        ShowPanel(false);
    }

    private void ShowPanel(bool revive)
    {
        _revivePanel.SetActive(revive);
        _restartPanel.SetActive(!revive);
    }

    private IEnumerator Timer_co()
    {
        _timerText.text = Mathf.RoundToInt(_timeToDecide).ToString();
        _circleTimer.fillAmount = 1f;
        _staticCorner.SetActive(true);
        _dynamicCorner.gameObject.SetActive(true);
        _dynamicCorner.localRotation = Quaternion.identity;
        
        float startTime = Time.unscaledTime;
        while (Time.unscaledTime < startTime + _timeToDecide)
        {
            float value = 1f - (Time.unscaledTime - startTime) / _timeToDecide;
            _circleTimer.fillAmount = value;
            _dynamicCorner.localRotation = Quaternion.Euler(Vector3.back * (1f - value) * 360f);
            _timerText.text = Mathf.CeilToInt(_timeToDecide * value).ToString();
            yield return null;
        }

        _timerText.text = "0";
        _circleTimer.fillAmount = 0f;
        _staticCorner.SetActive(false);
        _dynamicCorner.gameObject.SetActive(false);
        
        OnTimerEnd();
    }
}