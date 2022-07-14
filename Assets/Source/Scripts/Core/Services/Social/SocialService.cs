using Core.Attributes;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

namespace Core.Services
{
    [InjectionAlias(typeof(ISocialService))]
    public class SocialService : Service, ISocialService
    {
#if UNITY_IOS
        private static string TABLE_ALL_TIME = "highscore_ios_all_time";
#elif UNITY_ANDROID
        private static string TABLE_ALL_TIME = "CgkIuoDA-cofEAIQAA";
#endif

        private int _scoreToReport;
        private bool _active;
        private bool _needToShowLeaderboard;
        private bool _needToPostScore;

        public void PostScore(int score)
        {
            Debug.Log("Social Service: Going to post score " + score);
            _scoreToReport = score;
            _needToPostScore = true;
            
            if (_active)
            {
                Social.ReportScore(_scoreToReport, TABLE_ALL_TIME, (bool success) =>
                {
                    Debug.Log("Social Service: Post score result; Success " + success);
                    if (success)
                    {
                        FinalizeScorePosting();
                    }
                });
            }
        }

        public void ShowLeaderBoard()
        {
            Debug.Log("Social Service: Trying to show leaderboard; active = " + _active);
            if (_active && Social.localUser.authenticated)
            {
                _needToShowLeaderboard = false;
                Social.ShowLeaderboardUI();
            }
            else
            {
                _needToShowLeaderboard = true;
                InternalInitialization();
            }
        }

        private void InternalInitialization()
        {
#if UNITY_IOS
            Social.localUser.Authenticate(OnSocialAuth);
#endif

#if UNITY_ANDROID
            PlayGamesPlatform.Activate();
            PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (result) =>
            {
                Debug.Log("Social Service: GPG Auth result " + result);
                if (result == SignInStatus.Success)
                {
                    ActivateSocial();
                }
            });
#endif
        }

        private void FinalizeScorePosting()
        {
            Debug.Log("Social Service: FinalizeScorePosting");
            _needToPostScore = false;

            if (_needToShowLeaderboard)
            {
                _needToShowLeaderboard = false;
                ShowLeaderBoard();
            }
        }
        
        private void OnSocialAuth(bool success)
        {
            if (success)
            {
                ActivateSocial();
            }
        }
        
        private void ActivateSocial()
        {
            Debug.Log("Social Service: Activating Social Service...");
            _active = true;
            if (_needToPostScore)
            {
                PostScore(_scoreToReport);
            }
            else if (_needToShowLeaderboard)
            {
                ShowLeaderBoard();
            }
        }
    }
}
