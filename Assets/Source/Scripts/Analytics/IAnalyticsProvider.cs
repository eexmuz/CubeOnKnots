namespace Analytics
{
    interface IAnalyticsProvider
    {
        void initialize();

        void BuyChestInShop(string chestName);
        void Revenue(string productId, int count, float cost);
        void GetNewItem(string itemName); //
        void UpgradeItem(string itemName, int levelUpgrade);
        void SetUserFirstLoginDate();
        void SetUserLastLoginDate(); //
        void SetUserLevel(int level);
        void SetUserExp(int exp); //
        void SetUserRaiting(int raiting); //
        void SetUserRank(int rank);
        void AddMatchResult(string arena, bool withBot, int rank);
        void AddMatchEnter(string arena, bool withBot, int rank);
        void AddMatchFirstSpawn(string arena, bool withBot, int rank, string itemName);
        void AddVictoryArena(string arena, int side, bool withBot, int rank); //
        void AddSpawnItem(string tankName, int upgradeLevel, int side, bool withBot, int rank); //
        void AddVictoryItem(string tankName, int upgradeLevel, int side, bool withBot, int rank); //
        void SetCoins(int count); //
        void SetGems(int count); //
        void AddCoins(int count, IncomeMoneyReasons incomeMoneyReasons);
        void AddGems(int count, IncomeMoneyReasons incomeMoneyReasons);
        void AddSkillPoints(int count, IncomeMoneyReasons incomeMoneyReasons);
        void SpentCoins(int count, int deltaCount, OutcomeMoneyReasons outcomeMoneyReason); 
        void SpentGems(int count, int deltaCount, OutcomeMoneyReasons outcomeMoneyReason);
        void SpentSkillPoints(int count, int deltaCount, OutcomeMoneyReasons outcomeMoneyReason);
        void TutorialStep(string stepName, int stepIndex, int fps);
        void EnterNameStep();
        void GameLoader(int step);
        void GameLoaderMessage(string message);
        void TutorialFinish();
        void Matchmaking(int waitTime, MatchMakingResult matchMakingResult);
        void PlayerBattleFinish();
        void SendItemInfo();
        void GetChest(string chestName);
        void OpenChest(string chestName);
        void GetCards(string tankName, int count, string source);
        void VideoAdsWatch(string source, string result);
        void SkillTreeUpgrade(string skillName, string branchName, int level);
        void Transaction(string productId, string currency, float price);

        /*
        void pause(bool paused);

        void quit();

        void postInstall();

        void postFacebookConnect();

        void postLogin();

        void postLogout();

        void postSocialEvent(string type, Dictionary<string, string> postParams);

        void postRating(int rating, int bonus);

        void postInboxRewardCollected(int reward, string reason);

        void postLevelUp(int level, long xp);

        void postBonusCollected(string type, int bonusAmount, string info);

        void postTournamentWin(int place, int award);

        void postWager(long balance, int wagerAmount, int winAmount);

        void postUIEvent(string name, string data);

        void postMessageError(string endpoint, string error);

        void postBillingNotSupported(string message, string store);

        void postCreditsPurchasedStart();

        void postCreditsPurchasedSuccess(string purchaseOrigin);

        void postCreditsPurchasedFail(string msg);

        void postCreditsPurchasedCanceled(string platform, string msg);

        void postCreditsPurchasedEnd();

        void postDownloadGame(string gameName);

        void postLoadGame(string source, string gameName);

        void postInsufficuentFunds();

        void postStoreOpen(string location);

        void postError(string message);

        void postAttribution(string data);*/
    }
}
