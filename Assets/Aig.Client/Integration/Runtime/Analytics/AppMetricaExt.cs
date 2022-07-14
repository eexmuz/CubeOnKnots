#if  AIG_ANALYTICS_APP_METRICA
using UnityEngine;
using System.Collections;

public class AppMetricaExt : MonoBehaviour
{
    public const string VERSION = "3.7.0";

    [SerializeField] public string ApiKey;

    [SerializeField] public bool ExceptionsReporting = true;

    [SerializeField] public uint SessionTimeoutSec = 10;

    [SerializeField] public bool LocationTracking = true;

    [SerializeField] public bool Logs = true;

    [SerializeField] public bool HandleFirstActivationAsUpdate = false;

    [SerializeField] public bool StatisticsSending = true;

    private static bool _isInitialized = false;
    private bool _actualPauseStatus = false;

    private static IYandexAppMetrica _metrica = null;
    private static object syncRoot = new Object();

    public static IYandexAppMetrica Instance
    {
        get
        {
            if (_metrica == null)
            {
                lock (syncRoot)
                {
#if UNITY_IPHONE || UNITY_IOS
                    if (_metrica == null && Application.platform == RuntimePlatform.IPhonePlayer) {
                        _metrica = new YandexAppMetricaIOS ();
                    }
#elif UNITY_ANDROID
                    if (_metrica == null && Application.platform == RuntimePlatform.Android)
                    {
                        _metrica = new YandexAppMetricaAndroid();
                    }
#endif
                    if (_metrica == null)
                    {
                        _metrica = new YandexAppMetricaDummy();
                    }
                }
            }

            return _metrica;
        }
    }

    void SetupMetrica()
    {
        var configuration = new YandexAppMetricaConfig(ApiKey)
        {
            SessionTimeout = (int) SessionTimeoutSec,
            Logs = Logs,
            HandleFirstActivationAsUpdate = HandleFirstActivationAsUpdate,
            StatisticsSending = StatisticsSending,
        };

#if !APP_METRICA_TRACK_LOCATION_DISABLED
        configuration.LocationTracking = LocationTracking;
        // if (LocationTracking) {
        //     Input.location.Start ();
        // }
#else
        configuration.LocationTracking = false;
#endif

        Instance.ActivateWithConfiguration(configuration);
    }

    private void Awake()
    {
        if (!_isInitialized)
        {
            _isInitialized = true;
            SetupMetrica();
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        Instance.ResumeSession();
    }

    private void OnEnable()
    {
        if (ExceptionsReporting)
        {
#if UNITY_5 || UNITY_5_3_OR_NEWER
            Application.logMessageReceived += HandleLog;
#else
            Application.RegisterLogCallback(HandleLog);
#endif
        }
    }

    private void OnDisable()
    {
        if (ExceptionsReporting)
        {
#if UNITY_5 || UNITY_5_3_OR_NEWER
            Application.logMessageReceived -= HandleLog;
#else
            Application.RegisterLogCallback(null);
#endif
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (_actualPauseStatus != pauseStatus)
        {
            _actualPauseStatus = pauseStatus;
            if (pauseStatus)
            {
                Instance.PauseSession();
            }
            else
            {
                Instance.ResumeSession();
            }
        }
    }

    private void HandleLog(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Exception)
        {
            Instance.ReportError(condition, condition, stackTrace);
        }
    }

}
#endif
