using System;
using System.Collections;
using Aig.Client.Integration.Runtime.Subsystem;
using Core;
using Core.Attributes;
using Core.Services;
using Facebook.Unity;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class RateUsDialog : BaseViewController
{
    //[Inject]
    //private IAnalyticsService _analyticsService;

    [Inject]
    private IPlayerDataService _playerDataService;

    [SerializeField]
    private GameObject[] stars;

    [SerializeField]
    private GameObject buttonRate;

    [SerializeField]
    private GameObject buttonRateDisabled;

    private int _currentRate = 4;
    private string _reason = "new_player";

    public override void InitWithData(object data)
    {
        base.InitWithData(data);

        _playerDataService.IsNeedToShowRateUsDialog = false;
        _playerDataService.ShowRateUsTimeStamp = TimeUtils.GetTimestamp();
        _currentRate = -1;

        OnStarClick(_currentRate);
    }

    public void OnStarClick(int starIndex)
    {
        buttonRate.SetActive(starIndex >= 0);
        buttonRateDisabled.SetActive(starIndex < 0);

        _currentRate = starIndex;
        for (var i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(i <= starIndex);
        }
    }

    public void OnRateUsClick()
    {
        IntegrationSubsystem.Instance.AnalyticsService.RateUs(_reason, _currentRate + 1);

        _playerDataService.IsGameRated = true;

        StartCoroutine(RedirectWithDelay());

        CloseDialog();
    }

    private IEnumerator RedirectWithDelay()
    {
        yield return null;
#if !UNITY_IOS
        if (_currentRate >= 4)
        {
#endif
            RedirectToStore();
#if !UNITY_IOS
        }
#endif
    }

    public void OnExitClick()
    {
        IntegrationSubsystem.Instance.AnalyticsService.RateUs(_reason, 0);

        Dispatch(NotificationType.BlurGame, NotificationParams.Get(false));
        CloseDialog();
    }

    public override void OnShroudClicked()
    {
        IntegrationSubsystem.Instance.AnalyticsService.RateUs(_reason, 0);

        base.OnShroudClicked();
    }

    protected override void onClosedInternal()
    {
        Dispatch(NotificationType.RateUsDialogClosed);
        base.onClosedInternal();
    }

    private void RedirectToStore()
    {
#if UNITY_EDITOR
        Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
#elif UNITY_ANDROID
        Application.OpenURL("market://details?id=" + Application.identifier);
#elif UNITY_IPHONE
        Application.OpenURL("itms-apps://itunes.apple.com/app/id1558991437");
#endif
    }
}
