using System.Collections.Generic;
using Aig.Client.Integration.Runtime.Analytics;

public class LevelInfo
{
    public const string LEVEL_NUMBER = "level_number";
    public const string LEVEL_COUNT = "level_count";

    public int LevelNumber = 0;
    public int LevelCount = 0;

    public override string ToString()
    {
        return $"LevelNumber: {LevelNumber}, LevelCount: {LevelCount}";
    }

    public static void ParseAndAddLevelInfo(LevelInfo levelInfo, Dictionary<string, object> toDictionary)
    {
        if (levelInfo == null)
            return;

        toDictionary.Add(LEVEL_NUMBER, levelInfo.LevelNumber);
        toDictionary.Add(LEVEL_COUNT, levelInfo.LevelCount);
    }
}