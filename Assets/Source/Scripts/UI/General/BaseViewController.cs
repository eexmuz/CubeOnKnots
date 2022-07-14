using System.Collections;
using Core;
using Core.Attributes;
using Core.Notifications;
using Core.Services;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum ViewType
    {
        Screen,
        Widget,
        Dialog,
        Loader,
        Locker
    }

    /// <summary>
    ///     Base class for all dialog that control how they are diaplayed and hidden.
    /// </summary>
    [RequireComponent(typeof(Animation))]
    [RequireComponent(typeof(CanvasGroup))]
    public class BaseViewController : DIBehaviour, IDialogController
    {
        #region Fields

        [Inject] protected IDialogsService _dialogsService;
        [Inject] protected ISoundService _soundService;

        [SerializeField] public ViewType ViewType;
        
        [SerializeField] protected CanvasGroup canvasGroup;

        [SerializeField] private bool _pause;

        private bool _isCloseEnable = true;
        private Coroutine _closeCoroutine;

        [Header("Art Team's Animation")]
        [SerializeField] private Animation dialogAnimation;
        [SerializeField] private AnimationClip showClip;
        [SerializeField] private AnimationClip hideClip;
        
        [Header("Default Animation")]
        [SerializeField] private AnimationCurve inCurve;
        [SerializeField] private float inDuration = .33f;

        [SerializeField] private AnimationCurve outCurve;
        [SerializeField] private float outDuration = .33f;

        [SerializeField] private bool animAlpha = true;
        [SerializeField] private bool animScale = true;

        #endregion

        #region Public Events

        public event DialogEventHandler onClosed;
        public event DialogEventHandler onClosingBegin;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Canvas group of the base dialog.
        /// </summary>
        public CanvasGroup CanvasGroup
        {
            get => canvasGroup;
            set => canvasGroup = value;
        }

        public ViewName ViewName { get; set; }
        
        public string ViewUID { get; set; }

        public float InDuration => inDuration <= 0 ? 0.01f : inDuration;

        /// <summary>
        ///     Is the dialog currently displaying or hiding itself.
        /// </summary>
        public bool IsAnimating { get; set; }

        public virtual bool IsCloseEnable
        {
            get => _isCloseEnable;
            set => _isCloseEnable = value;
        }

        public float OutDuration => outDuration <= 0 ? 0.01f : outDuration;

        public bool DispatchPause => _pause;
        
        #endregion

        #region Public Methods and Operators

        private IEnumerator Close(bool instant = false)
        {
            if (onClosingBegin != null)
            {
                onClosingBegin(this);
            }

            if (ViewType == ViewType.Screen)
            {
                instant = true;
            }

            if (!instant)
            {
                if (dialogAnimation == null || hideClip == null)
                {
                    var time = 0f;
                    while (outCurve.Evaluate(time / outDuration) < 1f)
                    {
                        if (animAlpha && canvasGroup != null)
                            canvasGroup.alpha = Mathf.Lerp(1, 0, outCurve.Evaluate(time / outDuration));

                        if (animScale)
                            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero,
                                outCurve.Evaluate(time / outDuration));

                        time += Time.unscaledDeltaTime;
                        yield return new WaitForEndOfFrame();

                        if (this == null) yield break;
                    }

                    if (animAlpha && canvasGroup != null) canvasGroup.alpha = 0f;

                    if (animScale) transform.localScale = Vector3.zero;
                }
                else
                {
                    dialogAnimation.clip = hideClip;
                    dialogAnimation.Play();
                    yield return new WaitForSecondsRealtime(hideClip.length);
                    if (this == null) yield break;
                }
            }

            if (this == null) yield break;

            transform.localScale = Vector3.zero;

            if (canvasGroup != null) canvasGroup.alpha = 0;

            IsAnimating = false;

            onClosedInternal();

            Destroy(gameObject);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        /// <summary>
        ///     Pull the data needed for populating the dialog from the server
        /// </summary>
        public virtual void GetDialogData()
        {
        }

        public virtual IEnumerator InitializeDialog()
        {
            if (dialogAnimation == null || showClip == null)
            {
                if (animAlpha)
                    canvasGroup.alpha = 0f;

                if (animScale)
                    transform.localScale = Vector3.zero;
            }

            yield return WaitForStarted();
            // your stuff
        }

        public virtual void InitWithData(object data)
        {
        }

        public void OnDisable()
        {
            IsAnimating = false;
        }

        public virtual void OnShroudClicked()
        {
            if (_isCloseEnable)
                CloseDialog();
        }

        public virtual void CloseDialog(bool instant = false)
        {
            if (_isCloseEnable && _closeCoroutine == null)
            {
                _closeCoroutine = StartCoroutine(Close(instant));
            }
        }

        public IEnumerator Open(bool instant = false)
        {
            if (ViewType == ViewType.Screen)
            {
                instant = true;
            }

            if (!instant)
            {
                if (dialogAnimation == null || showClip == null)
                {
                    var time = 0f;
                    while (inCurve.Evaluate(time / inDuration) < 1f)
                    {
                        if (animAlpha) canvasGroup.alpha = Mathf.Lerp(0, 1, inCurve.Evaluate(time / inDuration));

                        if (animScale)
                            transform.localScale =
                                Vector3.Lerp(Vector3.zero, Vector3.one, inCurve.Evaluate(time / inDuration));

                        time += Time.unscaledDeltaTime;
                        yield return new WaitForEndOfFrame();
                    }

                    if (animAlpha) canvasGroup.alpha = 1f;

                    if (animScale) transform.localScale = Vector3.one;
                }
                else
                {
                    dialogAnimation.clip = showClip;
                    dialogAnimation.Play();
                    yield return new WaitForSecondsRealtime(showClip.length);
                }
            }

            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            IsAnimating = false;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        ///     Set the listners for dialog buttons
        /// </summary>
        public virtual void SetButtonListeners()
        {
            foreach (var button in GetComponentsInChildren<Button>())
            {
                button.onClick.AddListener(() => _soundService.PlaySound(Sounds.Click));
            }

            foreach (var toggle in GetComponentsInChildren<Toggle>())
            {
                toggle.onValueChanged.AddListener(state => _soundService.PlaySound(Sounds.Click));
            }
        }

        /// <summary>
        ///     Set the data for the dialog gathered
        /// </summary>
        public virtual void SetDialogData()
        {
            if (showClip != null)
            {
                inDuration = showClip.length;
                if (null == dialogAnimation.GetClip(showClip.name))
                {
                    Debug.LogError(
                        this + " - Does not have its show clip assigned to its Animation component in the inspector.");
                    dialogAnimation.AddClip(showClip, showClip.name);
                }
            }

            if (hideClip != null)
            {
                outDuration = hideClip.length;
                if (null == dialogAnimation.GetClip(hideClip.name))
                {
                    Debug.LogError(
                        this + " - Does not have its hide clip assigned to its Animation component in the inspector.");
                    dialogAnimation.AddClip(hideClip, hideClip.name);
                }
            }
        }

        /// <summary>
        ///     Init Dialog data after showing
        /// </summary>
        /// <summary>
        ///     Start the process for displaying the dialog gradually or instantly.
        /// </summary>
        public virtual void StartDisplay(bool instant)
        {
            if (IsAnimating)
                return;

            IsAnimating = true;

            // Ensure OnEnable is Called when Displaying
            gameObject.SetActive(true);

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            if (instant)
            {
                transform.localScale = Vector3.one;
                canvasGroup.alpha = 1f;

                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                IsAnimating = false;
            }
            else if (gameObject.activeInHierarchy)
            {
                StartCoroutine(display_co());
            }

            //StartCoroutine( InitializeDialog() );
        }

        /// <summary>
        ///     Start the process for hiding a the dialog gradually or instantly.
        /// </summary>
        public virtual void StartHide(bool instant)
        {
            if (IsAnimating)
                return;

            IsAnimating = true;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            if (instant)
            {
                onClosedInternal();
                Destroy(gameObject);
            }
            else if (gameObject.activeInHierarchy)
            {
                onClosedInternal();
                StartCoroutine(hide_co());
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fade and scale in the dialog.
        /// </summary>
        protected virtual IEnumerator display_co()
        {
            if (dialogAnimation == null || showClip == null)
            {
                var time = 0f;
                while (inCurve.Evaluate(time / inDuration) < 1f)
                {
                    if (animAlpha)
                        canvasGroup.alpha = Mathf.Lerp(0, 1, inCurve.Evaluate(time / inDuration));

                    if (animScale)
                        transform.localScale =
                            Vector3.Lerp(Vector3.zero, Vector3.one, inCurve.Evaluate(time / inDuration));

                    time += Time.unscaledDeltaTime;
                    yield return new WaitForEndOfFrame();
                }

                if (animAlpha)
                    canvasGroup.alpha = 1f;

                if (animScale)
                    transform.localScale = Vector3.one;
            }
            else
            {
                dialogAnimation.clip = showClip;
                dialogAnimation.Play();
                yield return new WaitForSecondsRealtime(showClip.length);
            }

            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            IsAnimating = false;
        }

        protected void EndBlockingOperation(string description = null)
        {
            UiBlockingNotificationParams notificationParams = null;
            if (description != null)
                notificationParams = new UiBlockingNotificationParams
                {
                    OperationDescription = description
                };
            Dispatch(NotificationType.UiBlockingOperationEnd, notificationParams);
        }

        protected void HandleCloseButton()
        {
            if (IsCloseEnable)
                _dialogsService.CloseDialog(ViewUID);
            //StartCoroutine(_dialogManagerService.hideLastDisplayed_co());
        }

        /// <summary>
        ///     Fade and scale out the dialog.
        /// </summary>
        protected virtual IEnumerator hide_co()
        {
            if (dialogAnimation == null || hideClip == null)
            {
                var time = 0f;
                while (outCurve.Evaluate(time / outDuration) < 1f)
                {
                    if (animAlpha)
                        canvasGroup.alpha = Mathf.Lerp(1, 0, outCurve.Evaluate(time / outDuration));

                    if (animScale)
                        transform.localScale =
                            Vector3.Lerp(Vector3.one, Vector3.zero, outCurve.Evaluate(time / outDuration));

                    time += Time.unscaledDeltaTime;
                    yield return new WaitForEndOfFrame();
                }

                if (animAlpha)
                    canvasGroup.alpha = 0f;

                if (animScale)
                    transform.localScale = Vector3.zero;
            }
            else
            {
                dialogAnimation.clip = hideClip;
                dialogAnimation.Play();
                yield return new WaitForSecondsRealtime(hideClip.length);
            }

            IsAnimating = false;
            gameObject.SetActive(false);
        }

        protected override void OnAppInitialized()
        {
            base.OnAppInitialized();

            GetDialogData();
            SetDialogData();
            SetButtonListeners();
        }

        protected virtual void onClosedInternal()
        {
            onClosed?.Invoke(this);
        }

        protected void StartBlockingOperation(string description = null)
        {
            UiBlockingNotificationParams notificationParams = null;
            if (description != null)
                notificationParams = new UiBlockingNotificationParams
                {
                    OperationDescription = description
                };
            Dispatch(NotificationType.UiBlockingOperationStart, notificationParams);
        }

        #endregion
    }
}