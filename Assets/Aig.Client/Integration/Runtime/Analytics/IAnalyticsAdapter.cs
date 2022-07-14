using System.Collections.Generic;
using Aig.Client.Integration.Runtime.Settings;

namespace Aig.Client.Integration.Runtime.Analytics
{
    interface IAnalyticsAdapter
    {
        AnalyticType AnalyticType { get; }

        void Initialize(IntegrationSettings settings);

        void VideoAdsAvailable(string adType, string placement, string result);
        void VideoAdsStared(string adType, string placement, string result);
        void VideoAdsWatch(string adType, string placement, string result);

        void PaymentSucceed(string inAppId, string currency, float price, string inAppType);

        void RateUs(string showReason, int rateResult);

        void LevelStart(LevelInfo levelInfo);
        void LevelFinish(LevelInfo levelInfo, string result, int time, int progress, int continueCount);

        void StageStart(LevelInfo levelInfo, StageInfo stageInfo);
        void StageFinish(LevelInfo levelInfo, StageInfo stageInfo, string result, int time, int progress, int continueCount);

        void Tutorial(string stepName);

        void Technical(string stepName, bool firstStart);

        void LevelUp(int level);

        void Errors(string type, string place, string errorName);

        void SkinUnlock(string skinType, string skinName, string skinRarity, string unlockType);

        void AbGroup(string eventName, string abGroupName);

        void CustomEvent(string eventName, Dictionary<string, object> eventParams, bool sendInstant);
    }
}
