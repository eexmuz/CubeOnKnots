using System.ComponentModel;
using System.IO;
using Aig.Client.Integration.Runtime.Subsystem;
using Core;
using Core.Services;
using Core.Settings;
using UnityEngine;
using Utility;

public partial class SROptions
{
#if DISABLE_SRDEBUGGER == false
    #region Fields

    #endregion

    #region Constructors and Destructors

    public SROptions()
    {
        SRDebug.Instance.PanelVisibilityChanged += OnPanelVisibilityChanged;
        OnPanelVisibilityChanged(true);
    }

    #endregion

    #region Methods

    [Category("UI")]
    [Sort(1)]
    public void HideAllUI()
    {
        App.Instance.GameRoot.SetActive(false);
    }

    [Category("UI")]
    [Sort(2)]
    public void ShowAllUI()
    {
        App.Instance.GameRoot.SetActive(true);
    }

    [Category("Login")]
    [Sort(1)]
    public void AddLoginDay()
    {
        _playerDataService.FirstLaunchTimeStamp -= 86400;
    }

    [Category("Login")]
    [Sort(2)]
    public void AddLoginSession()
    {
        _playerDataService.SessionCount += 1;
    }

    [Category("Rate Us")]
    [Sort(1)]
    public void AddRateUsDay()
    {
        _playerDataService.ShowRateUsTimeStamp -= 86400;
    }

    [Category("Rate Us")]
    [Sort(2)]
    public void ResetRateUsShow()
    {
        _playerDataService.IsNeedToShowRateUsDialog = true;
    }

    [Category("Data")]
    [Sort(1)]
    public void ResetAllSavedData()
    {
        PlayerPrefs.DeleteAll();
    }

    [Category("Ads")]
    [Sort(1)]
    public void ShowMediationDebugger()
    {
        MaxSdk.ShowMediationDebugger();
    }

    [Category("Ads")]
    [Sort(2)]
    public void BuyNoAds()
    {
        _playerDataService.NoAds = true;
        IntegrationSubsystem.Instance.AdsService.HideBanner();
        App.Instance.Dispatch(NotificationType.PurchaseNoAdsComplete);
    }

    private PlayerDataService _playerDataService;

    private void OnPanelVisibilityChanged(bool isVisible)
    {
        if (!isVisible || App.Instance == null)
            return;

        _playerDataService = App.Instance.GetComponent<PlayerDataService>();
    }

    #endregion
#endif
}