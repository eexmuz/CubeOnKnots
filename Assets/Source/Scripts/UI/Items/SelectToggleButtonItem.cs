using System;
using UnityEngine;

public class SelectToggleButtonItem : MonoBehaviour
{
    [SerializeField]
    private GameObject enabledState;

    [SerializeField]
    private GameObject disabledState;

    public Action OnSelect;

    public void OnToggleButtonClick()
    {
        OnSelect?.Invoke();
    }

    public void SetEnabled()
    {
        enabledState.SetActive(true);
        disabledState.SetActive(false);
    }

    public void SetDisabled()
    {
        enabledState.SetActive(false);
        disabledState.SetActive(true);
    }
}
