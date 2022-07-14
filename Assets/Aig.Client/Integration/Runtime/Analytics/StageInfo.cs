using System.Collections.Generic;
using Aig.Client.Integration.Runtime.Analytics;

public class StageInfo
{
    public const string STAGE_NUMBER = "stage_number";
    public const string STAGE_NAME = "stage_name";
    public const string STAGE_COUNT = "stage_count";

    public int StageNumber = 0;
    public string StageName = string.Empty;
    public int StageCount = 0;

    public override string ToString()
    {
        return $"StageNumber: {StageNumber}, StageName: {StageName}, StageCount: {StageCount}";
    }

    public static void ParseAndAddStageInfo(StageInfo stageInfo, Dictionary<string, object> toDictionary)
    {
        if (stageInfo == null)
            return;

        toDictionary.Add(STAGE_NUMBER, stageInfo.StageNumber);
        toDictionary.Add(STAGE_NAME, stageInfo.StageName);
        toDictionary.Add(STAGE_COUNT, stageInfo.StageCount);
    }
}