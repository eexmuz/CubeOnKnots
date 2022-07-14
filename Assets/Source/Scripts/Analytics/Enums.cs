namespace Analytics
{
    public enum IncomeMoneyReasons
    {
        LevelUp,
        ItemUpgrade,
        ConvertHardToSoft,
        IapShop,
        DailyMission,
        LobbyChestOpen,
        ShopChest,
        IapOffer,
        LobbyChestUnlockNow,
        ShopFreeChest,
        MatchEnd,
        MatchEndAdv,
        TrophyRoad
    }

    public enum OutcomeMoneyReasons
    {
        None,
        LobbyChestUnlockNow,
        ShopChestUnlockNow,
        FastConvertHardToSoft,
        ShopSlotItem,
        SkillUpgrade,
        ConvertHardToSoft,
        ItemUpgrade,
        ResultChestUnlockNow,
        BuyCards,
        BuyItem
    }

    public enum MatchMakingResult
    {
        Matched,
        Left,
        Disconnected
    }
}