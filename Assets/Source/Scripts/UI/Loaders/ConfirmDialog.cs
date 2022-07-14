using System;
using System.Collections.Generic;
using Aig.Client.Integration.Runtime.Analytics;
using Aig.Client.Integration.Runtime.Subsystem;
using UI;

public class ConfirmDialog : BaseViewController
{
    private Action<bool> _callback;
    
    public override void InitWithData(object data)
    {
        base.InitWithData(data);

        _callback = (Action<bool>) data;
    }

    public override void OnShroudClicked()
    {
        OnNoButtonClick();
    }

    public void OnNoButtonClick()
    {
        CloseDialog();
        // IntegrationSubsystem.Instance.AnalyticsService.CustomEvent("pop_up", new Dictionary<string, object>()
        // {
        //     {"pop_up_id", ""},
        //     {"show_reason", "restart_level"},
        //     {"result", "reject"}
        // }, false, AnalyticType.AppMetrica);
        _callback.Invoke(false);
    }

    public void OnYesButtonClick()
    {
        CloseDialog();
        // IntegrationSubsystem.Instance.AnalyticsService.CustomEvent("pop_up", new Dictionary<string, object>()
        // {
        //     {"pop_up_id", ""},
        //     {"show_reason", "restart_level"},
        //     {"result", "accept"}
        // }, false, AnalyticType.AppMetrica);
        _callback.Invoke(true);
    }
}
