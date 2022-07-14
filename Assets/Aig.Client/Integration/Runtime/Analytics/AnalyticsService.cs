using System;
using System.Collections;
using System.Collections.Generic;
using Aig.Client.Integration.Runtime.Ads;
using Aig.Client.Integration.Runtime.Settings;
using Aig.Client.Integration.Runtime.Subsystem;
using UnityEngine;

namespace Aig.Client.Integration.Runtime.Analytics
{
    public class AnalyticsService : MonoBehaviour, IAnalyticsService
    {
        private List<IAnalyticsAdapter> _adapters = new List<IAnalyticsAdapter>();
        private IntegrationSettings _settings;

        private Func<string> _abGroupParam;

        // ads
        private string _placement;
        private string _adType;
        private string _result;
        private int _priority;

        private int _levelStartTime = -1;
        private int _stageStartTime = -1;

        private Coroutine _adsCoroutine;

        public Func<string> AbGroupParam
        {
            get => _abGroupParam ?? (() => string.Empty);
            set => _abGroupParam = value;
        }

        public void RunService(IntegrationSettings settings)
        {
#if AIG_ANALYTICS

            Debug.Log("[ANALYTICS] -> Service run");

            _settings = settings;
#endif // AIG_ANALYTICS
        }

        public void RunInstant()
        {
#if AIG_ANALYTICS && AIG_ANALYTICS_APP_METRICA
            Debug.Log("[ANALYTICS] -> Add AppMetrica Launcher");

            var appMetricaGameObject = new GameObject("AppMetricaLauncher");
            appMetricaGameObject.transform.SetParent(transform);
            appMetricaGameObject.SetActive(false);

            var appMetrica = appMetricaGameObject.AddComponent<AppMetricaExt>();
            appMetrica.ApiKey = _settings.appMetricaApiKey;
            appMetrica.SessionTimeoutSec = _settings.appMetricaSessionTimeoutSec;

            appMetricaGameObject.SetActive(true);

            var adapter = gameObject.AddComponent<AppMetricaAdapter>();
            _adapters.Add(adapter);
            adapter.Initialize(_settings);
#endif // AIG_ANALYTICS && AIG_ANALYTICS_APP_METRICA
        }

        public void RunDelayed(float delay)
        {
#if AIG_ANALYTICS && AIG_ANALYTICS_APPS_FLYER
            Debug.Log("[ANALYTICS] -> Add AppsFlyer Launcher");

            StartCoroutine(RunDelayedCoroutine(delay));
#endif // AIG_ANALYTICS && AIG_ANALYTICS_APPS_FLYER
        }

        private IEnumerator RunDelayedCoroutine(float delay)
        {
            if (delay > 0.0f)
            {
                yield return new WaitForSeconds(delay);
            }

#if AIG_ANALYTICS && AIG_ANALYTICS_APPS_FLYER
            Debug.Log("[ANALYTICS] -> Add AppsFlyer Launcher delayed");

            var appsFlyerGameObject = new GameObject("AppsFlyerLauncher");
            appsFlyerGameObject.transform.SetParent(transform);
            appsFlyerGameObject.SetActive(false);

            var appsFlyer = appsFlyerGameObject.AddComponent<AppsFlyerObjectScript>();
            appsFlyer.devKey = _settings.appsFlyerDevKey;
            appsFlyer.appID = _settings.appsFlyerAppId;
            appsFlyer.getConversionData = true;

            appsFlyerGameObject.SetActive(true);

            var adapter = gameObject.AddComponent<AppsFlyerAdapter>();
            _adapters.Add(adapter);
            adapter.Initialize(_settings);
#endif // AIG_ANALYTICS && AIG_ANALYTICS_APPS_FLYER

            yield return null;
        }

        public void VideoAdsAvailable(string adType, string placement, string result,
            AnalyticType analyticType = AnalyticType.All)
        {
            Debug.Log($"[ANALYTICS] -> VideoAdsAvailable - ad_type: {adType}, placement: {placement}, result: {result}, analyticType: {analyticType}");

#if AIG_ANALYTICS && !AIG_DEV_ANALYTICS
            GetAdaptersToSend(analyticType).ForEach(adapter => adapter.VideoAdsAvailable(adType, placement, result));
#endif // AIG_ANALYTICS && !AIG_DEV_ANALYTICS
        }

        public void VideoAdsStarted(string adType, string placement, string result,
            AnalyticType analyticType = AnalyticType.All)
        {
            Debug.Log($"[ANALYTICS] -> VideoAdsStarted - ad_type: {adType}, placement: {placement}, result: {result}, analyticType: {analyticType}");

#if AIG_ANALYTICS && !AIG_DEV_ANALYTICS
            GetAdaptersToSend(analyticType).ForEach(adapter => adapter.VideoAdsStared(adType, placement, result));
#endif // AIG_ANALYTICS && !AIG_DEV_ANALYTICS
        }

        public void VideoAdsWatch(string adType, string placement, string result, int priority,
            AnalyticType analyticType = AnalyticType.All)
        {
            Debug.Log($"[ANALYTICS] -> try VideoAdsWatch - ad_type: {adType}, placement: {placement}, result: {result}, analyticType: {analyticType}");

            if (adType == AdsService.BANNER)
            {
                GetAdaptersToSend(analyticType)
                    .ForEach(adapter => adapter.VideoAdsWatch(_adType, _placement, _result));
                return;
            }

            if (priority <= _priority)
            {
                return;
            }

            _adType = adType;
            _placement = placement;
            _result = result;
            _priority = priority;

            if (_adsCoroutine == null)
            {
                _adsCoroutine = StartCoroutine(VideoAdsWatchWithDelay(analyticType));
            }
        }

        private IEnumerator VideoAdsWatchWithDelay(AnalyticType analyticType = AnalyticType.All)
        {
            yield return new WaitForSeconds(1f);

            Debug.Log($"[ANALYTICS] -> sent VideoAdsWatch - ad_type: {_adType}, placement: {_placement}, result: {_result}, analyticType: {analyticType}");

#if AIG_ANALYTICS && !AIG_DEV_ANALYTICS

            GetAdaptersToSend(analyticType)
                .ForEach(adapter => adapter.VideoAdsWatch(_adType, _placement, _result));

            _priority = 0;
            _adsCoroutine = null;

#endif // AIG_ANALYTICS && !AIG_DEV_ANALYTICS
        }

        public void PaymentSucceed(string inAppId, string currency, float price, string inAppType,
            AnalyticType analyticType = AnalyticType.All)
        {
            Debug.Log($"[ANALYTICS] -> PaymentSucceed - inapp_id: {inAppId}, currency: {currency}, price: {price}, inapp_type: {inAppType}, analyticType: {analyticType}");

#if AIG_ANALYTICS && !AIG_DEV_ANALYTICS
            GetAdaptersToSend(analyticType)
                .ForEach(adapter => adapter.PaymentSucceed(inAppId, currency, price, inAppType));
#endif // AIG_ANALYTICS && !AIG_DEV_ANALYTICS
        }

        public void RateUs(string showReason, int rateResult, AnalyticType analyticType = AnalyticType.All)
        {
            Debug.Log($"[ANALYTICS] -> RateUs - show_reason: {showReason}, rate_result: {rateResult}, analyticType: {analyticType}");

#if AIG_ANALYTICS && !AIG_DEV_ANALYTICS
            GetAdaptersToSend(analyticType).ForEach(adapter => adapter.RateUs(showReason, rateResult));
#endif // AIG_ANALYTICS && !AIG_DEV_ANALYTICS
        }

        public void LevelStart(LevelInfo levelInfo, AnalyticType analyticType = AnalyticType.All)
        {
            _levelStartTime = (int) Time.time;

            Debug.Log($"[ANALYTICS] -> LevelStart - level_info: {levelInfo}, analyticType: {analyticType}");

#if AIG_ANALYTICS && !AIG_DEV_ANALYTICS
            GetAdaptersToSend(analyticType).ForEach(adapter => adapter.LevelStart(levelInfo));
#endif // AIG_ANALYTICS && !AIG_DEV_ANALYTICS
        }

        public void LevelFinish(LevelInfo levelInfo, string result, int progress, int continueCount,
            AnalyticType analyticType = AnalyticType.All)
        {
            var time = _levelStartTime;
            if (_levelStartTime >= 0)
            {
                time = (int) Time.time - _levelStartTime;
                _levelStartTime = -1;
            }

            Debug.Log($"[ANALYTICS] -> LevelFinish - level_info: {levelInfo}, result: {result}, time: {time}, progress: {progress}, continue: {continueCount}, analyticType: {analyticType}");

#if AIG_ANALYTICS && !AIG_DEV_ANALYTICS
            GetAdaptersToSend(analyticType).ForEach(adapter =>
                adapter.LevelFinish(levelInfo, result, time, progress, continueCount));
#endif // AIG_ANALYTICS && !AIG_DEV_ANALYTICS
        }

        public void StageStart(LevelInfo levelInfo, StageInfo stageInfo, AnalyticType analyticType = AnalyticType.All)
        {
            _stageStartTime = (int) Time.time;

            Debug.Log($"[ANALYTICS] -> StageStart - level_info: {levelInfo}, stage_info: {stageInfo}, analyticType: {analyticType}");

#if AIG_ANALYTICS && !AIG_DEV_ANALYTICS
            GetAdaptersToSend(analyticType).ForEach(adapter => adapter.StageStart(levelInfo, stageInfo));
#endif // AIG_ANALYTICS && !AIG_DEV_ANALYTICS
        }

        public void StageFinish(LevelInfo levelInfo, StageInfo stageInfo, string result, int progress,
            int continueCount, AnalyticType analyticType = AnalyticType.All)
        {
            var time = _stageStartTime;
            if (_stageStartTime >= 0)
            {
                time = (int) Time.time - _stageStartTime;
                _stageStartTime = -1;
            }

            Debug.Log($"[ANALYTICS] -> StageFinish - level_info: {levelInfo}, stage_info: {stageInfo}, result: {result}, time: {time}, progress: {progress}, continue: {continueCount}, analyticType: {analyticType}");

#if AIG_ANALYTICS && !AIG_DEV_ANALYTICS
            GetAdaptersToSend(analyticType).ForEach(adapter =>
                adapter.StageFinish(levelInfo, stageInfo, result, time, progress, continueCount));
#endif // AIG_ANALYTICS && !AIG_DEV_ANALYTICS
        }

        public void Tutorial(string stepName, AnalyticType analyticType = AnalyticType.All)
        {
            Debug.Log($"[ANALYTICS] -> Tutorial - step_name: {stepName}, analyticType: {analyticType}");

#if AIG_ANALYTICS && !AIG_DEV_ANALYTICS
            GetAdaptersToSend(analyticType).ForEach(adapter => adapter.Tutorial(stepName));
#endif // AIG_ANALYTICS && !AIG_DEV_ANALYTICS
        }

        public void Technical(string stepName, bool firstStart, AnalyticType analyticType = AnalyticType.All)
        {
            Debug.Log($"[ANALYTICS] -> Technical - step_name: {stepName}, first_start: {firstStart}, analyticType: {analyticType}");

#if AIG_ANALYTICS && !AIG_DEV_ANALYTICS
            GetAdaptersToSend(analyticType).ForEach(adapter => adapter.Technical(stepName, firstStart));
#endif // AIG_ANALYTICS && !AIG_DEV_ANALYTICS
        }

        public void LevelUp(int level, AnalyticType analyticType = AnalyticType.All)
        {
            Debug.Log($"[ANALYTICS] -> LevelUp - level: {level}, analyticType: {analyticType}");

#if AIG_ANALYTICS && !AIG_DEV_ANALYTICS
            GetAdaptersToSend(analyticType).ForEach(adapter => adapter.LevelUp(level));
#endif // AIG_ANALYTICS && !AIG_DEV_ANALYTICS
        }

        public void Errors(string type, string place, string errorName, AnalyticType analyticType = AnalyticType.All)
        {
            Debug.Log($"[ANALYTICS] -> Errors - type: {type}, place: {place}, error_name: {errorName}, analyticType: {analyticType}");

#if AIG_ANALYTICS && !AIG_DEV_ANALYTICS
            GetAdaptersToSend(analyticType).ForEach(adapter => adapter.Errors(type, place, errorName));
#endif // AIG_ANALYTICS && !AIG_DEV_ANALYTICS
        }

        public void SkinUnlock(string skinType, string skinName, string skinRarity, string unlockType,
            AnalyticType analyticType = AnalyticType.All)
        {
            Debug.Log($"[ANALYTICS] -> SkinUnlock - skin_type: {skinType}, skin_name: {skinName}, skin_rarity: {skinRarity}, unlock_type: {unlockType}, analyticType: {analyticType}");

#if AIG_ANALYTICS && !AIG_DEV_ANALYTICS
            GetAdaptersToSend(analyticType)
                .ForEach(adapter => adapter.SkinUnlock(skinType, skinName, skinRarity, unlockType));
#endif // AIG_ANALYTICS && !AIG_DEV_ANALYTICS
        }

        public void AbGroup(string eventName, string abGroupName, AnalyticType analyticType = AnalyticType.All)
        {
            Debug.Log($"[ANALYTICS] -> AbGroup - action: {eventName}, ab_group_name: {abGroupName}, analyticType: {analyticType}");

#if AIG_ANALYTICS && !AIG_DEV_ANALYTICS
            GetAdaptersToSend(analyticType).ForEach(adapter => adapter.AbGroup(eventName, abGroupName));
#endif // AIG_ANALYTICS && !AIG_DEV_ANALYTICS
        }

        public void CustomEvent(string eventName, Dictionary<string, object> eventParams, bool sendInstant = false,
            AnalyticType analyticType = AnalyticType.All)
        {
            Debug.Log($"[ANALYTICS] -> CustomEvent - action: {eventName}, analyticType: {analyticType}");

#if AIG_ANALYTICS && !AIG_DEV_ANALYTICS
            GetAdaptersToSend(analyticType)
                .ForEach(adapter => adapter.CustomEvent(eventName, eventParams, sendInstant));
#endif // AIG_ANALYTICS && !AIG_DEV_ANALYTICS
        }

        private List<IAnalyticsAdapter> GetAdaptersToSend(AnalyticType analyticType)
        {
            var list = new List<IAnalyticsAdapter>();

            foreach (var adapter in _adapters)
            {
                if ((adapter.AnalyticType & analyticType) != AnalyticType.None)
                {
                    list.Add(adapter);
                }
            }

            return list;
        }
    }
}