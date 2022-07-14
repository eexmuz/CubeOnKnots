using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Level Config")]
public class LevelConfig : ScriptableObject
{
    public LevelStatus DefaultLevelStatus = new LevelStatus();
    public IntPOT TargetValue;
    [Tooltip("X = 0 stars, Y = 1 star, Z = 2 stars")]
    public int3 StarMoves;
    public int2 Dimensions;
    public List<BoardCellData> CellsData;

    public int LevelIndex { get; set; }

    public int CalculateStars(int moves)
    {
        int stars = 3;
        for (int i = 2; i >= 0; i--)
        {
            if (moves >= StarMoves[i])
            {
                stars--;
            }
            else
            {
                break;
            }
        }

        return stars;
    }
}
