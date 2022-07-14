using System;
using System.Collections.Generic;
using System.Linq;
using Core.Settings;
using Core.Attributes;
using Core.Notifications;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Services
{
    [InjectionAlias(typeof(IDialogsService))]
    public class DialogsService : Service, IDialogsService
    {
        #region Fields

        private readonly Dictionary<string, IDialogController> _dialogsById =
            new Dictionary<string, IDialogController>();

        private readonly List<IDialogController> _dialogStack = new List<IDialogController>();

        private readonly Dictionary<IDialogController, Shroud> _shroudsByDialog =
            new Dictionary<IDialogController, Shroud>();

        private uint _blockingOperationsCount;
        
        private Transform _screensRoot;
        private Transform _widgetsRoot;
        private Transform _dialogsRoot;
        private Transform _loaderRoot;
        private Transform _lockerRoot;
        
        private IDialogController _lastDialog;
        
        private UILocker _uiLocker;

        [Inject] private UiSystemSettings _uiSystemSettings;

        private int _viewIdsCounter;

        private bool _isDisplayingDialog;

        private bool _pauseState;

        private float _lastOpenedDialogTime;

        private const float OPEN_DIALOG_COOLDOWN = .1f;


        #endregion

        #region Public Properties

        public Vector2 DialogsRootSize { get; set; }

        public GraphicRaycaster Raycaster { get; set; }

        public bool IsDisplayingDialog
        {
            get => _isDisplayingDialog;
            set
            {
                if (_isDisplayingDialog != value)
                {
                    Dispatch(NotificationType.DisplayingDialogStateChanged, NotificationParams.Get(value));
                }
                _isDisplayingDialog = value;
            }
        }

        public bool PauseState
        {
            get => _pauseState;
            set
            {
                if (_pauseState != value)
                {
                    //Dispatch(NotificationType.PauseStateChanged, NotificationParams.Get(value));
                    Time.timeScale = value ? 0f : 1f;
                }

                _pauseState = value;
            }
        }
        
        #endregion

        #region Properties

        protected override IList<NotificationType> ObservedNotifications =>
            new List<NotificationType>
            {
                NotificationType.ShowView,
                NotificationType.CloseView,
                NotificationType.ShowAlert,
                NotificationType.UiBlockingOperationStart,
                NotificationType.UiBlockingOperationEnd
            };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Instantiates the view prefab and adds it to the views root
        /// </summary>
        public GameObject AddViewFromPrefab(GameObject viewPrefab)
        {
            var view = Instantiate(viewPrefab, _dialogsRoot, false);
            return view;
        }

        public void SuppressUI(float duration = .2f)
        {
            Raycaster.enabled = false;
            Invoke(nameof(UnsuppressUI), duration);
        }

        public void CloseDialog(string id)
        {
            Log("Closing dialog: {0}", id);
            IDialogController dialog;
            if (_dialogsById.TryGetValue(id, out dialog))
            {
                dialog.CloseDialog();
            }
            else
            {
                throw new Exception($"Can't find dialog {id} to close");
            }
        }

        public void CloseDialogByName(ViewName viewName, bool instant = false)
        {
            Log("Closing dialog: {0}", viewName);

            var dialog = GetOpenedDialogByName(viewName);

            if (dialog != null && this != null)
            {
                dialog.CloseDialog(instant);
            }
            else
            {
                Debug.Log(string.Format("Can't find dialog {0} to close", viewName));
            }
        }

        public IDialogController CreateDialog(ViewName viewName,
            ViewCreationOptions options = ViewCreationOptions.None)
        {
            // create a new dialog
            var dialogPrefab = _uiSystemSettings.GetDialogPrefabForName(viewName);

            if (dialogPrefab == null)
            {
                throw new Exception(string.Format("The view {0} does not have prefab linked", viewName));
            }

            return CreateDialog(dialogPrefab, viewName, options);
        }

        public IDialogController GetLastDialog()
        {
            return _lastDialog;
        }

        public IDialogController GetOpenedDialogByName(ViewName viewName)
        {
            var dialogs = _dialogStack.ToList().Where(d => d.ViewName == viewName).ToList();

            IDialogController dialog = null;

            if (dialogs.Count > 0)
            {
                dialog = dialogs.Last(d => d.ViewName == viewName);
            }

            return dialog;
        }
        
        public IDialogsService SetScreensRootTransform(Transform rootTransform)
        {
            ChangeChildren(_screensRoot, rootTransform);
            
            _screensRoot = rootTransform;
            
            return this;
        }

        public IDialogsService SetWidgetsRootTransform(Transform rootTransform)
        {
            ChangeChildren(_widgetsRoot, rootTransform);

            _widgetsRoot = rootTransform;

            return this;
        }
        
        public IDialogsService SetDialogsRootTransform(Transform rootTransform)
        {
            ChangeChildren(_dialogsRoot, rootTransform);

            _dialogsRoot = rootTransform;

            return this;
        }
        
        public IDialogsService SetLoaderRootTransform(Transform rootTransform)
        {
            ChangeChildren(_loaderRoot, rootTransform);

            _loaderRoot = rootTransform;

            return this;
        }

        public IDialogsService SetLockerRootTransform(Transform rootTransform)
        {
            ChangeChildren(_lockerRoot, rootTransform);

            _lockerRoot = rootTransform;

            return this;
        }

        public Transform GetScreensRootTransform()
        {
            return _screensRoot != null ? _screensRoot : null;
        }
        
        public Transform GetWidgetsRootTransform()
        {
            return _widgetsRoot != null ? _widgetsRoot : null;
        }

        public Transform GetDialogsRootTransform()
        {
            return _dialogsRoot != null ? _dialogsRoot : null;
        }

        public Transform GetLoaderRootTransform()
        {
            return _loaderRoot != null ? _loaderRoot : null;
        }
        
        public Transform GetLockerRootTransform()
        {
            return _lockerRoot != null ? _lockerRoot : null;
        }

        #endregion

        #region Methods

        protected override void OnNotification(NotificationType notificationType, NotificationParams notificationParams)
        {
            switch (notificationType)
            {
                case NotificationType.ShowView:
                    if (Time.unscaledTime < _lastOpenedDialogTime + OPEN_DIALOG_COOLDOWN)
                    {
                        break;
                    }
                    
                    var showParams = (ShowViewNotificationParams) notificationParams;
                    var controller = CreateDialog(showParams.ViewName, showParams.Options);
                    _lastOpenedDialogTime = Time.unscaledTime;
                    controller.InitWithData(showParams.Data);
                    break;
                case NotificationType.CloseView:
                    var closeParams = (CloseViewNotificationParams) notificationParams;
                    CloseDialogByName(closeParams.ViewName, closeParams.Instant);
                    break;
                case NotificationType.UiBlockingOperationStart:
                {
                    var lockingParams = notificationParams as UiBlockingNotificationParams;
                    IncBlockingOperationsCount(lockingParams?.OperationDescription);
                    break;
                }
                case NotificationType.UiBlockingOperationEnd:
                {
                    var lockingParams = notificationParams as UiBlockingNotificationParams;
                    DecBlockingOperationsCount(lockingParams?.OperationDescription);
                    break;
                }
            }
        }

        private IDialogController CreateDialog(GameObject dialogPrefab, ViewName viewName,
            ViewCreationOptions options = ViewCreationOptions.None)
        {
            BaseViewController viewController;
            var uid = GetUniqueViewId();

            var viewObject = Instantiate(dialogPrefab);
            viewObject.SetActive(true);
            viewObject.name = string.Format("{0}_id:{1}", viewName, uid);
            viewController = viewObject.GetComponent<BaseViewController>();

            if (viewController == null)
                throw new Exception(string.Format("The dialog {0} does not have BaseDialogController attached",
                    viewName));

            viewController.ViewUID = uid;
            viewController.ViewName = viewName;

            _lastDialog = viewController;

            StartCoroutine(viewController.InitializeDialog());

            if (viewController.ViewType == ViewType.Dialog && (options & ViewCreationOptions.NoShroud) != ViewCreationOptions.NoShroud)
            {
                // Create a shroud
                var shroudObject = Instantiate(_uiSystemSettings.shroudPrefab, GetDialogsRoot(viewController.ViewType), false);

                shroudObject.name = string.Format("Shroud_{0}", viewObject.name);

                var shroud = shroudObject.GetComponent<Shroud>();
                shroud.initialize(viewController);
                StartCoroutine(shroud.show(viewController.InDuration));

                _shroudsByDialog[viewController] = shroud;
            }

            _dialogsById[uid] = viewController;
            viewController.transform.SetParent(GetDialogsRoot(viewController.ViewType), false);

            viewController.onClosingBegin += OnDialogClosingBegin;
            viewController.onClosed += OnDialogClosed;

            var isInstant = (options & ViewCreationOptions.DisplayWithoutAnimation) ==
                            ViewCreationOptions.DisplayWithoutAnimation;

            StartCoroutine(viewController.Open(isInstant));

            _dialogStack.Add(viewController);

            IsDisplayingDialog = true;
            
            if (viewController.DispatchPause)
            {
                PauseState = true;
            }

            return viewController;
        }

        private void DecBlockingOperationsCount(string operationDescription = null)
        {
            if (_blockingOperationsCount > 0) _blockingOperationsCount--;

            Log("Removed operation. Description: {0}. Blocking operations count: {1}",
                operationDescription ?? "UNKNOWN",
                _blockingOperationsCount);

            if (_blockingOperationsCount == 0 && _uiLocker != null)
            {
                _uiLocker.destroy();
                _uiLocker = null;
            }
        }

        private Transform GetDialogsRoot(ViewType viewType)
        {
            switch (viewType)
            {
                case ViewType.Screen:
                    return _screensRoot;
                    break;
                case ViewType.Widget:
                    return _widgetsRoot;
                    break;
                case ViewType.Dialog:
                    return _dialogsRoot;
                    break;
                case ViewType.Loader:
                    return _loaderRoot;
                    break;
                case ViewType.Locker:
                    return _lockerRoot;
                    break;
            }

            return null;
        }

        private string GetUniqueViewId()
        {
            return _viewIdsCounter++.ToString();
        }

        private void IncBlockingOperationsCount(string operationDescription = null)
        {
            _blockingOperationsCount++;
            Log("Added operation. Description: {0}. Blocking operations count: {1}", operationDescription ?? "UNKNOWN",
                _blockingOperationsCount);
            if (_uiLocker == null)
            {
                var lockerObject = Instantiate(_uiSystemSettings.uiLockerPrefab, _lockerRoot, false);
                _uiLocker = lockerObject.GetComponent<UILocker>();
                if (_lockerRoot == null) _lockerRoot = new GameObject("Default Locker Root").transform;
            }
        }

        private void OnDialogClosed(IDialogController controller)
        {
            Log("Dialog closed: {0}. Id = {1}", controller.ViewName, controller.ViewUID);
            controller.onClosed -= OnDialogClosed;
            controller.onClosingBegin -= OnDialogClosingBegin;
            _dialogsById.Remove(controller.ViewUID);

            if (_shroudsByDialog.TryGetValue(controller, out _)) _shroudsByDialog.Remove(controller);

            _dialogStack.Remove(controller);

            if (controller.DispatchPause && _dialogStack.Any(d => d.DispatchPause) == false)
            {
                PauseState = false;
            }

            if (_dialogStack.Count == 0)
                IsDisplayingDialog = false;
            else
                _lastDialog = _dialogStack[_dialogStack.Count - 1];
        }

        private void OnDialogClosingBegin(IDialogController controller)
        {
            Log("Dialog {0} started to close", controller.ViewUID);
            Shroud shroud;
            if (_shroudsByDialog.TryGetValue(controller, out shroud))
                StartCoroutine(shroud.hide(controller.OutDuration));
        }

        private void ChangeChildren(Transform forTranform, Transform toTransform)
        {
            if (forTranform != null)
                for (var i = 0; i < forTranform.childCount; i++)
                {
                    var child = forTranform.GetChild(i);
                    child.SetParent(toTransform, false);
                }

            forTranform = toTransform;
        }
        
        private void UnsuppressUI()
        {
            Raycaster.enabled = true;
        }
        
        #endregion
    }
}