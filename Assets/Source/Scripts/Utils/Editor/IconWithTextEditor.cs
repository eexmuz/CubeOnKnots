using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IconWithText))]
public class IconWithTextEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Adjust"))
            (target as IconWithText).SetText("44");
    }
}
