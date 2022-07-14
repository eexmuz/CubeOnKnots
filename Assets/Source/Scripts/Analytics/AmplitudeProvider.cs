using Core;

namespace Analytics
{
    public class AmplitudeProvider : DIBehaviour, IAnalyticsProvider
    {
        //private Amplitude _amplitude;

        public void initialize()
        {
            /*_amplitude = Amplitude.Instance;
            _amplitude.logging = true;
            _amplitude.trackSessionEvents(true);
            _amplitude.init("c1261643f7e3acad3189047e43bc3232");*/
        }

        public void BuyChestInShop(string chestName)
        {
            /*Dictionary<string, object> options = new Dictionary<string, object>() {
                {"Chest" , chestName }
            };

            _amplitude.logEvent("buy_chest_in_shop", options);
            _amplitude.addUserProperty("buy_chest_in_shop_count", 1);*/
        }

        public void Revenue(string productId, int count, float cost)
        {
            /*_amplitude.logRevenue(productId, count, cost);
            _amplitude.addUserProperty("user_product_" + productId, count);

            Dictionary<string, object> options = new Dictionary<string, object>() {
                {"ProductId" , productId }
            };

            _amplitude.logEvent("inapp_product", options);*/
        }

        public void GetNewItem(string itemName)
        {
//        Dictionary<string, object> options = new Dictionary<string, object>() {
//            {"Tank" , tankName }
//        };
//
//        _amplitude.logEvent("get_new_tank", options);
//        _amplitude.addUserProperty("user_tank_count", 1);
//
//        List<string> list = new List<string>();
//        list.Add(tankName);
//        _amplitude.appendUserProperty("user_tank_list", list);
        }

        public void UpgradeItem(string itemName, int levelUpgrade)
        {
            /*Dictionary<string, object> options = new Dictionary<string, object>() {
                {"Tank" , tankName },
                {"Upgrade" , levelUpgrade }
            };

            _amplitude.logEvent("upgrade_tank", options);
            _amplitude.addUserProperty("user_upgrade_tank_count", 1);

            List<string> list = new List<string>();
            list.Add(tankName + "Up" + levelUpgrade);
            _amplitude.appendUserProperty("user_upgrade_list", list);*/
        }

        public void SetUserFirstLoginDate()
        {
            /*_amplitude.setOnceUserProperty("first_login_date", DateTime.Today.ToString(CultureInfo.InvariantCulture));

            Dictionary<string, object> options = new Dictionary<string, object>() {
                {"Date" , DateTime.UtcNow.ToShortDateString() }
            };

            _amplitude.logEvent("first_login_date", options);*/
        }

        public void SetUserLastLoginDate()
        {
//        _amplitude.setUserProperty("last_login_date", DateTime.Today.ToString(CultureInfo.InvariantCulture));
//
//        Dictionary<string, object> options = new Dictionary<string, object>() {
//            {"Date" , DateTime.UtcNow.ToShortDateString() }
//        };
//
//        _amplitude.logEvent("last_login_date", options);
        }

        public void SetUserLevel(int level)
        {
            /*_amplitude.setUserProperty("user_level", level);

            Dictionary<string, object> options = new Dictionary<string, object>() {
                {"Level" , level}
            };

            _amplitude.logEvent("user_level", options);*/
        }

        public void SetUserExp(int exp)
        {
//        _amplitude.setUserProperty("user_exp", exp);
//
//        Dictionary<string, object> options = new Dictionary<string, object>() {
//            {"Exp" , exp}
//        };
//
//        _amplitude.logEvent("user_exp", options);
        }

        public void SetUserRaiting(int raiting)
        {
//        _amplitude.setUserProperty("user_raiting", raiting);
//
//        Dictionary<string, object> options = new Dictionary<string, object>() {
//            {"Raiting" , raiting}
//        };
//
//        _amplitude.logEvent("user_raiting", options);
        }

        public void SetUserRank(int rank)
        {
            /*_amplitude.setUserProperty("user_rank", rank);

            Dictionary<string, object> options = new Dictionary<string, object>() {
                {"Rank" , rank}
            };

            _amplitude.logEvent("user_rank", options);*/
        }

        public void AddMatchResult(string arena, bool withBot, int rank)
        {
            /*Dictionary<string, object> options = new Dictionary<string, object>() {
                {"Arena",  arena},
                {"WithBot",  withBot},
                {"PlayerRank", rank}
            };

            _amplitude.logEvent("user_match_result", options);*/
        }

        public void AddMatchEnter(string arena, bool withBot, int rank)
        {
            /*Dictionary<string, object> options = new Dictionary<string, object>() {
                {"Arena",  arena},
                {"WithBot",  withBot},
                {"PlayerRank", rank}
            };

            _amplitude.logEvent("user_match_enter", options);*/
        }

        public void AddMatchFirstSpawn(string arena, bool withBot, int rank, string itemName)
        {
            /*Dictionary<string, object> options = new Dictionary<string, object>() {
                {"Arena",  arena},
                {"WithBot",  withBot},
                {"PlayerRank", rank},
                {"TankName", tankName}
            };

            _amplitude.logEvent("user_match_first_spawn", options);*/
        }

        public void AddVictoryArena(string arena, int side, bool withBot, int rank)
        {
//        Dictionary<string, object> options = new Dictionary<string, object>() {
//            {"Arena",  arena},
//            {"WithBot",  withBot},
//            {"PlayerRank", rank},
//            {"Side", side}
//        };
//
//        _amplitude.logEvent("user_victory_arena", options);
        }

        public void AddSpawnItem(string tankName, int upgradeLevel, int side, bool withBot, int rank)
        {
//        Dictionary<string, object> options = new Dictionary<string, object>() {
//            {"TankName",  tankName},
//            {"Upgrade",  upgradeLevel},
//            {"WithBot",  withBot},
//            {"PlayerRank", rank},
//            {"Side", side}
//        };
//
//        _amplitude.logEvent("spawn_tank", options);
        }

        public void AddVictoryItem(string tankName, int upgradeLevel, int side, bool withBot, int rank)
        {
//        Dictionary<string, object> options = new Dictionary<string, object>() {
//            {"TankName",  tankName},
//            {"Upgrade",  upgradeLevel},
//            {"WithBot",  withBot},
//            {"PlayerRank", rank},
//            {"Side", side}
//        };
//
//        _amplitude.logEvent("victory_tank", options);
        }

        public void SetCoins(int count)
        {
            //_amplitude.setUserProperty("user_coins", count);
        }

        public void SetGems(int count)
        {
            //_amplitude.setUserProperty("user_gems", count);
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
            /*_amplitude.setUserProperty("user_spent_coins", count);*/
        }

        public void SpentGems(int count, int deltaCount, OutcomeMoneyReasons outcomeMoneyReason)
        {
            /*_amplitude.setUserProperty("user_spent_gems", count);*/
        }

        public void SpentSkillPoints(int count, int deltaCount, OutcomeMoneyReasons outcomeMoneyReason)
        {

        }

        public void TutorialStep(string stepName, int stepIndex, int fps)
        {
            /*_amplitude.setUserProperty("tutorial_step", stepName);
            _amplitude.setUserProperty("tutorial_step_index", stepIndex);

            Dictionary<string, object> options = new Dictionary<string, object>() {
                {"TutorialStep",  stepName},
                {"TutorialStepIndex",  stepIndex},
                {"FPS",  fps}
            };

            _amplitude.logEvent("tutorial_step", options);*/
        }

        public void EnterNameStep()
        {
            /*_amplitude.logEvent("enter_name");*/
        }

        public void GameLoader(int step)
        {
            /*Dictionary<string, object> options = new Dictionary<string, object>() {
                {"Step",  step}
            };

            _amplitude.logEvent("game_loader", options);*/
        }

        public void GameLoaderMessage(string message)
        {
            /*Dictionary<string, object> options = new Dictionary<string, object>() {
                {"Message",  message}
            };

            _amplitude.logEvent("game_loader_message", options);*/
        }

        public void TutorialFinish()
        {
            /*_amplitude.logEvent("tutorial_finish");*/
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
