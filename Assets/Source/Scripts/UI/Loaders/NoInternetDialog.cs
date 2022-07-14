using System;
using System.Collections.Generic;
using Aig.Client.Integration.Runtime.Analytics;
using Aig.Client.Integration.Runtime.Subsystem;
using Data;
using UI;

public class NoInternetDialog : BaseViewController
{
    private Action<bool> _callback;
    
    public override void InitWithData(object data)
    {
        base.InitWithData(data);

        _callback = (Action<bool>) data;
    }

    public override void OnShroudClicked()
    {
        OnCloseButtonClick();
    }

    public void OnCloseButtonClick()
    {
        IntegrationSubsystem.Instance.AnalyticsService.CustomEvent("pop_up", new Dictionary<string, object>()
        {
            {"pop_up_id", ""},
            {"show_reason", "no_connection"},
            {"result", "reject"}
        }, false, AnalyticType.AppMetrica);
        CloseDialog();
        _callback.Invoke(false);
    }

    public void OnRetryButtonClick()
    {
        IntegrationSubsystem.Instance.AnalyticsService.CustomEvent("pop_up", new Dictionary<string, object>()
        {
            {"pop_up_id", ""},
            {"show_reason", "no_connection"},
            {"result", "accept"}
        }, false, AnalyticType.AppMetrica);
        CloseDialog();
        _callback.Invoke(true);
    }
}
