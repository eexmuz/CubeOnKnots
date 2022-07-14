using TMPro;
using UnityEngine;

public class CustomSlider : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _text;

    public void OnValueChanged(float value)
    {
        _text.text = value.ToString("0.00");
    }
}