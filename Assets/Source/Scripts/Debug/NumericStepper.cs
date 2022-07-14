using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NumericStepper : MonoBehaviour
{
    #region Fields

    public Button DownNumberButton;
    public int MaxValue = 100;
    public int MinValue;
    public UnityEvent OnValueChanged = new UnityEvent();
    public int Step = 1;
    public string Suffix = "";
    public Button UpNumberButton;
    public int Value;
    public Text ValueText;

    #endregion

    #region Methods

    private void OnDownNumberButtonClick()
    {
        if (Value == MinValue)
            return;

        Value -= Step;

        if (Value < MinValue)
            Value = MinValue;

        OnUpdate();
    }

    private void OnUpdate()
    {
        ValueText.text = Value + Suffix;

        if (OnValueChanged != null)
            OnValueChanged.Invoke();
    }

    private void OnUpNumberButtonClick()
    {
        if (Value == MaxValue)
            return;

        Value += Step;

        if (Value > MaxValue)
            Value = MaxValue;

        OnUpdate();
    }

    [UsedImplicitly]
    private void Start()
    {
        OnUpdate();

        UpNumberButton.onClick.AddListener(OnUpNumberButtonClick);
        DownNumberButton.onClick.AddListener(OnDownNumberButtonClick);
    }

    #endregion
}