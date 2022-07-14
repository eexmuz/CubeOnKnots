using System;
using System.Collections;
using Aig.Client.Integration.Runtime.Analytics;
using Aig.Client.Integration.Runtime.Ads;
using Aig.Client.Integration.Runtime.Settings;
using Aig.Client.Integration.Runtime.Utils;
using UnityEngine;

namespace Aig.Client.Integration.Runtime.Subsystem
{
    public class IntegrationSubsystem : MonoBehaviour
    {
        private const string Version = "0.9.0";

        public static IntegrationSubsystem Instance;

        public IntegrationSettings Settings{get; private set;}

        public IAnalyticsService AnalyticsService{get; private set;}

        public IAdsService AdsService{get; private set;}

        public AdsAnalyticsService adsAnalyticsService;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Debug.Log("[INTEGRATION] -> Initialize Integration Subsystem");

            var integrationSubsystemGameObject = new GameObject("IntegrationSubsystem")
            {
                hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable
            };

            DontDestroyOnLoad(integrationSubsystemGameObject);

            Instance = integrationSubsystemGameObject.AddComponent<IntegrationSubsystem>();

            Instance.Launch();
        }


        private void Launch()
        {
            LoadSettings();

            if (Settings.autoRun)
            {
                Run();
            }
        }

        public void Run()
        {
            Debug.Log("[INTEGRATION] -> Run");

            gameObject.AddComponent<InternetStatusService>();

            AnalyticsService = gameObject.AddComponent<AnalyticsService>();
            AnalyticsService.RunService(Settings);
            //AnalyticsService.RunInstant();

            AdsService = gameObject.AddComponent<AdsService>();

            adsAnalyticsService = new AdsAnalyticsService();
            adsAnalyticsService.Init();

            AdsService.RunService(Settings);

#if !AIG_ADS
            AnalyticsService.RunDelayed(1.0f);
#endif
        }

        private void LoadSettings()
        {
            Debug.Log("[INTEGRATION] -> LoadSettings");

            var integrationSettings = Resources.Load<IntegrationSettings>("IntegrationSettings");

            if (integrationSettings == null)
            {
                throw new Exception("Integration Settings not created! Create it in context menu Azur/Integration/Integration Settings and put it to any Resources folder!");
            }
            else
            {
                Settings = integrationSettings;
            }
        }
    }
}
