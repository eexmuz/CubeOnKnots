using System.Collections.Generic;
using Aig.Client.Integration.Runtime.Settings;
using Aig.Client.Integration.Runtime.Subsystem;
using Aig.Client.Integration.Runtime.Utils;
using Core;
using Core.Attributes;
using Core.Services;
using UnityEngine;

namespace Aig.Client.Integration.Runtime.Analytics
{
    public class AppMetricaAdapter : MonoBehaviour, IAnalyticsAdapter
    {
        [Inject]
        private IPlayerDataService _playerDataService;

        public AnalyticType AnalyticType { get; } = AnalyticType.AppMetrica;

        public void Initialize(IntegrationSettings settings)
        {
#if AIG_ANALYTICS_APP_METRICA
            AppMetrica.Instance.SetUserProfileID(SystemInfo.deviceUniqueIdentifier);
#endif // AIG_ANALYTICS_APP_METRICA

            App.Inject(this);
        }

        public void VideoAdsAvailable(string adType, string placement, string result)
        {
#if AIG_ANALYTICS_APP_METRICA
            Dictionary<string, object> param = GenerateDefaultDict();
            param.Add(AnalyticsEvents.AD_TYPE, adType);
            param.Add(AnalyticsEvents.PLACEMENT, placement);
            param.Add(AnalyticsEvents.RESULT, result);
            param.Add(AnalyticsEvents.CONNECTION, InternetStatusService.Instance.IsConnectedToInternet);

            AppMetrica.Instance.ReportEvent(AnalyticsEvents.VIDEO_ADS_AVAILABLE, param);
#endif // AIG_ANALYTICS_APP_METRICA
        }

        public void VideoAdsStared(string adType, string placement, string result)
        {
#if AIG_ANALYTICS_APP_METRICA
            Dictionary<string, object> param = GenerateDefaultDict();
            param.Add(AnalyticsEvents.AD_TYPE, adType);
            param.Add(AnalyticsEvents.PLACEMENT, placement);
            param.Add(AnalyticsEvents.RESULT, result);
            param.Add(AnalyticsEvents.CONNECTION, InternetStatusService.Instance.IsConnectedToInternet);

            AppMetrica.Instance.ReportEvent(AnalyticsEvents.VIDEO_ADS_STARTED, param);
#endif // AIG_ANALYTICS_APP_METRICA
        }

        public void VideoAdsWatch(string adType, string placement, string result)
        {
#if AIG_ANALYTICS_APP_METRICA
            Dictionary<string, object> param = GenerateDefaultDict();
            param.Add(AnalyticsEvents.AD_TYPE, adType);
            param.Add(AnalyticsEvents.PLACEMENT, placement);
            param.Add(AnalyticsEvents.RESULT, result);
            param.Add(AnalyticsEvents.CONNECTION, InternetStatusService.Instance.IsConnectedToInternet);

            AppMetrica.Instance.ReportEvent(AnalyticsEvents.VIDEO_ADS_WATCH, param);
#endif // AIG_ANALYTICS_APP_METRICA
        }

        public void PaymentSucceed(string inAppId, string currency, float price, string inAppType)
        {
#if AIG_ANALYTICS_APP_METRICA
            Dictionary<string, object> param = GenerateDefaultDict();
            param.Add(AnalyticsEvents.INAPP_ID, inAppId);
            param.Add(AnalyticsEvents.CURRENCY, currency);
            param.Add(AnalyticsEvents.PRICE, price);

            AppMetrica.Instance.ReportEvent(AnalyticsEvents.PAYMENT_SUCCEED, param);
#endif // AIG_ANALYTICS_APP_METRICA
        }

        public void RateUs(string showReason, int rateResult)
        {
#if AIG_ANALYTICS_APP_METRICA
            Dictionary<string, object> param = GenerateDefaultDict();
            param.Add(AnalyticsEvents.SHOW_REASON, showReason);
            param.Add(AnalyticsEvents.RATE_RESULT, rateResult);

            AppMetrica.Instance.ReportEvent(AnalyticsEvents.RATE_US, param);
#endif // AIG_ANALYTICS_APP_METRICA
        }

        public void LevelStart(LevelInfo levelInfo)
        {
#if AIG_ANALYTICS_APP_METRICA
            Dictionary<string, object> param = GenerateDefaultDict();
            LevelInfo.ParseAndAddLevelInfo(levelInfo, param);

            AppMetrica.Instance.ReportEvent(AnalyticsEvents.LEVEL_START, param);
            AppMetrica.Instance.SendEventsBuffer();
#endif // AIG_ANALYTICS_APP_METRICA
        }

        public void LevelFinish(LevelInfo levelInfo, string result, int time, int progress, int continueCount)
        {
#if AIG_ANALYTICS_APP_METRICA
            Dictionary<string, object> param = GenerateDefaultDict();
            LevelInfo.ParseAndAddLevelInfo(levelInfo, param);
            param.Add(AnalyticsEvents.RESULT, result);
            param.Add(AnalyticsEvents.TIME, time);
            param.Add(AnalyticsEvents.PROGRESS, progress);
            param.Add(AnalyticsEvents.CONTINUE, continueCount);

            AppMetrica.Instance.ReportEvent(AnalyticsEvents.LEVEL_FINISH, param);
            AppMetrica.Instance.SendEventsBuffer();
#endif // AIG_ANALYTICS_APP_METRICA
        }

        public void StageStart(LevelInfo levelInfo, StageInfo stageInfo)
        {
#if AIG_ANALYTICS_APP_METRICA
            Dictionary<string, object> param = GenerateDefaultDict();
            LevelInfo.ParseAndAddLevelInfo(levelInfo, param);
            StageInfo.ParseAndAddStageInfo(stageInfo, param);
            AppMetrica.Instance.ReportEvent(AnalyticsEvents.STAGE_START, param);
            AppMetrica.Instance.SendEventsBuffer();
#endif // AIG_ANALYTICS_APP_METRICA
        }

        public void StageFinish(LevelInfo levelInfo, StageInfo stageInfo, string result, int time, int progress, int continueCount)
        {
#if AIG_ANALYTICS_APP_METRICA
            Dictionary<string, object> param = GenerateDefaultDict();
            LevelInfo.ParseAndAddLevelInfo(levelInfo, param);
            StageInfo.ParseAndAddStageInfo(stageInfo, param);
            param.Add(AnalyticsEvents.RESULT, result);
            param.Add(AnalyticsEvents.TIME, time);
            param.Add(AnalyticsEvents.PROGRESS, progress);
            param.Add(AnalyticsEvents.CONTINUE, continueCount);
            AppMetrica.Instance.ReportEvent(AnalyticsEvents.STAGE_FINISH, param);
            AppMetrica.Instance.SendEventsBuffer();
#endif // AIG_ANALYTICS_APP_METRICA
        }

        public void Tutorial(string stepName)
        {
#if AIG_ANALYTICS_APP_METRICA
            Dictionary<string, object> param = GenerateDefaultDict();
            param.Add(AnalyticsEvents.STEP_NAME, stepName);

            AppMetrica.Instance.ReportEvent(AnalyticsEvents.TUTORIAL, param);
            AppMetrica.Instance.SendEventsBuffer();
#endif // AIG_ANALYTICS_APP_METRICA
        }

        public void Technical(string stepName, bool firstStart)
        {
#if AIG_ANALYTICS_APP_METRICA
            Dictionary<string, object> param = GenerateDefaultDict();
            param.Add(AnalyticsEvents.STEP_NAME, stepName);
            param.Add(AnalyticsEvents.FIRST_START, firstStart);

            AppMetrica.Instance.ReportEvent(AnalyticsEvents.TECHNICAL, param);
            AppMetrica.Instance.SendEventsBuffer();
#endif // AIG_ANALYTICS_APP_METRICA
        }

        public void LevelUp(int level)
        {
#if AIG_ANALYTICS_APP_METRICA
            Dictionary<string, object> param = GenerateDefaultDict();
            param.Add(AnalyticsEvents.LEVEL, level);

            AppMetrica.Instance.ReportEvent(AnalyticsEvents.LEVEL_UP, param);
#endif // AIG_ANALYTICS_APP_METRICA
        }

        public void Errors(string type, string place, string errorName)
        {
#if AIG_ANALYTICS_APP_METRICA
            Dictionary<string, object> param = GenerateDefaultDict();
            param.Add(AnalyticsEvents.TYPE, type);
            param.Add(AnalyticsEvents.PLACE, place);
            param.Add(AnalyticsEvents.ERROR_NAME, errorName);

            AppMetrica.Instance.ReportEvent(AnalyticsEvents.ERRORS, param);
            AppMetrica.Instance.SendEventsBuffer();
#endif // AIG_ANALYTICS_APP_METRICA
        }

        public void SkinUnlock(string skinType, string skinName, string skinRarity, string unlockType)
        {
#if AIG_ANALYTICS_APP_METRICA
            Dictionary<string, object> param = GenerateDefaultDict();
            param.Add(AnalyticsEvents.SKIN_TYPE, skinType);
            param.Add(AnalyticsEvents.SKIN_NAME, skinName);
            param.Add(AnalyticsEvents.SKIN_RARITY, skinRarity);
            param.Add(AnalyticsEvents.UNLOCK_TYPE, unlockType);

            AppMetrica.Instance.ReportEvent(AnalyticsEvents.SKIN_UNLOCK, param);
#endif // AIG_ANALYTICS_APP_METRICA
        }

        public void AbGroup(string eventName, string abGroupName)
        {
#if AIG_ANALYTICS_APP_METRICA
            Dictionary<string, object> param = GenerateDefaultDict();
            param.Add(AnalyticsEvents.ACTION, eventName);
            param.Add(AnalyticsEvents.AB_GROUP_NAME, abGroupName);

            AppMetrica.Instance.ReportEvent(AnalyticsEvents.AB_GROUP, param);
            AppMetrica.Instance.SendEventsBuffer();
#endif // AIG_ANALYTICS_APP_METRICA
        }

        public void CustomEvent(string eventName, Dictionary<string, object> eventParams, bool sendInstant)
        {
#if AIG_ANALYTICS_APP_METRICA
            Dictionary<string, object> param = GenerateDefaultDict();

            foreach (var eventParam in eventParams)
            {
                if (param.ContainsKey(eventParam.Key))
                {
                    param[eventParam.Key] = eventParam.Value;
                }
                else
                {
                    param.Add(eventParam.Key, eventParam.Value);
                }
            }

            AppMetrica.Instance.ReportEvent(eventName, param);

            if (sendInstant)
            {
                AppMetrica.Instance.SendEventsBuffer();
            }
#endif // AIG_ANALYTICS_APP_METRICA
        }

#if AIG_ANALYTICS_APP_METRICA
        private Dictionary<string, object> GenerateDefaultDict()
        {
            var defaultDictionary = new Dictionary<string, object>()
            {
                //{"level_completed_count", _playerDataService.LevelCompletedCount},
                //{"level_count", _playerDataService.LevelCount}
                //{AnalyticsEvents.AB_GROUP_PARAM, IntegrationSubsystem.Instance.AnalyticsService.AbGroupParam()}
            };

            return defaultDictionary;
        }
#endif // AIG_ANALYTICS_APP_METRICA
    }
}
