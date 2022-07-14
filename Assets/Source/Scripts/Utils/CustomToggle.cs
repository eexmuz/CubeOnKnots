using Core;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class CustomToggle : DIBehaviour
{
    [SerializeField]
    private GameObject _on;
    
    [SerializeField]
    private GameObject _off;
    
    private Toggle _toggle;

    protected override void OnAppInitialized()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(SetToggle);
    }

    public void SetToggle(bool state)
    {
        _on.SetActive(state);
        _off.SetActive(!state);
    }

    public void SetToggleWithoutNotification(bool state)
    {
        SetToggle(state);
        _toggle ??= GetComponent<Toggle>();
        _toggle.SetIsOnWithoutNotify(state);
    }

    public void AddListener(System.Action action)
    {
        _toggle.onValueChanged.AddListener(state => action.Invoke());
    }
}