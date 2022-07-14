using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public delegate void ShroudClosedHandler(Shroud shroud);

    /// <summary>
    ///     Class is used to provide visual contrast for dialogs displayed, and close out displayed dialogs if selected.
    /// </summary>
    public class Shroud : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Color shroudColor = new Color(0f, 0f, 0f, 0.85f);

        private BaseViewController _displayedView;

        private bool _isInteractable = true;

        private IDialogController _linkedController;
        [SerializeField] private Button button;

        [SerializeField] private CanvasGroup shroudGroup;
        [SerializeField] private Image shroudImage;
        [SerializeField] private Transform shroudParent;
        [SerializeField] private int shroudSiblingIndex;

        [Space(10)] [SerializeField] private Transform shroudsTransform;

        #endregion

        #region Public Events

        public event ShroudClosedHandler onClosed;

        #endregion

        #region Public Properties

        public bool IsAnimating { get; private set; }

        #endregion

        #region Public Methods and Operators

        /*
    /// <summary>
    /// Move the shroud above the currently selected dialog assigned to be displayed.
    /// </summary>
    public IEnumerator moveShroud_co()
    {
        _isAnimating = true;
        _displayedDialog = DialogManagerService.dialogStack.Peek();
        Transform _displayedDialogsTransform = _displayedDialog.transform;

        // Update Position in Scene Hierarchy
        _shroudsTransform.SetParent(_displayedDialogsTransform.parent, true);
        _shroudsTransform.SetSiblingIndex(_displayedDialogsTransform.GetSiblingIndex());

        _shroudImage.enabled = true;
        Color startingColor = _shroudImage.color;

        float time = 0f;
        float duration = DialogManagerService.dialogStack.Peek().inDuration;
        while(time / duration < 1f)
        {
            _shroudImage.color = Color.Lerp(startingColor, _shroudColor, time / duration);
            time += Time.deltaTime;

            yield return null;
        }

        _shroudImage.color = _shroudColor;
        _shroudGroup.interactable = true;
        _isAnimating = false;
    }
    */
        /*
    /// <summary>
    /// Reset the shroud to its original position, and fade out both it and the selected dialog.
    /// </summary>
    public IEnumerator returnShroud_co()
    {
        _isAnimating = true;
        _shroudGroup.interactable = false;
        if(Tooltip.isInspected(_displayedDialog.transform))
            Tooltip.resetAll();

        Color startingColor = _shroudImage.color;

        float time = 0f;
        float duration = DialogManagerService.dialogStack.Peek().outDuration;
        while (time / duration < 1f)
        {
            _shroudImage.color = Color.Lerp(startingColor, Color.clear, time / duration);
            time += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        _shroudImage.color = Color.clear;
        _shroudImage.enabled = false;
        _isAnimating = false;

        // Update Position in Scene Hierarchy
        _shroudsTransform.SetParent(_shroudParent, true);
        _shroudsTransform.SetSiblingIndex(_shroudSiblingIndex);
    }       
    */

        public void destroy()
        {
            Destroy(gameObject);
        }

        public IEnumerator hide(float duration = 0)
        {
            _isInteractable = false;
            if (duration > 0)
            {
                float time = 0;
                var startColor = shroudColor;
                while (time < duration)
                {
                    shroudImage.color = Color.Lerp(startColor, Color.clear, time / duration);
                    time += Time.unscaledDeltaTime;
                    yield return 0;
                }
            }

            if (onClosed != null) onClosed(this);

            Destroy(gameObject);
        }

        public Shroud initialize(IDialogController linkedController)
        {
            _linkedController = linkedController;
            return this;
        }

        public IEnumerator show(float duration = 0)
        {
            _isInteractable = false;
            shroudImage.enabled = true;
            shroudGroup.interactable = true;
            shroudGroup.blocksRaycasts = true;

            if (duration > 0)
            {
                float time = 0;
                while (time < duration)
                {
                    shroudImage.color = Color.Lerp(Color.clear, shroudColor, time / duration);
                    time += Time.unscaledDeltaTime;
                    yield return 0;
                }
            }

            shroudImage.color = shroudColor;
            IsAnimating = false;
            _isInteractable = true;
        }

        #endregion

        #region Methods

        private void Awake()
        {
            button.onClick.AddListener(() =>
            {
                if (!_isInteractable) return;
                if (_linkedController != null)
                    _linkedController.OnShroudClicked();
                else
                    Debug.LogError("The Shroud item does not have a dialog controller linked");
            });
        }

        #endregion
    }
}