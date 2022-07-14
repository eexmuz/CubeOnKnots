using Analytics;

namespace Core.Services
{
    public interface IAnalyticsService : IService
    {
        void BuyChestInShop(string chestName); // +
        void Revenue(string productId, int count, float cost); // +
        void GetNewItem(string itemName); // +
        void UpgradeItem(string itemName, int levelUpgrade); // +
        void SetUserFirstLoginDate(); // +
        void SetUserLastLoginDate(); // +
        void SetUserLevel(int level); // +
        void SetUserExp(int exp); // +
        void SetUserRaiting(int raiting); // +
        void SetUserRank(int rank); // +
        void AddMatchResult(string arena, bool withBot, int rank); // +
        void AddMatchEnter(string arena, bool withBot, int rank); // +
        void AddMatchFirstSpawn(string arena, bool withBot, int rank, string itemName); // +
        void AddVictoryArena(string arena, int side, bool withBot, int rank); // +
        void AddSpawnItem(string itemName, int upgradeLevel, int side, bool withBot, int rank); // +
        void AddVictoryItem(string itemName, int upgradeLevel, int side, bool withBot, int rank); // +
        void SetCoins(int count); // +
        void SetGems(int count); // +
        void AddCoins(int count, IncomeMoneyReasons incomeMoneyReasons);
        void AddGems(int count, IncomeMoneyReasons incomeMoneyReasons);
        void AddSkillPoints(int count, IncomeMoneyReasons incomeMoneyReasons);
        void SpentCoins(int count, int deltaCount, OutcomeMoneyReasons outcomeMoneyReason); // +
        void SpentGems(int count, int deltaCount, OutcomeMoneyReasons outcomeMoneyReason); // +
        void SpentSkillPoints(int count, int deltaCount, OutcomeMoneyReasons outcomeMoneyReason);
        void TutorialStep(string stepName, int stepIndex);
        void EnterNameStep();
        void GameLoader(int step);
        void GameLoaderMessage(string message);
        void TutorialFinish();
        void Matchmaking(int waitTime, MatchMakingResult matchMakingResult);
        void PlayerBattleFinish();
        void SendItemInfo();
        void GetChest(string chestName);
        void OpenChest(string chestName);
        void GetCards(string itemName, int count, string source);
        void VideoAdsWatch(string source, string result);
        void SkillTreeUpgrade(string skillName, string branchName, int level);
        void Transaction(string productId, string currency, float price);
    }
}