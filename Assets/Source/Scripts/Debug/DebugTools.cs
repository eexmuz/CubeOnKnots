using Core;
using Core.Settings;
using Core.Attributes;

//using Lean.Touch;

public class DebugTools : DIBehaviour
{
#if DISABLE_SRDEBUGGER == false
    #region Fields

    //private List<LeanFinger> _fingers;
    //private IDebugService _srDebug;
    //private BoxCollider _srDebuggerBoxCollider;

    [Inject] private ApplicationSettings _applicationSettings;

    #endregion

    #region Methods

    protected override void OnAppInitialized()
    {
        base.OnAppInitialized();

        /*
        _fingers = LeanTouch.Fingers;

        _srDebug = SRDebug.Instance;
        _srDebug.PanelVisibilityChanged += OnSrDebugPanelVisibilityChanged;

        var srDebugger = GameObject.Find("SRDebugger");
        srDebugger.layer = 12;

        _srDebuggerBoxCollider = srDebugger.AddComponent<BoxCollider>();
        _srDebuggerBoxCollider.center = new Vector3(0f, 0f, -10f);
        _srDebuggerBoxCollider.size = new Vector3(10000f, 10000f, 1f);
        _srDebuggerBoxCollider.enabled = false;
        */

        if (_applicationSettings.production)
        {
            Destroy(this);
            return;
        }

        SRDebug.Init();
    }

    #endregion

    /*
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (!_srDebug.IsDebugPanelVisible && !BugReporter.IsOpened)
            {
                _srDebug.ShowDebugPanel(false);
            }
            else if (_srDebug.IsDebugPanelVisible)
            {
                _srDebug.HideDebugPanel();
            }
            return;
        }

        if (_fingers == null || _fingers.Count < 2)
        {
            return;
        }

        // Get the fingers we want to use
        var fingers = LeanTouch.GetFingers(false, 2);

        // Calculate the scaling values based on these fingers
        var pinchScale = LeanGesture.GetPinchScale(fingers);

        if (pinchScale > 2.0f && !_srDebug.IsDebugPanelVisible && !BugReporter.IsOpened)
        {
            _srDebug.ShowDebugPanel(false);
        }
        else if (pinchScale < 0.8f && _srDebug.IsDebugPanelVisible)
        {
            _srDebug.HideDebugPanel();
        }

        /*
        if ((LeanTouch.PinchScale > 1.2f) && !_srDebug.IsDebugPanelVisible && !BugReporter.IsOpened)
        {
            _srDebug.ShowDebugPanel(false);
        }
        else if ((LeanTouch.PinchScale < 0.8f) && _srDebug.IsDebugPanelVisible)
        {
            _srDebug.HideDebugPanel();
        }
        else
        {
            var multiDragDeltaY = LeanTouch.MultiDragDelta.y;
            var absMultiDragDeltaX = Mathf.Abs(LeanTouch.MultiDragDelta.x);
            var k = multiDragDeltaY/Screen.height;
            if ((multiDragDeltaY < -absMultiDragDeltaX) && (k < -0.07f) && !BugReporter.IsOpened)
            {
                BugReporter.Show();
            }
            else if ((multiDragDeltaY > absMultiDragDeltaX) && (k > 0.07f) && BugReporter.IsOpened)
            {
                BugReporter.Hide();
            }
        }
        */
    //}
    /*
    private void OnSrDebugPanelVisibilityChanged(bool isVisible)
    {
        _srDebuggerBoxCollider.enabled = isVisible;
    }
    */
#endif
}