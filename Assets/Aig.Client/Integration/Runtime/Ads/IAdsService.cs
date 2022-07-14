using System;
using Aig.Client.Integration.Runtime.Settings;

namespace Aig.Client.Integration.Runtime.Ads
{
    public interface IAdsService
    {
        void RunService(IntegrationSettings settings);
        void RunServiceManual();

        bool IsInterstitialVideoReady();
        bool IsRewardVideoReady();
        bool IsPlayingAd { get; }

        bool ShowVideo(bool rewarded, string placement, Action rewardCallback = null,
            bool ignoreTimeout = false);

        void ShowBanner();
        void HideBanner();

        Action OnAdNotReady { get; set; }
        Action<bool> OnMuteGame { get; set; }

        Func<bool> NoAdFunc { get; set; }
        Func<bool> AdShowFunc { get; set; }

        void LoadInterstitial();
        void LoadRewardedAd();
        void CreteBanner();

        Action<string, string, string, int> OnVideoAdsWatch{ get; set; }
        Action<string, string, string> OnVideoAdsStarted{ get; set; }
        Action<string, string, string> OnVideoAdsAvailable{ get; set; }
    }
}