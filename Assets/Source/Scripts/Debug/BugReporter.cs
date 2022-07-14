#if DISABLE_SRDEBUGGER == false
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;
using JetBrains.Annotations;
using SRDebugger.Internal;
using SRDebugger.Services;
using SRF.Service;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;
using Service = SRDebugger.Internal.Service;

public class BugReporter : NewMonoBehaviour
{
    #region Static Fields

    private static BugReporter _instance;

    #endregion

    #region Fields

    public GameObject ButtonsContainer;
    public Button CopyToClipboardButton;
    public InputField DescriptionField;
    public NumericStepper NumericStepper;
    public Button OpenInBrowserButton;
    public GameObject OptionsContainer;
    public Slider ProgressBar;
    public Text ResultMessageText;
    public Toggle ScreenshotToggle;
    public Toggle StackTraceToggle;
    public Button SubmitButton;
    private string _cachedScreenshotDataAsBase64String;
    private string _cachedSystemInformationAsJson;

    private ConsoleEntry[] _consoleEntries;
    private string _reportData;
    private string _reportUrl;

    #endregion

    #region Public Properties

    public static bool IsOpened => _instance != null;

    #endregion

    #region Public Methods and Operators

    public static void Hide()
    {
        if (_instance != null)
            _instance.Close();
    }

    public static void Show()
    {
        if (_instance == null)
            GameObjectUtils.CreateGameObject(Resources.Load<GameObject>("BugReporter"));
    }

    [UsedImplicitly]
    public void Close()
    {
        _instance = null;
        Destroy(gameObject, 0.1f);
    }

    [UsedImplicitly]
    public void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = _reportUrl;
    }

    [UsedImplicitly]
    public void OpenInBrowser()
    {
        Application.OpenURL(_reportUrl);
    }

    [UsedImplicitly]
    public void Submit()
    {
        EventSystem.current.SetSelectedGameObject(null);

        ProgressBar.value = 0;
        ClearErrorMessage();
        SetLoadingSpinnerVisible(true);
        SetFormEnabled(false);
        OptionsContainer.SetActive(false);

        StartCoroutine(SubmitCo());
    }

    #endregion

    #region Methods

    protected override void _Draw()
    {
        _reportData = CreateReportData();

        var dataSize = Encoding.ASCII.GetByteCount(_reportData);
        var text = string.Format("Estimated size: {0}", StringUtil.BytesToShortString(dataSize));
        ShowMessage(text);
    }

    protected override void _Start()
    {
        SRDebuggerUtil.EnsureEventSystemExists();

        _consoleEntries = Service.Console.Entries.ToArray();

        gameObject.layer = 12;

        var boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.center = new Vector3(0f, 0f, -10f);
        boxCollider.size = new Vector3(10000f, 10000f, 1f);

        ResultMessageText.text = "";

        ScreenshotToggle.isOn = false;
        StackTraceToggle.isOn = false;

        ScreenshotToggle.gameObject.SetActive(false);

        SetSubmitButtonVisible(true);
        SetLoadingSpinnerVisible(false);

//        StartCoroutine(ScreenshotCaptureCo());
    }

    private static Dictionary<string, object> GetBuildData()
    {
        var buildData = new Dictionary<string, object>();
        buildData["Version"] = App.GetSettings().AppSettings.appVersion;

        buildData["Number"] = App.GetSettings().AppSettings.appVersionCode;

        buildData["Debug"] = !App.GetSettings().AppSettings.production;

        return buildData;
    }

    private static Dictionary<string, Dictionary<string, object>> GetSystemInformation()
    {
        var systemInformation = new Dictionary<string, Dictionary<string, object>>();
        systemInformation["Build"] = GetBuildData();

        foreach (var item in SRServiceManager.GetService<ISystemInformationService>().CreateReport())
            systemInformation[item.Key] = item.Value;

        return systemInformation;
    }

    [UsedImplicitly]
    private void Awake()
    {
        _instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void ClearErrorMessage()
    {
        ResultMessageText.text = "";
        ResultMessageText.gameObject.SetActive(false);
    }

    private IList<object[]> CreateConsoleDump(bool includeStackTrace)
    {
        var list = new List<object[]>();

        var consoleEntriesLength = _consoleEntries.Length;
        var startIndex = (int) (consoleEntriesLength * (1 - NumericStepper.Value * 0.01f));
        startIndex = Mathf.Clamp(startIndex, 0, consoleEntriesLength - 1);
        for (var i = startIndex; i < consoleEntriesLength; i++)
        {
            var entry = new object[4];
            var consoleEntry = _consoleEntries[i];
            var logType = consoleEntry.LogType;
            entry[0] = logType.ToString();
            entry[1] = consoleEntry.Message;
            entry[2] = includeStackTrace || logType == LogType.Error || logType == LogType.Exception
                ? consoleEntry.StackTrace
                : "";
            //entry[3] = consoleEntry.Timestamp;

            list.Add(entry);
        }

        return list;
    }

    private string CreateReportData()
    {
        var data = new Hashtable();
        data["userDescription"] = DescriptionField.text;
        data["console"] = JsonUtility.ToJson(CreateConsoleDump(StackTraceToggle.isOn));
        data["systemInformation"] = GetCachedSystemInformationAsJson();

        if (ScreenshotToggle.isOn && BugReportScreenshotUtil.ScreenshotData != null)
            data["screenshot"] = GetCachedScreenshotDataAsBase64String();

        BugReportScreenshotUtil.ScreenshotData = null;

        return JsonUtility.ToJson(data);
    }

    private string GetCachedScreenshotDataAsBase64String()
    {
        return _cachedScreenshotDataAsBase64String ??
               (_cachedScreenshotDataAsBase64String = Convert.ToBase64String(BugReportScreenshotUtil.ScreenshotData));
    }

    private string GetCachedSystemInformationAsJson()
    {
        return _cachedSystemInformationAsJson ??
               (_cachedSystemInformationAsJson = JsonUtility.ToJson(GetSystemInformation()));
    }

    private void OnSendReportError(JSONNode data)
    {
        ShowMessage("Error sending bug report");

        SetLoadingSpinnerVisible(false);
        SetFormEnabled(true);
        OptionsContainer.SetActive(true);
    }

    private void OnSendReportSuccess(JSONNode data)
    {
        _reportUrl = data["url"];

        ShowMessage(_reportUrl);

        DescriptionField.readOnly = true;

        SetSubmitButtonVisible(false);
        SetLoadingSpinnerVisible(false);
        SetFormEnabled(true);
    }

    // ReSharper disable once UnusedMember.Local
    private IEnumerator ScreenshotCaptureCo()
    {
        SetDebugToolsVisible(false);

        // Wait for screenshot to be captured
        yield return StartCoroutine(BugReportScreenshotUtil.ScreenshotCaptureCo());

        yield return new WaitForEndOfFrame();

        SetDebugToolsVisible(true);

        // haсk to resolve the conflict with the SRDebugger console
        gameObject.GetComponent<Canvas>().sortingOrder += 1;
    }

    private void SetDebugToolsVisible(bool visible)
    {
        GetComponent<Canvas>().enabled = visible;

        var srDebuggerCanvas = GameObject.Find("SRDebugger/Panel/Canvas");
        if (srDebuggerCanvas != null)
            srDebuggerCanvas.GetComponent<Canvas>().enabled = visible;
    }

    private void SetFormEnabled(bool e)
    {
        SubmitButton.interactable = e;
        DescriptionField.interactable = e;
    }

    private void SetLoadingSpinnerVisible(bool visible)
    {
        ProgressBar.gameObject.SetActive(visible);
        ButtonsContainer.SetActive(!visible);
    }

    private void SetSubmitButtonVisible(bool visible)
    {
        SubmitButton.gameObject.SetActive(visible);
        CopyToClipboardButton.gameObject.SetActive(!visible);
        OpenInBrowserButton.gameObject.SetActive(!visible);
    }

    private void ShowMessage(string message)
    {
        ResultMessageText.text = message;
        ResultMessageText.gameObject.SetActive(true);
    }

    private IEnumerator SubmitCo()
    {
        /*if (BugReportScreenshotUtil.ScreenshotData == null)
        {
            SetDebugToolsVisible(false);

            yield return new WaitForEndOfFrame();

            // Wait for screenshot to be captured
            yield return StartCoroutine(BugReportScreenshotUtil.ScreenshotCaptureCo());

            SetDebugToolsVisible(true);

            // haсk to resolve the conflict with the SRDebugger console
            gameObject.GetComponent<Canvas>().sortingOrder += 1;
        }*/

        yield return new WaitForEndOfFrame();

        //TODO SEND REPORT
        //new ClientReportServerCommand(_reportData).execute(OnSendReportSuccess, OnSendReportError);
    }

    #endregion
}
#endif