using System.Collections.Generic;
using Analytics;
using Core.Attributes;
using UnityEngine;

namespace Core.Services
{
    [InjectionAlias(typeof(IAnalyticsService))]
    public class AnalyticsService : Service, IAnalyticsService
    {
        private List<IAnalyticsProvider> _providers = new List<IAnalyticsProvider>();

        private float[] _deltaTimeHistory = new float[100];
        private int _deltaTimeHistoryIndex;

        private void Update()
        {
            UpdateDeltaTimeHistory();
        }

        private void UpdateDeltaTimeHistory()
        {
            _deltaTimeHistoryIndex++;
            if (_deltaTimeHistoryIndex >= _deltaTimeHistory.Length)
            {
                _deltaTimeHistoryIndex = 0;
            }

            _deltaTimeHistory[_deltaTimeHistoryIndex] = Time.deltaTime;
        }

        public int GetCurrentFPS()
        {
            return Mathf.RoundToInt(1f / Time.deltaTime);
        }

        public int GetAverageFPS()
        {
            float sum = 0;
            foreach (float f in _deltaTimeHistory)
            {
                sum += f;
            }

            return Mathf.RoundToInt(_deltaTimeHistory.Length / sum);
        }


        public override void Run()
        {
            base.Run();

            _providers = new List<IAnalyticsProvider>()
            {
                gameObject.AddComponent<AppsFlyerProvider>(),
                gameObject.AddComponent<AmplitudeProvider>(),
                gameObject.AddComponent<YandexMetricaProvider>()
            };

            _providers.ForEach(provider => provider.initialize());

            /*
                Handle to model
            */

        }

        public void BuyChestInShop(string chestName)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.BuyChestInShop(chestName));
#endif
        }

        public void Revenue(string productId, int count, float cost)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.Revenue(productId, count, cost));
#endif
        }

        public void GetNewItem(string itemName)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.GetNewItem(itemName));
#endif
        }

        public void UpgradeItem(string itemName, int levelUpgrade)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.UpgradeItem(itemName, levelUpgrade));
#endif
        }

        public void SetUserFirstLoginDate()
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.SetUserFirstLoginDate());
#endif
        }

        public void SetUserLastLoginDate()
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.SetUserLastLoginDate());
#endif
        }

        public void SetUserLevel(int level)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.SetUserLevel(level));
#endif
        }

        public void SetUserExp(int exp)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.SetUserExp(exp));
#endif
        }

        public void SetUserRaiting(int raiting)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.SetUserRaiting(raiting));
#endif
        }

        public void SetUserRank(int rank)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.SetUserRank(rank));
#endif
        }

        public void AddMatchResult(string arena, bool withBot, int rank)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.AddMatchResult(arena, withBot, rank));
#endif
        }

        public void AddMatchEnter(string arena, bool withBot, int rank)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.AddMatchEnter(arena, withBot, rank));
#endif
        }

        public void AddMatchFirstSpawn(string arena, bool withBot, int rank, string itemName)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.AddMatchFirstSpawn(arena, withBot, rank, itemName));
#endif
        }

        public void AddVictoryArena(string arena, int side, bool withBot, int rank)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.AddVictoryArena(arena, side, withBot, rank));
#endif
        }

        public void AddSpawnItem(string itemName, int upgradeLevel, int side, bool withBot, int rank)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.AddSpawnItem(itemName, upgradeLevel, side, withBot, rank));
#endif
        }

        public void AddVictoryItem(string itemName, int upgradeLevel, int side, bool withBot, int rank)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.AddVictoryItem(itemName, upgradeLevel, side, withBot, rank));
#endif
        }

        public void SetCoins(int count)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.SetCoins(count));
#endif
        }

        public void SetGems(int count)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.SetGems(count));
#endif
        }

        public void AddCoins(int count, IncomeMoneyReasons incomeMoneyReasons)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.AddCoins(count, incomeMoneyReasons));
#endif
        }

        public void AddGems(int count, IncomeMoneyReasons incomeMoneyReasons)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.AddGems(count, incomeMoneyReasons));
#endif
        }

        public void AddSkillPoints(int count, IncomeMoneyReasons incomeMoneyReasons)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.AddSkillPoints(count, incomeMoneyReasons));
#endif
        }

        public void SpentCoins(int count, int deltaCount, OutcomeMoneyReasons outcomeMoneyReason)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.SpentCoins(count, deltaCount, outcomeMoneyReason));
#endif
        }

        public void SpentGems(int count, int deltaCount, OutcomeMoneyReasons outcomeMoneyReason)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.SpentGems(count, deltaCount, outcomeMoneyReason));
#endif
        }

        public void SpentSkillPoints(int count, int deltaCount, OutcomeMoneyReasons outcomeMoneyReason)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.SpentSkillPoints(count, deltaCount, outcomeMoneyReason));
#endif
        }

        public void TutorialStep(string stepName, int stepIndex)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.TutorialStep(stepName, stepIndex, GetAverageFPS()));
#endif
        }

        public void EnterNameStep()
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.EnterNameStep());
#endif
        }

        public void GameLoader(int step)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.GameLoader(step));
#endif
        }

        public void GameLoaderMessage(string message)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.GameLoaderMessage(message));
#endif
        }

        public void TutorialFinish()
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.TutorialFinish());
#endif
        }

        public void Matchmaking(int waitTime, MatchMakingResult matchMakingResult)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.Matchmaking(waitTime, matchMakingResult));
#endif
        }

        public void PlayerBattleFinish()
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.PlayerBattleFinish());
            //AnalyticState.GameAnalyticState.finished = true;
#endif
        }

        public void SendItemInfo()
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            /*
            if (string.IsNullOrEmpty(AnalyticState.ItemAnalyticState.itemName))
            {
                return;
            }
            */
            _providers.ForEach(provider => provider.SendItemInfo());
            //AnalyticState.ItemAnalyticState.Clear();
#endif
        }

        public void GetChest(string chestName)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.GetChest(chestName));
#endif
        }

        public void OpenChest(string chestName)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.OpenChest(chestName));
#endif
        }

        public void GetCards(string itemName, int count, string source)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.GetCards(itemName, count, source));
#endif
        }

        public void VideoAdsWatch(string source, string result)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.VideoAdsWatch(source, result));
#endif
        }

        public void SkillTreeUpgrade(string skillName, string branchName, int level)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.SkillTreeUpgrade(skillName, branchName, level));
#endif
        }

        public void Transaction(string productId, string currency, float price)
        {
#if !POLYPLAY_DEBUG && !UNITY_EDITOR
            _providers.ForEach(provider => provider.Transaction(productId, currency, price));
#endif
        }
    }
}
