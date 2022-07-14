using System.Collections;
using Core;
using DG.Tweening;
using UnityEngine;

public class SettingsDropdown : DIBehaviour
{
    [SerializeField]
    private RectTransform _dropdownPanel;

    [SerializeField]
    private float _droppedY;

    [SerializeField]
    private float _hiddenY;
    
    [SerializeField]
    private float _dropTime;
    
    private bool _dropped;
    private Sequence _sequence;

    private void Start()
    {
        SetState(false);
    }

    public void Toggle(bool instant = false)
    {
        float targetY = _dropped ? _hiddenY : _droppedY;
        float time = _dropTime * Mathf.Abs(targetY - _dropdownPanel.anchoredPosition.y) / Mathf.Abs(_droppedY - _hiddenY);
        _dropped = !_dropped;

        _sequence ??= DOTween.Sequence();
        _sequence.Kill();

        _sequence.Append(_dropdownPanel.DOAnchorPosY(targetY, time));
    }

    private void SetState(bool dropped)
    {
        _dropped = dropped;
        _sequence?.Kill();
        var pos = _dropdownPanel.anchoredPosition;
        pos.y = dropped ? _droppedY : _hiddenY;
        _dropdownPanel.anchoredPosition = pos;
    }
}