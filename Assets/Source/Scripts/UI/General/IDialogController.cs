using System.Collections;
using UnityEngine;

public delegate void DialogEventHandler(IDialogController controller);

public interface IDialogController
{
    #region Public Events

    event DialogEventHandler onClosed;

    event DialogEventHandler onClosingBegin;

    #endregion

    #region Public Properties

    CanvasGroup CanvasGroup { get; set; }
    ViewName ViewName { get; set; }
    string ViewUID { get; set; }
    float InDuration { get; }
    bool IsAnimating { get; set; }
    bool IsCloseEnable { get; set; }
    float OutDuration { get; }
    bool DispatchPause { get; }

    #endregion

    #region Public Methods and Operators

    void Destroy();
    void GetDialogData();

    void InitWithData(object data);

    void OnShroudClicked();

    void CloseDialog(bool instant = false);

    IEnumerator Open(bool instant = false);
    void SetButtonListeners();
    void SetDialogData();
    void StartDisplay(bool instant);
    void StartHide(bool instant);

    #endregion
}