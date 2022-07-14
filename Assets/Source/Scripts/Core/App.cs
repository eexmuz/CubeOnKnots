using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aig.Client.Integration.Runtime.Subsystem;
using Core.Instructions;
using Core.Subsystems;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

namespace Core
{
    /// <summary>
    ///     This is the main application class and main entry point.
    /// </summary>
    public class App : MonoBehaviour, INotificationDispatcher
    {
        #region Static Fields

        /// <summary>
        ///     Gets the single instance of the object
        /// </summary>
        public static App Instance;

        /// <summary>
        ///     The dependencies injector.
        /// </summary>
        private static readonly Injector Injector = new Injector(GetInjectionBinding);

        #endregion

        #region Fields

        /// <summary>
        ///     The injection map.
        ///     Subsystems, Services and Settings are added automatically as long as they have [InjectionAlias] attribute.
        /// </summary>
        private readonly IDictionary<Type, object> _injectionMap = new Dictionary<Type, object>();

        /// <summary>
        ///     The app-wide notification dispatcher.
        /// </summary>
        private readonly INotificationDispatcher _notificationDispatcher = new NotificationDispatcher();

        /// <summary>
        ///     Collection of all subsystems
        /// </summary>
        private readonly List<ISubsystem> _subsystems = new List<ISubsystem>();

        /// <summary>
        ///     The root object of the game.
        /// </summary>
        private GameObject _gameRoot;

        /// <summary>
        ///     Application and Subsystem settings
        /// </summary>
        private SettingsContainer _settingsContainer;

        #endregion

        #region Public Properties

        public static INotificationDispatcher SharedNotificationDispatcher => Instance;

        /// <summary>
        ///     The root object of the game.
        /// </summary>
        public GameObject GameRoot
        {
            get => _gameRoot;
            set => _gameRoot = value;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get the current settings object of the app
        /// </summary>
        public static SettingsContainer GetSettings()
        {
            return Instance._settingsContainer;
        }

        /// <summary>
        ///     Call to have all dependencies injected into an object
        /// </summary>
        public static void Inject(object instance)
        {
            InjectDependencies(instance);
        }

        /// <summary>
        ///     Call to have field dependencies injected into an object
        /// </summary>
        public static void InjectFields(object instance)
        {
            InjectFieldDependencies(instance);
        }

        /// <summary>
        ///     Call to have property dependencies injected into an object
        /// </summary>
        public static void InjectProperties(object instance)
        {
            InjectPropertyDependencies(instance);
        }

        /// <summary>
        ///     Log a message to the console.
        /// </summary>
        public static void Log(object message)
        {
            Debug.Log(message);
        }

        /// <summary>
        ///     Log a message to the console.
        /// </summary>
        public static void Log(string format, params object[] args)
        {
            Debug.LogFormat(format, args);
        }

        /// <summary>
        ///     Log an error to the console.
        /// </summary>
        public static void LogError(object message)
        {
            Debug.LogError(message);
        }

        /// <summary>
        ///     Log an error to the console.
        /// </summary>
        public static void LogError(string format, params object[] args)
        {
            Debug.LogErrorFormat(format, args);
        }

        /// <summary>
        ///     Log a warning to the console.
        /// </summary>
        public static void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }

        /// <summary>
        ///     Log a warning to the console.
        /// </summary>
        public static void LogWarning(string format, params object[] args)
        {
            Debug.LogWarningFormat(format, args);
        }

        public void Dispatch(NotificationType notificationType, NotificationParams notificationParams = null)
        {
            _notificationDispatcher.Dispatch(notificationType, notificationParams);
        }

        public void Subscribe(NotificationType notificationType, NotificationHandler handler)
        {
            _notificationDispatcher.Subscribe(notificationType, handler);
        }

        public void Unsubscribe(NotificationType notificationType, NotificationHandler handler)
        {
            _notificationDispatcher.Unsubscribe(notificationType, handler);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Add a new subsystem to the application
        /// </summary>
        private static void AddSubsystem<T>() where T : Component
        {
            var subsystem = Instance.gameObject.AddComponent<T>() as ISubsystem;
            Instance._subsystems.Add(subsystem);
        }


        /// <summary>
        ///     Get any component from the application by type.
        /// </summary>
        private static object Get(Type type)
        {
            return Instance._subsystems.FirstOrDefault(subsystem => subsystem.GetType() == type) ??
                   (object) GetAllServices().FirstOrDefault(subsystem => subsystem.GetType() == type) ??
                   Instance._settingsContainer.ActiveSettings.FirstOrDefault(settings => settings.GetType() == type);
        }



        /// <summary>
        ///     Get all components from the application by type.
        /// </summary>
        private static IEnumerable<T> GetAll<T>()
        {
            return Instance._subsystems.OfType<T>();
        }


        /// <summary>
        ///     Query the sub-systems to get all system components
        /// </summary>
        private static IEnumerable<IService> GetAllServices()
        {
            return Instance._subsystems.Select(subsystem => subsystem.GetAllServices())
                .Aggregate((s1, s2) => s1.Concat(s2));
        }


        /// <summary>
        ///     Get any component from the application by type.
        /// </summary>
        private static T Get<T>()
        {
            var instance = Instance._subsystems.OfType<T>().FirstOrDefault();

            if (instance != null)
                return instance;

            instance = GetAllServices().OfType<T>().FirstOrDefault();

            if (instance != null)
                return instance;

            return Instance._settingsContainer.ActiveSettings.OfType<T>().FirstOrDefault();
        }


        /// <summary>
        ///     Gets the injectable value binded to the specified type.
        /// </summary>
        private static object GetInjectionBinding(Type type)
        {
            Instance._injectionMap.TryGetValue(type, out var res);
            return res;
        }

        /// <summary>
        ///     Run the application initialization before the scene is loaded. This handles loading all of the configured
        ///     subsystems.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        [UsedImplicitly]
        private static void Initialize()
        {
#if UNITY_EDITOR
            Application.runInBackground = true;
#else
            Application.runInBackground = false;
#endif

            Profiler.enabled = false;
            Application.targetFrameRate = 30;

            DOTween.Init();

            var appGameObject = new GameObject("App")
            {
                //comment for Photon Messaging
                hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInHierarchy | HideFlags.HideInInspector |
                            HideFlags.NotEditable
            };

            DontDestroyOnLoad(appGameObject);

            Instance = appGameObject.AddComponent<App>();

            Application.backgroundLoadingPriority = ThreadPriority.Low;

            if (SceneManager.GetActiveScene().buildIndex != Scenes.PRELOADER) SceneManager.LoadScene(Scenes.PRELOADER);

            AddSubsystem<AudioSubsystem>();
            AddSubsystem<GameSubsystem>();
            AddSubsystem<NativeSubsystem>();
            AddSubsystem<UtilsSubsystem>();
            AddSubsystem<DisplaySubsystem>();
        }

        /// <summary>
        ///     Inject all the dependencies of the object
        /// </summary>
        private static void InjectDependencies(object injectable)
        {
            InjectFieldDependencies(injectable);
            InjectPropertyDependencies(injectable);
        }

        /// <summary>
        ///     Inject all object field dependencies of the object
        /// </summary>
        private static void InjectFieldDependencies(object injectable)
        {
            Injector.InjectFields(injectable);
        }

        /// <summary>
        ///     Inject all object property dependencies of the object
        /// </summary>
        private static void InjectPropertyDependencies(object injectable)
        {
            Injector.InjectProperties(injectable);
        }

        /// <summary>
        ///     Generates the injections map from all subsystems, services and settings, which have [InjectionAlias] attribute.
        /// </summary>
        private void GenerateInjectionsMap()
        {
            var injectableObjects = new List<object>();

            // Adding all subsystem to the processed objects list
            _subsystems.ForEach(subsystem => injectableObjects.Add(subsystem));

            // Adding all services to the processed objects list
            GetAllServices().ToList().ForEach(service => injectableObjects.Add(service));

            // Adding all settings to the processed objects list
            _settingsContainer.ActiveSettings.ForEach(settings => injectableObjects.Add(settings));

            // Converting objects list to injection map and appending it to the existing one
            InjectionAliasesParser.AppendInjectionMap(injectableObjects, _injectionMap, true);
        }

        /// <summary>
        ///     Runs the startup sequence of the entire system and puts it into a running state.
        /// </summary>
        private IEnumerator InitializeSystem()
        {
            yield return new VerifySubsystemStatus(SubsystemStatus.NotReady, _subsystems);

            var settingsObject = GameObject.Find("Settings");
            if (settingsObject != null)
                _settingsContainer = GameObject.Find("Settings").GetComponent<SettingsContainer>();
            else
                _settingsContainer = Instance.gameObject.AddComponent<SettingsContainer>();

            _settingsContainer.FinalizeSettings();
            GenerateInjectionsMap();
            _subsystems.ForEach(InjectDependencies);
            GetAllServices().ToList().ForEach(InjectDependencies);

            _subsystems.ForEach(systemComponent => systemComponent.Initialize(this));
            yield return new VerifySubsystemStatus(SubsystemStatus.Ready, _subsystems);

            _subsystems.ForEach(systemComponent => systemComponent.StartSubsystem());
            yield return new VerifySubsystemStatus(SubsystemStatus.Started, _subsystems);

            _subsystems.ForEach(systemComponent => systemComponent.Run());
            yield return new VerifySubsystemStatus(SubsystemStatus.Running, _subsystems);

            if (_settingsContainer.GameSettings.loader != null)
            {
                var loader = Instantiate(_settingsContainer.GameSettings.loader);
                var gameLoader = loader.GetComponent<GameLoader>();

                if (gameLoader == null)
                    LogError("Failed to find GameLoader");
                IntegrationSubsystem.Instance.AnalyticsService.RunInstant();

                GameRoot = gameLoader.Load();
            }
            else
            {
                LogError("Failed to find GameLoader");
            }
        }

        /// <summary>
        ///     Called when the player is paused. Usually when application is sent to the background.
        /// </summary>
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                StartCoroutine(PauseSystem());
            else
                StartCoroutine(ResumeSystem());
        }

        /// <summary>
        ///     Puts the system into a paused state
        /// </summary>
        private IEnumerator PauseSystem()
        {
            _subsystems.ForEach(subsystem => subsystem.Pause(true));
            yield break;
        }

        /// <summary>
        ///     Puts the system back into a running state after being paused.
        /// </summary>
        private IEnumerator ResumeSystem()
        {
            _subsystems.ForEach(subsystem => subsystem.Pause(false));
            yield break;
        }

        /// <summary>
        ///     Start is called on the frame when a script is enabled,
        ///     just before any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            StartCoroutine(InitializeSystem());
            DontDestroyOnLoad(gameObject);
        }

        #endregion
    }
}