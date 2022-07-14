using System.Collections.Generic;
using Core;
using Core.Attributes;
using Core.Services;
using Core.Settings;
using Lean.Touch;
using UI;
using UnityEngine;

public class LevelsMenuDialog : BaseViewController
{
    [SerializeField]
    private LevelsMenuItem _menuItemPrefab;

    [SerializeField]
    private Transform _menuItemsParent;

    [SerializeField]
    private int _maxPageItems;

    [SerializeField]
    private GameObject _prevButton;

    [Inject]
    private IPlayerDataService _playerDataService;

    [Inject]
    private ISoundService _soundService;

    private int _currentPage;
    private List<LevelsMenuItem> _items;

    public override void InitWithData(object data)
    {
        base.InitWithData(data);

        var levels = _playerDataService.LevelStatusList;
        for (int i = 0; i < levels.Count; i++)
        {
            LevelsMenuItem item = Instantiate(_menuItemPrefab, _menuItemsParent);
            item.Init(i, i >= levels.Count ? null : levels[i]);
        }
        
        //OpenPage(_playerDataService.LastLevel / _maxPageItems);
        
        Subscribe(NotificationType.OpenLevelFromMenu, OnOpenLevelFromMenu);
    }

    public void OnBackButtonClick()
    {
        CloseDialog();
    }

    public void NextPage()
    {
        OpenPage(_currentPage + 1);
    }

    public void PrevPage()
    {
        _soundService.PlaySound(Sounds.Click);
        OpenPage(_currentPage - 1);
    }

    private void OpenPage(int pageIndex)
    {
        if (pageIndex < 0)
        {
            return;
        }
        
        _currentPage = pageIndex;
        
        var levels = _playerDataService.LevelStatusList;

        if (_items != null)
        {
            int i = _currentPage * _maxPageItems;
            foreach (var item in _items)
            {
                item.Init(i, i >= levels.Count ? null : levels[i]);
                i++;
            }
        }
        else
        {
            _items = new List<LevelsMenuItem>();
            for (int i = _currentPage * _maxPageItems; i < _maxPageItems * (_currentPage + 1); i++)
            {
                LevelsMenuItem item = Instantiate(_menuItemPrefab, _menuItemsParent);
                item.Init(i, i >= levels.Count ? null : levels[i]);
                _items.Add(item);
            }
        }
        
        _prevButton.SetActive(_currentPage > 0);
    }

    private void OnOpenLevelFromMenu(NotificationType notificationType, NotificationParams notificationParams)
    {
        _soundService.PlaySound(Sounds.Click);
        CloseDialog();
    }

    public void OnSwipeUp(LeanFinger finger)
    {
        //NextPage();
    }
    
    public void OnSwipeDown(LeanFinger finger)
    {
        //PrevPage();
    }
}