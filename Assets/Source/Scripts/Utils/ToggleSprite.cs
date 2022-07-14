using UnityEngine;

public class ToggleSprite : MonoBehaviour
{
    [SerializeField]
    private GameObject _off;

    [SerializeField]
    private GameObject _on;

    public void OnValueChanged(bool state)
    {
        _off.SetActive(!state);
        _on.SetActive(state);
    }
}