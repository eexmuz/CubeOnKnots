using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconWithText : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _text;

    [SerializeField]
    private Image _icon;

    [SerializeField]
    private Vector2 _iconOffset;

    public void SetText(string text)
    {
        if (_text.text == text)
        {
            return;
        }
        
        _text.text = text;
        _text.ForceMeshUpdate();
        _icon.rectTransform.anchoredPosition = new Vector2(_text.bounds.min.x + _iconOffset.x,
            _text.rectTransform.anchoredPosition.y + _iconOffset.y);
    }
}
