using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Aig.Client.Integration.Runtime.Settings
{
     [CreateAssetMenu(
          fileName = nameof(IntegrationSettings),
          menuName = "Azur/Integration/Integration Settings",
          order    = 9999)]
     public class IntegrationSettings : ScriptableObject
     {
          [Space(10)]
          public string googleDocId;

          [Space(10)]
          [Header("General")]
          public bool autoRun = true;

          [Space(10)]
          [Header("Analytics")]
          public bool enableAnalytics;

          [Space(10)]
          [Header("AppMetrica")]
          public bool enableAppMetrica;

          public string appMetricaApiKey;
          public uint appMetricaSessionTimeoutSec = 300;

          [Space(10)]
          [Header("AppsFlyer")]
          [SerializeField]
          private bool enableAppsFlyer;

          public string appsFlyerDevKey;
          public string appsFlyerAppId;
          public int appsFlyerSessionTimeoutSec = 300;

          [Space(10)]
          [Header("Ads")]
          public bool enableAds;
          
          [Space(10)]
          [Header("AppLovin Max")]
          [TextArea]
          public string maxSdkKey;
          [Space(5)]
          public string adMobAndroidAppId;
          public string adMobIosAppId;

          [Space(5)]
          public string androidInterstitialAdUnitId;
          public string androidRewardedAdUnitId;
          public string androidBannerAdUnitId;

          [Space(5)]
          public string iosInterstitialAdUnitId;
          public string iosRewardedAdUnitId;
          public string iosBannerAdUnitId;

          [HideInInspector]
          public string InterstitialAdUnitId;
          [HideInInspector]
          public string RewardedAdUnitId;
          [HideInInspector]
          public string BannerAdUnitId;

          [Space(5)]
          public bool loadInterstitialOnStart = true;
          public bool loadRewardedOnStart = true;
          public bool createBannerOnStart = true;
          public bool autoShowBannerOnStart = true;
          public float autoShowBannerDelay = 20;

          [Space(5)]
          public int interstitialDelayAndroid = 60;
          public int firstInterstitialDelayAndroid = 60;

          [Space(5)]
          public int interstitialDelayIos = 60;
          public int firstInterstitialDelayIos = 60;

          public int interstitialDelay
          {
               get
               {
#if UNITY_ANDROID
                    return interstitialDelayAndroid;
#endif

#if UNITY_IOS
                    return interstitialDelayIos;
#endif
               }
          }

          public int firstInterstitialDelay
          {
               get
               {
#if UNITY_ANDROID
                    return firstInterstitialDelayAndroid;
#endif

#if UNITY_IOS
                    return firstInterstitialDelayIos;
#endif
               }
          }

          public string locationUsageDescription = "Used for best downloading ads";

          //public bool consentFlowEnabled = true;
          //public string consentFlowPrivacyPolicyUrl = "https://privacy.azurgames.com/policy.html#h.hn0lb3lfd0ij";

#if UNITY_EDITOR

          public void ForceValidate()
          {
               OnValidate();
          }

          private void OnValidate()
          {
               var buildTargetGroup = BuildTargetGroup.Unknown;

#if UNITY_ANDROID
               buildTargetGroup = BuildTargetGroup.Android;

               InterstitialAdUnitId = androidInterstitialAdUnitId;
               RewardedAdUnitId = androidRewardedAdUnitId;
               BannerAdUnitId = androidBannerAdUnitId;
#endif // UNITY_ANDROID

#if UNITY_IOS
               buildTargetGroup = BuildTargetGroup.iOS;

               InterstitialAdUnitId = iosInterstitialAdUnitId;
               RewardedAdUnitId = iosRewardedAdUnitId;
               BannerAdUnitId = iosBannerAdUnitId;
#endif // UNITY_IOS

#if UNITY_ANDROID || UNITY_IOS
               SetDefinesForGroup(buildTargetGroup, new List<DefineItem>()
               {
                    new DefineItem("AIG_ANALYTICS", enableAnalytics),
                    new DefineItem("AIG_ANALYTICS_APP_METRICA", enableAnalytics && enableAppMetrica),
                    new DefineItem("AIG_ANALYTICS_APPS_FLYER", enableAnalytics && enableAppsFlyer),
                    new DefineItem("AIG_ADS", enableAds),
                    new DefineItem("AIG_DEV_ANALYTICS", false)
               });
#endif // UNITY_ANDROID || UNITY_IOS

/* commented. no need with AppMetricaExt. but left just-in-case
#if AIG_ANALYTICS_APP_METRICA
               var dirPath = "Assets/AppMetrica";
               var fileName = "AppMetrica.cs";
               var filePath = Path.Combine(dirPath, fileName);
               var contains = "if (LocationTracking) {";
               var comment = "//";
               var marker = "//### Changed";
               var lineEnd = "\n";

               if (Directory.Exists(dirPath) && File.Exists(filePath))
               {
                    var newFile = string.Empty;

                    var reader = new StreamReader(filePath);
                    var needsToWrite = false;

                    while (reader.EndOfStream == false)
                    {
                         var line = reader.ReadLine();
                         if (line.Contains(marker))
                         {
                              break;
                         }

                         if (line.Contains(contains) && line.Contains(comment) == false)
                         {
                              line = comment + line;
                              newFile += line + lineEnd;
                              line = comment + reader.ReadLine();
                              newFile += line + lineEnd;
                              line = comment + reader.ReadLine();
                              newFile += line + lineEnd;

                              needsToWrite = true;
                         }
                         else
                         {
                              newFile += line + lineEnd;
                         }
                    }

                    reader.Close();

                    if (needsToWrite)
                    {
                         newFile = marker + lineEnd + newFile;
                         StreamWriter writer = new StreamWriter(filePath, false);
                         writer.WriteLine(newFile);
                         writer.Close();
                    }
               }
#endif // AIG_ANALYTICS_APP_METRICA
*/

#if AIG_ADS
               AppLovinSettings.Instance.QualityServiceEnabled = true;
               AppLovinSettings.Instance.SdkKey = maxSdkKey;
               AppLovinSettings.Instance.AdMobAndroidAppId = adMobAndroidAppId;
               AppLovinSettings.Instance.AdMobIosAppId = adMobIosAppId;

               AppLovinSettings.Instance.ConsentFlowEnabled = false;
               //AppLovinSettings.Instance.ConsentFlowEnabled = consentFlowEnabled;
               //AppLovinSettings.Instance.ConsentFlowPrivacyPolicyUrl = consentFlowPrivacyPolicyUrl;
#endif // AIG_ADS

               PlayerSettings.iOS.locationUsageDescription = locationUsageDescription;
          }

          private readonly struct DefineItem
          {
               public readonly string Define;
               public readonly bool Enabled;

               public DefineItem(string define, bool enabled)
               {
                    this.Define = define;
                    this.Enabled = enabled;
               }
          }

          private static void SetDefinesForGroup(BuildTargetGroup targetGroup, List<DefineItem> defines)
          {
               var currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
               var updatedDefines = currentDefines;

               foreach (var define in defines)
               {
                    updatedDefines = GetUpdatedDefines(updatedDefines, define.Define, define.Enabled);
               }

               if (updatedDefines != currentDefines)
               {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, updatedDefines);
               }
          }

          private static string GetUpdatedDefines(string defines, string define, bool enabled)
          {
               var definesArray = string.IsNullOrEmpty(defines)
                    ? new List<string>()
                    : new List<string>(defines.Split(';'));

               if (enabled)
               {
                    if (!definesArray.Contains(define))
                    {
                         definesArray.Add(define);
                    }
               }
               else
               {
                    definesArray.Remove(define);
               }

               var updatedDefines = string.Join(";", definesArray.ToArray());

               return updatedDefines;
          }
#endif // UNITY_EDITOR
     }
}
