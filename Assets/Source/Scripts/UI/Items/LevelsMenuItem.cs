using Core;
using TMPro;
using UnityEngine;

public class LevelsMenuItem : DIBehaviour
{
    [SerializeField]
    private GameObject _lockImage;

    [SerializeField]
    private GameObject[] _stars;

    [SerializeField]
    private TMP_Text _levelNumberText;

    private int _levelNumber;
    private bool _unlocked;
    
    public void Init(int levelNumber, LevelStatus levelStatus)
    {
        _unlocked = levelStatus != null && levelStatus.Unlocked;
        
        _lockImage.SetActive(_unlocked == false);

        if (levelStatus != null && _unlocked)
        {
            for (int i = 0; i < _stars.Length; i++)
            {
                _stars[i].SetActive(i < levelStatus.Stars);
            }
        }

        _levelNumber = levelNumber;
        _levelNumberText.text = (_levelNumber + 1).ToString();
    }

    public void OnMenuItemPressed()
    {
        if (_unlocked == false)
        {
            return;
        }
        
        Dispatch(NotificationType.OpenLevelFromMenu, NotificationParams.Get(_levelNumber));
    }
}