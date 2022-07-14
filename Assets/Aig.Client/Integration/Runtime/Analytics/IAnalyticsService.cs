using System;
using System.Collections.Generic;
using Aig.Client.Integration.Runtime.Settings;

namespace Aig.Client.Integration.Runtime.Analytics
{
    public enum AnalyticType
    {
        None       = 0x0000,
        AppMetrica = 0x0001,
        AppsFlyer  = 0x0002,
        All = AppMetrica | AppsFlyer
    }

    public interface IAnalyticsService
    {
        void RunService(IntegrationSettings settings);

        void RunInstant();

        void RunDelayed(float delay);

        void VideoAdsAvailable(string adType, string placement, string result, AnalyticType analyticType = AnalyticType.All);
        void VideoAdsStarted(string adType, string placement, string result, AnalyticType analyticType = AnalyticType.All);
        void VideoAdsWatch(string adType, string placement, string result, int priority, AnalyticType analyticType = AnalyticType.All);

        void PaymentSucceed(string inAppId, string currency, float price, string inAppType, AnalyticType analyticType = AnalyticType.All);

        void RateUs(string showReason, int rateResult, AnalyticType analyticType = AnalyticType.All);

        void LevelStart(LevelInfo levelInfo, AnalyticType analyticType = AnalyticType.All);
        void LevelFinish(LevelInfo levelInfo, string result, int progress, int continueCount, AnalyticType analyticType = AnalyticType.All);

        void StageStart(LevelInfo levelInfo, StageInfo stageInfo, AnalyticType analyticType = AnalyticType.All);
        void StageFinish(LevelInfo levelInfo, StageInfo stageInfo, string result, int progress, int continueCount, AnalyticType analyticType = AnalyticType.All);

        void Tutorial(string stepName, AnalyticType analyticType = AnalyticType.All);

        void Technical(string stepName, bool firstStart, AnalyticType analyticType = AnalyticType.All);

        void LevelUp(int level, AnalyticType analyticType = AnalyticType.All);

        void Errors(string type, string place, string errorName, AnalyticType analyticType = AnalyticType.All);

        void SkinUnlock(string skinType, string skinName, string skinRarity, string unlockType, AnalyticType analyticType = AnalyticType.All);

        void AbGroup(string eventName, string abGroupName, AnalyticType analyticType = AnalyticType.All);

        void CustomEvent(string eventName, Dictionary<string, object> eventParams, bool sendInstant, AnalyticType analyticType = AnalyticType.All);

        Func<string> AbGroupParam { get; set; }
    }
}