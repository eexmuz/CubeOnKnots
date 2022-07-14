using System;
using System.Collections;
using Aig.Client.Integration.Runtime.Settings;
using Aig.Client.Integration.Runtime.Subsystem;
using Aig.Client.Integration.Runtime.Utils;
using Aig.Client.Ios.Idfa;
using UnityEngine;

namespace Aig.Client.Integration.Runtime.Ads
{
    public class AdsService : MonoBehaviour, IAdsService
    {
        public const string REWARDED = "rewarded";
        public const string INTERSTITIAL = "interstitial";
        public const string BANNER = "banner";

        public const string CLICKED = "clicked";
        public const string WATCHED = "watched";
        public const string CANCELED = "canceled";
        public const string SUCCESS = "success";
        public const string START = "start";
        public const string NOT_AVAILABLE = "not_available";

        public const string GAME = "game";

        private IntegrationSettings _settings;

        private float _lastInterstitialTime;

        private string _placement;

        private Action _rewardCallback;
        private bool _isRewardReceived;

        public Action<bool> OnMuteGame { get; set; }
        public Action OnAdNotReady { get; set; }

        private Func<bool> _noAdFunc;
        public Func<bool> NoAdFunc
        {
            get => _noAdFunc ?? (() => false);
            set => _noAdFunc = value;
        }

        private Func<bool> _adShowFunc;
        public Func<bool> AdShowFunc
        {
            get => _adShowFunc ?? (() => true);
            set => _adShowFunc = value;
        }

        public Action<string, string, string, int> OnVideoAdsWatch { get; set; }
        public Action<string, string, string> OnVideoAdsStarted { get; set; }
        public Action<string, string, string> OnVideoAdsAvailable { get; set; }
        public bool IsPlayingAd => _videoPlaying;

        public void RunService(IntegrationSettings settings)
        {
#if AIG_ADS
            Debug.Log("[ADS] -> Service run");

            _settings = settings;

            _lastInterstitialTime = Time.time - _settings.interstitialDelay;
            _lastRewardedTime = _lastInterstitialTime;
#endif // AIG_ADS
        }

        public void RunServiceManual()
        {
#if AIG_ADS

#if UNITY_IOS && !UNITY_EDITOR
            AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(true);
#endif // UNITY_IOS && !UNITY_EDITOR

            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            {
                Debug.Log("[ADS] -> OnSdkInitializedEvent");

                StartCoroutine(Initialize());
            };

            AppTrackingTransparency.RequestTrackingAuthorization(status =>
            {
                StartCoroutine(InitAnalyticsAndAdsWithDelay(status));
            });
#endif // AIG_ADS
        }

        private IEnumerator InitAnalyticsAndAdsWithDelay(TrackingAuthorizationStatus status)
        {
            yield return new WaitForSeconds(1.0f);

            IntegrationSubsystem.Instance.AnalyticsService.RunDelayed(0.0f);

            MaxSdk.SetSdkKey(_settings.maxSdkKey);

#if UNITY_IOS && !UNITY_EDITOR
            MaxSdk.SetHasUserConsent(status == TrackingAuthorizationStatus.Authorized);
#endif // UNITY_IOS && !UNITY_EDITOR


#if UNITY_ANDROID && !UNITY_EDITOR
            //if no GDPR on Android - no need set value
            MaxSdk.SetHasUserConsent(true);
#endif // UNITY_ANDROID && !UNITY_EDITOR

            MaxSdk.InitializeSdk();
        }

        private IEnumerator Initialize()
        {
#if AIG_ADS
            InitInterstitial();

            if (_settings.loadInterstitialOnStart)
            {
                LoadInterstitial();
#if !UNITY_EDITOR
                yield return new WaitUntil(IsInterstitialVideoReady);
#endif // !UNITY_EDITOR
            }

            InitRewarded();

            if (_settings.loadRewardedOnStart)
            {
                LoadRewardedAd();
#if !UNITY_EDITOR
                yield return new WaitUntil(IsRewardVideoReady);
#endif // !UNITY_EDITOR
            }

            InitBanner();

            if (_settings.createBannerOnStart)
            {
                CreteBanner();
            }
#endif // AIG_ADS
            yield return null;
        }

        private void InitInterstitial()
        {
#if AIG_ADS
            MaxSdkCallbacks.OnInterstitialDisplayedEvent += OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.OnInterstitialLoadFailedEvent += OnInterstitialLoadFailedEvent;
            MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += OnInterstitialAdFailedToDisplayEvent;
            MaxSdkCallbacks.OnInterstitialHiddenEvent += OnInterstitialHiddenEvent;
            MaxSdkCallbacks.OnInterstitialClickedEvent += OnInterstitialClickedEvent;
#endif // AIG_ADS
        }

        private void InitRewarded()
        {
#if AIG_ADS
            // Attach callback
            MaxSdkCallbacks.OnRewardedAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.OnRewardedAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.OnRewardedAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.OnRewardedAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.OnRewardedAdHiddenEvent += OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
#endif // AIG_ADS
        }

        private void InitBanner()
        {
#if AIG_ADS
            MaxSdkCallbacks.OnBannerAdLoadedEvent += OnBannerAdLoadedEvent;
            MaxSdkCallbacks.OnBannerAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
#endif // AIG_ADS
        }

#region Interstitial

public bool IsInterstitialVideoReady()
        {
#if AIG_ADS
            return MaxSdk.IsInterstitialReady(_settings.InterstitialAdUnitId);
#endif // AIG_ADS
            return false;
        }
        private void OnInterstitialDisplayedEvent(string adUnitId)
        {
#if AIG_ADS
            Debug.Log("[ADS] -> OnInterstitialDisplayedEvent");

            _lastInterstitialTime = Time.time;
            //MaxSdk.SetMuted(false);
            //OnMuteGame?.Invoke(false);
#endif // AIG_ADS
        }

        private void OnInterstitialLoadedEvent(string adUnitId)
        {
#if AIG_ADS
            // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
            Debug.Log("[ADS] -> OnInterstitialLoadedEvent");
#endif // AIG_ADS
        }

        private void OnInterstitialLoadFailedEvent(string adUnitId, int errorCode)
        {
#if AIG_ADS
            Debug.Log("[ADS] -> Interstitial Ad Load Failed: " + errorCode);

            // Interstitial ad failed to load. We recommend re-trying in 3 seconds.
            Invoke(nameof(LoadInterstitial), 3);
#endif // AIG_ADS
        }

        private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, int errorCode)
        {
#if AIG_ADS
            Debug.Log("[ADS] -> OnInterstitialAdFailedToDisplayEvent");

            // Interstitial ad failed to display. We recommend loading the next ad
            VideoPlayingStateChanged(false);
            LoadInterstitial();
#endif // AIG_ADS
        }

        private void OnInterstitialHiddenEvent(string adUnitId)
        {
#if AIG_ADS
            Debug.Log("[ADS] -> OnInterstitialHiddenEvent");

            OnVideoAdsWatch?.Invoke(INTERSTITIAL, _placement, WATCHED, 1);
            VideoPlayingStateChanged(false);

            // Interstitial ad is hidden. Pre-load the next ad
            LoadInterstitial();
            _lastInterstitialTime = Time.time;
            MaxSdk.SetMuted(false);
            OnMuteGame?.Invoke(false);
#endif // AIG_ADS
        }

        private void OnInterstitialClickedEvent(string adUnitId)
        {
#if AIG_ADS
            Debug.Log("[ADS] -> OnInterstitialClickEvent");

            OnVideoAdsWatch?.Invoke(INTERSTITIAL, _placement, CLICKED, 3);

            // Interstitial ad is hidden. Pre-load the next ad
            LoadInterstitial();
            _lastInterstitialTime = Time.time;
            MaxSdk.SetMuted(false);
            OnMuteGame?.Invoke(false);
#endif // AIG_ADS
        }

        public void LoadInterstitial()
        {
#if AIG_ADS
            MaxSdk.LoadInterstitial(_settings.InterstitialAdUnitId);
#endif // AIG_ADS
        }

#endregion Interstitial

#region Rewarded

        public void LoadRewardedAd()
        {
#if AIG_ADS
            MaxSdk.LoadRewardedAd(_settings.RewardedAdUnitId);
#endif // AIG_ADS
        }

        public bool IsRewardVideoReady()
        {
#if AIG_ADS
    return MaxSdk.IsRewardedAdReady(_settings.RewardedAdUnitId);
#endif // AIG_ADS
    return false;
        }

        private void OnRewardedAdLoadedEvent(string adUnitId)
        {
#if AIG_ADS
            // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
#endif // AIG_ADS
        }

        private void OnRewardedAdLoadFailedEvent(string adUnitId, int errorCode)
        {
#if AIG_ADS
            // Rewarded ad failed to load. We recommend re-trying in 3 seconds.
            Debug.Log("[ADS] -> Rewarded Ad Load Failed: " + errorCode);
            Invoke(nameof(LoadRewardedAd), 3);
#endif // AIG_ADS
        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, int errorCode)
        {
#if AIG_ADS
            // Rewarded ad failed to display. We recommend loading the next ad
            VideoPlayingStateChanged(false);
            LoadRewardedAd();
#endif // AIG_ADS
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId)
        {
#if AIG_ADS
            //IntegrationSubsystem.Instance.AnalyticsService.VideoAdsWatch(REWARDED, _placement, WATCHED, _levelInfo, 1);
            _lastRewardedTime = Time.time;
#endif // AIG_ADS
        }

        private void OnRewardedAdClickedEvent(string adUnitId)
        {
#if AIG_ADS
            OnVideoAdsWatch?.Invoke(REWARDED, _placement, CLICKED, 3);

            _lastRewardedTime = Time.time;
            MaxSdk.SetMuted(false);
            OnMuteGame?.Invoke(false);
#endif // AIG_ADS
        }

        private void OnRewardedAdHiddenEvent(string adUnitId)
        {
#if AIG_ADS
            OnVideoAdsWatch?.Invoke(REWARDED, _placement, _isRewardReceived ? WATCHED : CANCELED, 1);
            _isRewardReceived = false;
            VideoPlayingStateChanged(false);

            LoadRewardedAd();
            _lastRewardedTime = Time.time;
            MaxSdk.SetMuted(false);
            OnMuteGame?.Invoke(false);
#endif // AIG_ADS
        }

#if AIG_ADS
        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward)
        {
            _isRewardReceived = true;
            _rewardCallback?.Invoke();
            _lastRewardedTime = Time.time;
        }
#endif // AIG_ADS

#endregion Rewarded

#region Banner

        private void OnBannerAdLoadedEvent(string adUnitId)
        {
#if AIG_ADS
            if (_settings.autoShowBannerOnStart)
            {
                Invoke(nameof(ShowBanner), _settings.autoShowBannerDelay);
            }
#endif // AIG_ADS
        }

        private void OnBannerAdLoadFailedEvent(string adUnitId, int errorCode)
        {
#if AIG_ADS
            Debug.Log("[ADS] -> Banner Ad Load Failed: " + errorCode);

            Invoke(nameof(CreteBanner), 3);
#endif // AIG_ADS
        }

        public void CreteBanner()
        {
#if AIG_ADS
            MaxSdk.CreateBanner(_settings.BannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
#endif // AIG_ADS
        }

#endregion Banner

        private float _lastRewardedTime;
        private bool _videoPlaying;
        public bool ShowVideo(bool rewarded, string placement, Action rewardCallback = null, bool ignoreTimeout = false)
        {
            if (_videoPlaying)
            {
                return true;
            }

            _rewardCallback = rewardCallback;
            _placement = placement;

#if AIG_ADS
            Debug.Log("[ADS] -> Try show video ad: placement:" + _placement + " | rewarded: " + rewarded);

            if (rewarded)
            {
                if (MaxSdk.IsRewardedAdReady(_settings.RewardedAdUnitId) && InternetStatusService.Instance.IsConnectedToInternet)
                {
                    OnVideoAdsAvailable?.Invoke(REWARDED, _placement, SUCCESS);
                    OnVideoAdsStarted?.Invoke(REWARDED, _placement, START);

                    MaxSdk.SetMuted(true);
                    OnMuteGame?.Invoke(true);

                    MaxSdk.ShowRewardedAd(_settings.RewardedAdUnitId);

                    VideoPlayingStateChanged(true);
                    _lastRewardedTime = Time.time;

                    return true;
                }

                OnAdNotReady?.Invoke();

                Debug.Log("[ADS] -> Fail: REWARDED IS NOT READY");

                OnVideoAdsAvailable?.Invoke(REWARDED, _placement, NOT_AVAILABLE);

                return false;
            }

            if (NoAdFunc())
            {
                Debug.Log("[ADS] -> Fail: NO AD");
                _lastInterstitialTime = Time.time;

                return true;
            }

            bool isCooldownOver = _lastRewardedTime > _lastInterstitialTime
                ? (Time.time - _lastRewardedTime >= 10f)
                : (Time.time - _lastInterstitialTime >= _settings.interstitialDelay);
            
            if ((isCooldownOver || _lastInterstitialTime <= 0f || ignoreTimeout) 
                && Time.time >= _settings.firstInterstitialDelay && AdShowFunc())
            {
                if (MaxSdk.IsInterstitialReady(_settings.InterstitialAdUnitId))
                {
                    OnVideoAdsAvailable?.Invoke(INTERSTITIAL, _placement, SUCCESS);
                    OnVideoAdsStarted?.Invoke(INTERSTITIAL, _placement, START);

                    VideoPlayingStateChanged(true);
                    _lastInterstitialTime = Time.time;

                    MaxSdk.SetMuted(true);
                    OnMuteGame?.Invoke(true);

                    MaxSdk.ShowInterstitial(_settings.InterstitialAdUnitId);

                    return true;
                }

                OnAdNotReady?.Invoke();

                Debug.Log("[ADS] -> Fail: INTERSTITIAL IS NOT READY");

                OnVideoAdsAvailable?.Invoke(INTERSTITIAL, _placement, NOT_AVAILABLE);

                return false;
            }

            Debug.Log($"[ADS] -> Fail: INTERSTITIAL CAN'T BE SHOWN: \n" + 
                      $"TimeOut: {Time.time - _lastInterstitialTime >= _settings.interstitialDelay} \n" + 
                      $"Ignore timeout: {ignoreTimeout} \n" +
                      $"Timeout <= 0 : {_lastInterstitialTime <= 0f} \n" +
                      $"Ad Show: {AdShowFunc()} \n" +
                      $"First Interstitial: {Time.time >= _settings.firstInterstitialDelay}");

            return false;

#endif // AIG_ADS

            rewardCallback?.Invoke();

            return true;
        }

        public void ShowBanner()
        {
#if AIG_ADS
            if (NoAdFunc())
            {
                return;
            }

            Debug.Log("[ADS] -> Show Banner");

            //OnVideoAdsWatch?.Invoke(BANNER, GAME, WATCHED, 3);

            MaxSdk.ShowBanner(_settings.BannerAdUnitId);
#endif
        }

        public void HideBanner()
        {
#if AIG_ADS
            Debug.Log("[ADS] -> Hide Banner");

            MaxSdk.HideBanner(_settings.BannerAdUnitId);
#endif
        }

        private void VideoPlayingStateChanged(bool state)
        {
            _videoPlaying = state;
        }
    }
}
