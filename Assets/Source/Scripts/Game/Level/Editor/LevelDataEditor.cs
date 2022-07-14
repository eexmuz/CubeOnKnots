using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelConfig))]
public class LevelDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var duplicate = ((LevelConfig) target).CellsData
            .GroupBy(c => c.Coords)
            .FirstOrDefault(g => g.Count() > 1);

        if (duplicate != null)
        {
            EditorGUILayout.HelpBox("Cell collision: " + duplicate.Key, MessageType.Error);
        }
        
        DrawBoardPreview();
    }

    private void DrawBoardPreview()
    {
        LevelConfig levelConfig = target as LevelConfig;
        Coords coords = new Coords();
        GUIStyle style = GUI.skin.box;
        style.alignment = TextAnchor.MiddleCenter;

        EditorGUI.BeginDisabledGroup(true);
        for (coords.y = levelConfig.Dimensions.y - 1; coords.y >= 0; coords.y--)
        {
            EditorGUILayout.BeginHorizontal();
            for (coords.x = 0; coords.x < levelConfig.Dimensions.x; coords.x++)
            {
                BlockInfo blockInfo = levelConfig.CellsData.FirstOrDefault(c => c.Coords.Equals(coords))?.BlockInfo;
                string display = "";
                if (blockInfo != null)
                {
                    if (blockInfo.Mergeable == false)
                    {
                        display = "BARRIER";
                    }
                    else
                    {
                        display = $"{1 << blockInfo.PowerOfTwo}";
                        if (blockInfo.Movable == false)
                        {
                            display = $"[[{display}]]";
                        }
                    }
                }
                
                EditorGUILayout.TextField(display, style, GUILayout.Width(60f), GUILayout.Height(60f));
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.EndDisabledGroup();
    }
}