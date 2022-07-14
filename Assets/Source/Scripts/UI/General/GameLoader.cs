using System;
using System.Collections;
using System.Collections.Generic;
using Aig.Client.Integration.Runtime.Analytics;
using Aig.Client.Integration.Runtime.Subsystem;
using Core;
using Core.Settings;
using Core.Attributes;
using Core.Notifications;
using Core.Services;
using Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using VertexDI.Game;

/// <summary>
///     Responsible for the initial boot and loading of the game
/// </summary>
public class GameLoader : DIBehaviour
{
    #region Fields

    [Inject]
    private ApplicationSettings _applicationSettings;
    
    [Inject]
    private IDialogsService _dialogsService;

    [Inject]
    private IPlayerDataService _playerDataService;

    /// <summary>
    ///     The instance of the game root
    /// </summary>
    private GameObject _gameContainer;

    /// <summary>
    ///     The instance of the loading screen
    /// </summary>
    private LoadingLogo _loadingLogo;

    private GameObject _splashScreen;

    private Transform _viewScreensContainer;
    private Transform _viewWidgetsContainer;
    private Transform _viewDialogsContainer;
    private Transform _viewLoadingContainer;
    private Transform _viewLockContainer;
    
    /// <summary>
    ///     The instance of the views root
    /// </summary>
    private ViewsContainer _viewsContainer;

    /// <summary>
    ///     Loading screen prefab
    /// </summary>
    [SerializeField] private GameObject loadingLogoPrefab;

    /// <summary>
    ///     Splash screen prefab
    /// </summary>
    [SerializeField] private GameObject splashScreenPrefab;

    /// <summary>
    ///     Prefab for the root views object that UI and Dialogs are under
    /// </summary>
    [SerializeField] private ViewsContainer viewsContainerPrefab;

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///     Call to load the game
    /// </summary>
    public GameObject Load()
    {
        IntegrationSubsystem.Instance.AnalyticsService.Technical("01_StartGameLoader", _playerDataService.IsFirstLaunch);

        DontDestroyOnLoad(gameObject);

        _gameContainer = new GameObject {name = "GameContainer"};
        DontDestroyOnLoad(_gameContainer);

        StartCoroutine(LoadCo());
        return _gameContainer;
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Handles initial load step
    /// </summary>
    private IEnumerator LoadCo()
    {
        //Instantiate(_mainCameraPrefab, _gameContainer.transform, false);

        IntegrationSubsystem.Instance.AnalyticsService.Technical("02_GameLoader", _playerDataService.IsFirstLaunch);

        if (splashScreenPrefab != null)
        {
            _splashScreen = Instantiate(splashScreenPrefab);
            _loadingLogo = Instantiate(loadingLogoPrefab).GetComponent<LoadingLogo>();
            _loadingLogo.canvasGroup.alpha = 0;
            yield return _loadingLogo.canvasGroup.GetComponent<CanvasGroup>().DOFade(1.0f, 0.3f).SetDelay(2.0f)
                .WaitForCompletion();

            Destroy(_splashScreen);
        }
        else
        {
            _loadingLogo = Instantiate(loadingLogoPrefab).GetComponent<LoadingLogo>();
        }

        _viewsContainer = Instantiate(viewsContainerPrefab, _gameContainer.transform, false);
        _viewsContainer.gameObject.name = "ViewsContainer";
        //DontDestroyOnLoad(_viewsContainer.gameObject);

        //var lockerContainer = Instantiate(lockerContainerPrefab, _gameContainer.transform, false);
        //lockerContainer.name = "LockerContainer";
        //DontDestroyOnLoad(lockerContainer);

        _dialogsService.DialogsRootSize = ((RectTransform) _viewsContainer.transform).sizeDelta;

        _dialogsService.SetScreensRootTransform(_viewsContainer.ScreensLayer)
            .SetWidgetsRootTransform(_viewsContainer.WidgetsLayer)
            .SetDialogsRootTransform(_viewsContainer.DialogsLayer)
            .SetLoaderRootTransform(_viewsContainer.LoaderLayer)
            .SetLockerRootTransform(_viewsContainer.LockerLayer);

        _dialogsService.Raycaster = _viewsContainer.Raycaster;
        
        Dispatch(NotificationType.UiBlockingOperationStart);

        //_gameContainer.AddComponent<DebugTools>();

        IntegrationSubsystem.Instance.AnalyticsService.Technical("03_EndGameLoader", _playerDataService.IsFirstLaunch);

        yield return new WaitForSeconds(0.5f);

        if (_playerDataService.IsGdprApplied == false)
        {
            ShowGDPR();
        }
        else
        {
            StartUp();
        }
    }

    private void ShowGDPR()
    {
        Subscribe(NotificationType.PrivacyDialogClosed, OnPrivacyDialogClosedHandler);
        Dispatch(NotificationType.ShowView, ShowViewNotificationParams.Get(ViewName.PrivacyDialog, ViewCreationOptions.NoShroud));

        IntegrationSubsystem.Instance.AnalyticsService.Technical("04_GDPR", _playerDataService.IsFirstLaunch);

        _viewsContainer.GetComponent<CanvasGroup>().alpha = 1.0f;
        _viewsContainer.GetComponent<Canvas>().sortingOrder = 10;

        Dispatch(NotificationType.UiBlockingOperationEnd);
    }

    private void OnPrivacyDialogClosedHandler(NotificationType notificationType, NotificationParams notificationParams)
    {
        Dispatch(NotificationType.UiBlockingOperationStart);
        IntegrationSubsystem.Instance.AnalyticsService.CustomEvent("pop_up", new Dictionary<string, object>()
        {
            {"pop_up_id", "gdpr"},
            {"show_reason", "first_launch"},
            {"result", "accept"}
        }, false, AnalyticType.AppMetrica);

        _viewsContainer.GetComponent<Canvas>().sortingOrder = 1;

        StartUp();
    }

    private void StartUp()
    {
        IntegrationSubsystem.Instance.AdsService.RunServiceManual();

        StartCoroutine(StartUpCo());
    }

    /// <summary>
    /// </summary>
    private IEnumerator StartUpCo()
    {
        IntegrationSubsystem.Instance.AnalyticsService.Technical("05_Startup", _playerDataService.IsFirstLaunch);

        /*
        OperationResult<ConfigResponse> configRequestResult = new OperationResult<ConfigResponse>();
        yield return _appDataService.getConfig(configRequestResult);
        if (!checkOperationSuccess(configRequestResult)) yield break;

        OperationResult<LoginResponse> loginResult = new OperationResult<LoginResponse>();
        yield return handlePlayerLogin(loginResult);
        if (!checkOperationSuccess(loginResult)) yield break;

        OperationResult<PlayerResponse> playerRequestResult = new OperationResult<PlayerResponse>();
        yield return _playerDataService.getPlayerData(playerRequestResult);
        if (!checkOperationSuccess(playerRequestResult)) yield break;

        OperationResult<Dictionary<string, int>> playerResourcesRequestResult = new OperationResult<Dictionary<string, int>>();
        yield return _playerResourcesService.getResources(ResourcesKeys.ALL, playerResourcesRequestResult);
        if (!checkResourcesOoperationSuccess(playerResourcesRequestResult)) yield break;

        if (_gameSettings.ResetAllPlayerData) {
            resetPlayerResources();
        }

        StartCoroutine(_storeService.getIAPPackages());
        StartCoroutine(_storeService.getStore());
        */

        //_loadingLogo.GetComponent<CanvasGroup>().DOFade(1f, 0.33f);
        //_viewsContainer.GetComponent<CanvasGroup>().DOFade(0f, 0.33f);
        //yield return new WaitForSeconds(1.34f);

        if (_viewsContainer.GetComponent<CanvasGroup>().alpha <= 0f)
            new List<ViewName>().ForEach(view =>
            {
                _dialogsService.CreateDialog(view,
                    ViewCreationOptions.DisplayWithoutAnimation | ViewCreationOptions.NoShroud);
            });

        /*
        OperationResult<LobbyResponse> lobbyRequestResult = new OperationResult<LobbyResponse>();
        yield return createLobby(lobbyRequestResult);
        if (!checkOperationSuccess(lobbyRequestResult)) yield break;
        */

        //yield return new TransitionCameraAndWait(MainCameraState.Lobby);

        var asyncOperation = SceneManager.LoadSceneAsync(Scenes.GAME);
        while (!asyncOperation.isDone)
        {
            _loadingLogo.SetLoadingProgress(0.5f + asyncOperation.progress * 0.5f);
            yield return null;
        }

        _loadingLogo.SetLoadingProgress(1.0f);
        yield return _loadingLogo.WaitForProgress();

        //yield return new WaitAsyncOperationIsDone(asyncOperation);
        yield return new WaitForSeconds(0.5f);

        _viewsContainer.GetComponent<CanvasGroup>().alpha = 1.0f;
        yield return _loadingLogo.FadeOut();

        //Dispatch(NotificationType.ShowView, ShowViewNotificationParams.Get(ViewName.LoadingScreen));

        Dispatch(NotificationType.LoadingLogoFaded);
        //IntegrationSubsystem.Instance.AnalyticsService.Technical("06_EndGameLoader", _playerDataService.IsFirstLaunch);

        Destroy(_loadingLogo.gameObject);
        Destroy(gameObject);
    }

    #endregion
}