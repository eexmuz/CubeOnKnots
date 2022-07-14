using System.Collections;
using System.Collections.Generic;
using Core;
using Core.Settings;
using Core.Attributes;

namespace Analytics
{
    public class YandexMetricaProvider : DIBehaviour, IAnalyticsProvider
    {
        [Inject]
        private ApplicationSettings _applicationSettings;

/*    [Inject]
    private ITutorialService _tutorialService;

    [Inject]
    private IPlayFabService _playFabService;*/

        private bool _activated;

        public void initialize()
        {
            StartCoroutine(WaitForActivation());
        }

        private Dictionary<string, object> GenerateDefaultDict()
        {
            Dictionary<string, object> temp = new Dictionary<string, object>()
            {
                /*{ "level", UserDBManager.user.Current.playerLevel.GetValue()},
            { "profileID", UserDBManager.user.Current.playFabId},
            { "battles", UserDBManager.user.Current.battleCount},
            { "paid_total", UserDBManager.user.Current.paidTotal},
            { "region", UserDBManager.user.Current.photonNetworkRegion},
            { "social",_playFabService.IsLoggedInFacebook ? "fb" : string.Empty },
            { "clan_status", false},
            { "payer", UserDBManager.user.Current.paidTotal > 0f},
            { "rating", UserDBManager.user.Current.playerRating.GetValue()},
            { "league", UserDBManager.user.Current.playerRank.GetValue()}*/
            };

            /*var userProfile = new YandexAppMetricaUserProfile();

        userProfile.ApplyFromArray(new List<YandexAppMetricaUserProfileUpdate>
        {
            new YandexAppMetricaNumberAttribute("paid_total").WithValue(UserDBManager.user.Current.paidTotal),
            new YandexAppMetricaNumberAttribute("level").WithValue(UserDBManager.user.Current.playerLevel.GetValue()),
            new YandexAppMetricaNumberAttribute("battles").WithValue(UserDBManager.user.Current.battleCount),
            new YandexAppMetricaNumberAttribute("hard_currency_blance").WithValue(UserDBManager.user.Current.gems.GetValue()),
            new YandexAppMetricaNumberAttribute("soft_currency_blance").WithValue(UserDBManager.user.Current.coins.GetValue()),
            new YandexAppMetricaNumberAttribute("upgrade_currency_blance").WithValue(UserDBManager.user.Current.skillPoints.GetValue()),
            new YandexAppMetricaNumberAttribute("rating").WithValue(UserDBManager.user.Current.playerRating.GetValue()),
            new YandexAppMetricaNumberAttribute("league").WithValue(UserDBManager.user.Current.playerRank.GetValue()),
            new YandexAppMetricaBooleanAttribute("payer").WithValue(UserDBManager.user.Current.paidTotal > 0f)
        });

        AppMetrica.Instance.ReportUserProfile(userProfile);*/

            return temp;
        }

        private string GetItemIdFromIncomeMoneyReasons(IncomeMoneyReasons incomeMoneyReasons)
        {
            string itemId = string.Empty;

            /*switch (incomeMoneyReasons)
        {
            case IncomeMoneyReasons.ConvertHardToSoft:
            case IncomeMoneyReasons.LobbyChestOpen:
            case IncomeMoneyReasons.LobbyChestUnlockNow:
            case IncomeMoneyReasons.DailyMission:
            case IncomeMoneyReasons.ShopChest:
            case IncomeMoneyReasons.ShopFreeChest:
            case IncomeMoneyReasons.TankUpgrade:
                itemId = "item/" + AnalyticState.BuyItemAnalyticState.ItemRecord.idItemType + "/" + AnalyticState.BuyItemAnalyticState.ItemRecord.code;
                break;

            case IncomeMoneyReasons.LevelUp:
                itemId = "user_level_up/" + UserDBManager.user.Current.playerLevel.GetValue();
                break;

            case IncomeMoneyReasons.MatchEnd:
            case IncomeMoneyReasons.MatchEndAdv:
                break;

            case IncomeMoneyReasons.IapOffer:
            case IncomeMoneyReasons.IapShop:
                itemId = "iap/" + AnalyticState.IapAnalyticState.productId;
                break;
        }*/

            return itemId;
        }

        private string GetItemIdFromOutcomeMoneyReasons(OutcomeMoneyReasons outcomeMoneyReasons)
        {
            string itemId = string.Empty;

            /*switch (outcomeMoneyReasons)
        {
            case OutcomeMoneyReasons.ConvertHardToSoft:
            case OutcomeMoneyReasons.FastConvertHardToSoft:
            case OutcomeMoneyReasons.LobbyChestUnlockNow:
            case OutcomeMoneyReasons.ShopChestUnlockNow:
            case OutcomeMoneyReasons.ShopSlotTank:
            case OutcomeMoneyReasons.SkillUpgrade:
            case OutcomeMoneyReasons.TankUpgrade:
                itemId = AnalyticState.BuyItemAnalyticState.category + "/" + 
                         AnalyticState.BuyItemAnalyticState.ItemRecord.idItemType + "/" + 
                         AnalyticState.BuyItemAnalyticState.ItemRecord.code;
                break;
        }*/

            return itemId;
        }

        private IEnumerator WaitForActivation()
        {
            /*while (UserDBManager.user == null 
               || UserDBManager.user.Current == null
               || string.IsNullOrEmpty(UserDBManager.user.Current.playFabId)
               || _applicationSettings == null 
               || _tutorialService == null)
        {
            yield return null;
        }
        AppMetrica.Instance.SetUserProfileID(UserDBManager.user.Current.playFabId);*/
        
            yield return null;
            _activated = true;
        }

        public void BuyChestInShop(string chestName)
        {
            /*if (!_activated)
        {
            return;
        }*/
        }

        public void Revenue(string productId, int count, float cost)
        {
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
            /*Dictionary<string, object> param = GenerateDefaultDict();
        param.Add("map", arena);
        param.Add("game_mode", AnalyticState.GameAnalyticState.gameMode);
        param.Add("vs_bot", withBot);

        AppMetrica.Instance.ReportEvent("player_battle_start", param);*/
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
            /*if (count <= 0)
        {
            return;
        }

        Dictionary<string, object> param = GenerateDefaultDict();
        param.Add("value", count);
        param.Add("category", incomeMoneyReasons.ToString());
        param.Add("screen", AnalyticState.WindowAnalyticState.WindowName);
        param.Add("item_id", GetItemIdFromIncomeMoneyReasons(incomeMoneyReasons));

        AppMetrica.Instance.ReportEvent("soft_currency", param);*/
        }

        public void AddGems(int count, IncomeMoneyReasons incomeMoneyReasons)
        {
            /*if (count <= 0)
        {
            return;
        }

        Dictionary<string, object> param = GenerateDefaultDict();
        param.Add("value", count);
        param.Add("category", incomeMoneyReasons.ToString());
        param.Add("screen", AnalyticState.WindowAnalyticState.WindowName);
        param.Add("item_id", GetItemIdFromIncomeMoneyReasons(incomeMoneyReasons));

        AppMetrica.Instance.ReportEvent("hard_currency", param);*/
        }

        public void AddSkillPoints(int count, IncomeMoneyReasons incomeMoneyReasons)
        {
            /*if (count <= 0)
        {
            return;
        }

        Dictionary<string, object> param = GenerateDefaultDict();
        param.Add("value", count);
        param.Add("category", incomeMoneyReasons.ToString());
        param.Add("screen", AnalyticState.WindowAnalyticState.WindowName);
        param.Add("item_id", GetItemIdFromIncomeMoneyReasons(incomeMoneyReasons));

        AppMetrica.Instance.ReportEvent("upgrade_currency", param);*/
        }

        public void SpentCoins(int count, int deltaCount, OutcomeMoneyReasons outcomeMoneyReason)
        {
            /*if (deltaCount <= 0)
        {
            return;
        }

        Dictionary<string, object> param = GenerateDefaultDict();
        param.Add("value", -deltaCount);
        param.Add("category", outcomeMoneyReason.ToString());
        param.Add("screen", AnalyticState.WindowAnalyticState.WindowName);
        param.Add("item_id", GetItemIdFromOutcomeMoneyReasons(outcomeMoneyReason));

        AppMetrica.Instance.ReportEvent("soft_currency", param);*/
        }

        public void SpentGems(int count, int deltaCount, OutcomeMoneyReasons outcomeMoneyReason)
        {
            /*if (deltaCount <= 0)
        {
            return;
        }

        Dictionary<string, object> param = GenerateDefaultDict();
        param.Add("value", -deltaCount);
        param.Add("category", outcomeMoneyReason.ToString());
        param.Add("screen", AnalyticState.WindowAnalyticState.WindowName);
        param.Add("item_id", GetItemIdFromOutcomeMoneyReasons(outcomeMoneyReason));

        AppMetrica.Instance.ReportEvent("hard_currency", param);*/
        }

        public void SpentSkillPoints(int count, int deltaCount, OutcomeMoneyReasons outcomeMoneyReason)
        {
            /*if (deltaCount <= 0)
        {
            return;
        }

        Dictionary<string, object> param = GenerateDefaultDict();
        param.Add("value", -deltaCount);
        param.Add("category", outcomeMoneyReason.ToString());
        param.Add("screen", AnalyticState.WindowAnalyticState.WindowName);
        param.Add("item_id", GetItemIdFromOutcomeMoneyReasons(outcomeMoneyReason));

        AppMetrica.Instance.ReportEvent("upgrade_currency", param);*/
        }

        public void TutorialStep(string stepName, int stepIndex, int fps)
        {
            /*Dictionary<string, object> param = GenerateDefaultDict();
        param.Add("step", String.Format("{0:00}_{1}", stepIndex, stepName));

        AppMetrica.Instance.ReportEvent("tutorial", param);*/
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
            /*Dictionary<string, object> param = GenerateDefaultDict();
        param.Add("step", "finish");

        AppMetrica.Instance.ReportEvent("tutorial", param);*/
        }

        public void Matchmaking(int waitTime, MatchMakingResult matchMakingResult)
        {
            /*Dictionary<string, object> param = GenerateDefaultDict();
        param.Add("wait_time", waitTime);
        param.Add("game_mode", AnalyticState.GameAnalyticState.gameMode);
        param.Add("result", matchMakingResult.ToString());

        AppMetrica.Instance.ReportEvent("matchmaking", param);*/
        }

        public void PlayerBattleFinish()
        {
            /*Dictionary<string, object> param = GenerateDefaultDict();
        param.Add("damage", AnalyticState.GameAnalyticState.damage);
        param.Add("incoming_damage", AnalyticState.GameAnalyticState.incomingDamage);
        param.Add("tank_spawns", AnalyticState.GameAnalyticState.tankSpawns);
        param.Add("slots", AnalyticState.GameAnalyticState.tankSlots);
        param.Add("duration", AnalyticState.GameAnalyticState.battleDuration);
        param.Add("finish_reason", AnalyticState.GameAnalyticState.userBattleFinishReason.ToString());
        param.Add("game_mode", AnalyticState.GameAnalyticState.gameMode);
        param.Add("kills", AnalyticState.GameAnalyticState.kills);
        param.Add("map", AnalyticState.GameAnalyticState.mapName);
        param.Add("map_side", AnalyticState.GameAnalyticState.mapSide);
        param.Add("player_result", AnalyticState.GameAnalyticState.victory);
        param.Add("soft_reward", AnalyticState.GameAnalyticState.softReward);
        param.Add("end_type", AnalyticState.GameAnalyticState.gameEndType.ToString());
        param.Add("vs_bot", AnalyticState.GameAnalyticState.withBot);
        param.Add("rating_reward", AnalyticState.GameAnalyticState.raitingReward);

        AppMetrica.Instance.ReportEvent("player_battle_finish", param);*/
        }

        public void SendItemInfo()
        {
            /*Dictionary<string, object> param = GenerateDefaultDict();
        param.Add("damage", AnalyticState.TankAnalyticState.damage);
        param.Add("incoming_damage", AnalyticState.TankAnalyticState.incomingDamage);
        param.Add("ability1_uses", AnalyticState.TankAnalyticState.abilityUses[0]);
        param.Add("ability2_uses", AnalyticState.TankAnalyticState.abilityUses[1]);
        param.Add("ability3_uses", AnalyticState.TankAnalyticState.abilityUses[2]);
        param.Add("tank_name", AnalyticState.TankAnalyticState.tankName);
        param.Add("tank_level", AnalyticState.TankAnalyticState.tankLevel);
        param.Add("lifetime", Mathf.RoundToInt(Time.time - AnalyticState.TankAnalyticState.startLifeTime));
        param.Add("shots_fired", AnalyticState.TankAnalyticState.shotsFired);
        param.Add("shots_hit", AnalyticState.TankAnalyticState.shotsHit);

        AppMetrica.Instance.ReportEvent("tank_spawn", param);*/
        }

        public void GetChest(string chestName)
        {
            /*Dictionary<string, object> param = GenerateDefaultDict();
        param.Add("source", AnalyticState.WindowAnalyticState.WindowName);
        param.Add("chest_slots", UserDBManager.user.GetFreeChestSlotCount());
        param.Add("type", chestName);

        AppMetrica.Instance.ReportEvent("chest_get", param);*/
        }

        public void OpenChest(string chestName)
        {
            /*Dictionary<string, object> param = GenerateDefaultDict();
        param.Add("type", chestName);

        AppMetrica.Instance.ReportEvent("chest_open", param);*/
        }

        public void GetCards(string tankName, int count, string source)
        {
            /*Dictionary<string, object> param = GenerateDefaultDict();
        param.Add("tank_name", tankName);
        param.Add("cards_count", count);
        param.Add("source", source);

        AppMetrica.Instance.ReportEvent("cards_get", param);*/
        }

        public void VideoAdsWatch(string source, string result)
        {
            /*Dictionary<string, object> param = GenerateDefaultDict();
        param.Add("result", result);
        param.Add("placement", source);

        AppMetrica.Instance.ReportEvent("video_ads_watch", param);*/
        }

        public void SkillTreeUpgrade(string skillName, string branchName, int level)
        {
            /*Dictionary<string, object> param = GenerateDefaultDict();
        param.Add("mastery_tree", branchName);
        param.Add("mastery_name", skillName);
        param.Add("mastery_level", level);

        AppMetrica.Instance.ReportEvent("mastery_upgrade", param);*/
        }

        public void Transaction(string productId, string currency, float price)
        {
            /*Dictionary<string, object> param = GenerateDefaultDict();
        param.Add("inapp_id", productId);
        param.Add("currency", currency);
        param.Add("price", price);

        AppMetrica.Instance.ReportEvent("transaction", param);*/
        }
    }
}
