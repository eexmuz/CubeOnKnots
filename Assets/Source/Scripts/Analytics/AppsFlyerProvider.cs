using Analytics;
using Core;

namespace Analytics
{
    public class AppsFlyerProvider : DIBehaviour, IAnalyticsProvider
    {

        public void initialize()
        {
            /*AppsFlyer.setAppsFlyerKey("eAdQTkWbNVhsXhupjzbLRi");
#if UNITY_IOS
/* Mandatory - set your apple app ID
   NOTE: You should enter the number only and not the "ID" prefix #1#
//AppsFlyer.setAppID ("YOUR_APP_ID_HERE");
//AppsFlyer.trackAppLaunch ();
#elif UNITY_ANDROID
        /* Mandatory - set your Android package name #1#
        AppsFlyer.setAppID(ApplicationSettings.Instance.androidBoudle);
        /* For getting the conversion data in Android, you need to add the "AppsFlyerTrackerCallbacks" listener.#1#
        AppsFlyer.init("eAdQTkWbNVhsXhupjzbLRi");
#endif*/
        }

        public void BuyChestInShop(string chestName)
        {

        }

        public void Revenue(string productId, int count, float cost)
        {
            /*AppsFlyer.trackRichEvent(AFInAppEvents.PURCHASE, new Dictionary<string, string>(){
            {AFInAppEvents.CONTENT_ID, productId},
            {AFInAppEvents.CONTENT_TYPE, "hard_inapp"},
            {AFInAppEvents.REVENUE, cost.ToString()},
            {AFInAppEvents.CURRENCY, "USD"}
        });*/
        }

        public void GetNewItem(string itemName)
        {

        }

        public void UpgradeItem(string itemName, int levelUpgrade)
        {

        }

        public void SetUserFirstLoginDate()
        {

        }

        public void SetUserLastLoginDate()
        {

        }

        public void SetUserLevel(int level)
        {

        }

        public void SetUserExp(int exp)
        {

        }

        public void SetUserRaiting(int raiting)
        {

        }

        public void SetUserRank(int rank)
        {

        }

        public void AddMatchResult(string arena, bool withBot, int rank)
        {

        }

        public void AddMatchEnter(string arena, bool withBot, int rank)
        {

        }

        public void AddMatchFirstSpawn(string arena, bool withBot, int rank, string itemName)
        {

        }

        public void AddVictoryArena(string arena, int side, bool withBot, int rank)
        {

        }

        public void AddSpawnItem(string tankName, int upgradeLevel, int side, bool withBot, int rank)
        {

        }

        public void AddVictoryItem(string tankName, int upgradeLevel, int side, bool withBot, int rank)
        {

        }

        public void SetCoins(int count)
        {

        }

        public void SetGems(int count)
        {

        }

        public void AddCoins(int count, IncomeMoneyReasons incomeMoneyReasons)
        {

        }

        public void AddGems(int count, IncomeMoneyReasons incomeMoneyReasons)
        {

        }

        public void AddSkillPoints(int count, IncomeMoneyReasons incomeMoneyReasons)
        {

        }

        public void SpentCoins(int count, int deltaCount, OutcomeMoneyReasons outcomeMoneyReason)
        {

        }

        public void SpentGems(int count, int deltaCount, OutcomeMoneyReasons outcomeMoneyReason)
        {

        }

        public void SpentSkillPoints(int count, int deltaCount, OutcomeMoneyReasons outcomeMoneyReason)
        {

        }

        public void TutorialStep(string stepName, int stepIndex, int fps)
        {

        }

        public void EnterNameStep()
        {

        }

        public void GameLoader(int step)
        {

        }

        public void GameLoaderMessage(string message)
        {

        }

        public void TutorialFinish()
        {

        }

        public void Matchmaking(int waitTime, MatchMakingResult matchMakingResult)
        {

        }

        public void PlayerBattleFinish()
        {

        }

        public void SendItemInfo()
        {

        }

        public void GetChest(string chestName)
        {

        }

        public void OpenChest(string chestName)
        {
        
        }

        public void GetCards(string tankName, int count, string source)
        {

        }

        public void VideoAdsWatch(string source, string result)
        {

        }

        public void SkillTreeUpgrade(string skillName, string branchName, int level)
        {

        }

        public void Transaction(string productId, string currency, float price)
        {

        }
    }
}
