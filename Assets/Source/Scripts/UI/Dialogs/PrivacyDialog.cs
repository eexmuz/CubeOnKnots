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

public class PrivacyDialog : BaseViewController
{
    [Inject]
    private IPlayerDataService _playerDataService;

    public override void InitWithData(object data)
    {
        base.InitWithData(data);
    }

    public void OnTermsClick()
    {
        Application.OpenURL("https://devgame.me/policy");
    }

    public void OnPrivacyClick()
    {
        Application.OpenURL("https://devgame.me/policy");
    }

    public void OnAcceptClick()
    {
        _playerDataService.IsGdprApplied = true;
        Dispatch(NotificationType.PrivacyDialogClosed);
        CloseDialog();
    }
}
